using BinaryExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LibARMP.IO
{
    public static class ArmpFileWriter
    {
        static ArmpFileWriter()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }


        /// <summary>
        /// Writes an <see cref="ARMP"/> to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="armp">The <see cref="ARMP"/> to write.</param>
        /// <param name="stream">The destination <see cref="Stream"/>.</param>
        private static void WriteARMP(ARMP armp, Stream stream)
        {
            bool isOldEngine = armp.FormatVersion == Version.OldEngine || armp.FormatVersion == Version.OldEngineIshin;

            Encoding writerEncoding = Encoding.UTF8;
            if (isOldEngine)
            {
                writerEncoding = Encoding.GetEncoding(932); // Shift JIS
            }

            BinaryWriter writer = new BinaryWriter(stream, writerEncoding, true);

            writer.Write("armp", false); //Magic
            if (isOldEngine) writer.Write(0x02010000, true); // Endianness identifier for OE
            else writer.Write(0);
            if (isOldEngine) // Version and Revision are flipped on different endianess. Presumably both values are read together as an int32
            {
                writer.Write(armp.Version, true);
                writer.Write(armp.Revision, true);
            }
            else
            {
                writer.Write(armp.Revision);
                writer.Write(armp.Version);
            }
            writer.Write(0); // File size (only used in OE, placeholder for now)

            if (isOldEngine)
            {
                WriteTableOE(writer, armp.MainTable);
            }
            else
            {
                writer.WriteTimes(0, 0x10); // Dummy main table pointer and padding
                uint mainPtr = WriteTableRecursive(writer, armp.MainTable);
                writer.BaseStream.Seek(0x10);
                writer.Write(mainPtr);
            }

            // Reset the stream position
            writer.BaseStream.Seek(0);
        }



        /// <summary>
        /// Writes an <see cref="ARMP"/> to a file.
        /// </summary>
        /// <param name="armp">The <see cref="ARMP"/> to write.</param>
        /// <param name="path">The destination file path.</param>
        public static void WriteARMPToFile(ARMP armp, string path)
        {
            using (Stream stream = new MemoryStream())
            {
                WriteARMP(armp, stream);
                File.WriteAllBytes(path, stream.ToArray());
            }
        }



        /// <summary>
        /// Writes an <see cref="ARMP"/> to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="armp">The <see cref="ARMP"/> to write.</param>
        public static Stream WriteARMPToStream(ARMP armp)
        {
            Stream stream = new MemoryStream();
            WriteARMP(armp, stream);
            return stream;
        }



        /// <summary>
        /// Writes an <see cref="ARMP"/> to a byte array.
        /// </summary>
        /// <param name="armp">The <see cref="ARMP"/> to write.</param>
        public static byte[] WriteARMPToArray(ARMP armp)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                WriteARMP(armp, stream);
                return stream.ToArray();
            }
        }



        /// <summary>
        /// Writes an OE table to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/>.</param>
        /// <param name="table">The <see cref="ArmpTable"/> to write.</param>
        private static void WriteTableOE(BinaryWriter writer, ArmpTable table)
        {
            long baseOffset = writer.BaseStream.Position;
            int ptr = 0;

            writer.WriteTimes(0, 0x40); // Placeholder table

            ///// Entry Count /////
            writer.WriteAtPosition(table.Entries.Count, baseOffset, true);

            ///// Entry Validity /////
            #region EntryValidity

            if (table.TableInfo.HasEntryValidity)
            {
                List<bool> entryValidity = new List<bool>();
                ptr = (int)writer.BaseStream.Position;

                // Ishin
                if (table.TableInfo.FormatVersion == Version.OldEngineIshin)
                {
                    // Ishin stores these booleans as int32
                    foreach (ArmpEntry entry in table.Entries)
                    {
                        writer.Write(entry.IsValid ? 1 : 0, true);
                    }
                    // Update the main table pointer at 0x4
                    writer.WriteAtPosition(ptr, baseOffset + 0x4, true);
                }
                // 0, K1, FOTNS
                else
                {
                    foreach (ArmpEntry entry in table.Entries)
                    {
                        entryValidity.Add(entry.IsValid);
                    }
                    Util.WriteBooleanBitmask(writer, entryValidity, true);
                    // Update the main table pointer at 0xC
                    writer.WriteAtPosition(ptr, baseOffset + 0xC, true);
                }

                writer.WritePadding(0, 0x10);
            }
            #endregion


            ///// Entry Names /////
            #region EntryNames

            if (table.TableInfo.HasEntryNames)
            {
                List<string> entryNames = new List<string>();
                foreach (ArmpEntry entry in table.Entries)
                {
                    entryNames.Add(entry.Name);
                }
                ptr = Util.WriteText(writer, entryNames, true);
                // Update the main table pointer at 0x8
                writer.WriteAtPosition(ptr, baseOffset + 0x8, true);
                
                writer.WritePadding(0, 0x10);
            }
            #endregion


            ///// Column Names and Count /////
            #region ColumnNames

            if (table.TableInfo.HasColumnNames)
            {
                ptr = Util.WriteText(writer, table.GetColumnNames(), true);
                // Update the main table column count at 0x10 and pointer to column names at 0x14
                writer.WriteAtPosition(table.Columns.Count, baseOffset + 0x10, true);
                writer.WriteAtPosition(ptr, baseOffset + 0x14, true);
            }
            #endregion


            ///// Text /////
            #region Text

            if (table.TableInfo.HasText)
            {
                table.UpdateTextList();

                ptr = Util.WriteText(writer, table.Text, true);
                // Update the main table text offset table pointer at 0x28 and text count at 0x2C
                writer.WriteAtPosition(ptr, baseOffset + 0x28, true);
                writer.WriteAtPosition(table.Text.Count, baseOffset + 0x2C, true);
            }
            #endregion


            ///// Column Types /////
            #region ColumnTypes

            ptr = (int)writer.BaseStream.Position;
            foreach (ArmpTableColumn column in table.Columns)
            {
                int typeID = column.Type.GetID(table.TableInfo.FormatVersion);
                writer.Write(typeID, true);
            }
            // Update the main table pointer at 0x18
            writer.WriteAtPosition(ptr, baseOffset + 0x18, true);
            #endregion


            ///// Column Metadata /////
            #region ColumnMetadata

            if (table.TableInfo.HasColumnMetadata)
            {
                ptr = (int)writer.BaseStream.Position;
                foreach (ArmpTableColumn column in table.Columns)
                {
                    writer.Write(column.ColumnMetadata, true);
                }
                // Update the main table pointer at 0x24
                writer.WriteAtPosition(ptr, baseOffset + 0x24, true);
            }
            #endregion


            ///// Column Contents /////
            #region ColumnContents

            List<int> columnValueOffsets = new List<int>();
            foreach (ArmpTableColumn column in table.Columns)
            {
                if (column.Type.IDOE == -1)
                {
                    columnValueOffsets.Add(0);
                }
                else
                {
                    columnValueOffsets.Add((int)writer.BaseStream.Position);

                    // Write operations based on column type
                    if (column.Type.CSType == typeof(string))
                    {
                        foreach (ArmpEntry entry in table.Entries)
                        {
                            string value = entry.GetValueFromColumn<string>(column.Name);
                            if (value != null)
                            {
                                int index = table.Text.IndexOf(value);
                                writer.Write((short)index, true);
                            }
                            else
                            {
                                writer.Write((short)-1, true);
                            }
                        }
                    }

                    else if (column.Type.CSType == typeof(bool))
                    {
                        List<bool> boolList = new List<bool>();
                        foreach (ArmpEntry entry in table.Entries)
                        {
                            boolList.Add(entry.GetValueFromColumn<bool>(column.Name));
                        }

                        if (boolList.Count > 0)
                        {
                            Util.WriteBooleanBitmask(writer, boolList, true);
                        }
                    }

                    else if (column.Type.CSType == typeof(byte))
                    {
                        foreach (ArmpEntry entry in table.Entries)
                            writer.Write(entry.GetValueFromColumn<byte>(column.Name));
                    }

                    else if (column.Type.CSType == typeof(sbyte))
                    {
                        foreach (ArmpEntry entry in table.Entries)
                            writer.Write(entry.GetValueFromColumn<sbyte>(column.Name));
                    }

                    else if (column.Type.CSType == typeof(UInt16))
                    {
                        foreach (ArmpEntry entry in table.Entries)
                            writer.Write(entry.GetValueFromColumn<UInt16>(column.Name), true);
                    }

                    else if (column.Type.CSType == typeof(Int16))
                    {
                        foreach (ArmpEntry entry in table.Entries)
                            writer.Write(entry.GetValueFromColumn<Int16>(column.Name), true);
                    }

                    else if (column.Type.CSType == typeof(UInt32))
                    {
                        foreach (ArmpEntry entry in table.Entries)
                            writer.Write(entry.GetValueFromColumn<UInt32>(column.Name), true);
                    }

                    else if (column.Type.CSType == typeof(Int32))
                    {
                        foreach (ArmpEntry entry in table.Entries)
                            writer.Write(entry.GetValueFromColumn<Int32>(column.Name), true);
                    }
                }
            }

            writer.WritePadding(0, 4);

            // Write the column value offset table
            int ptrColumnOffsetTable = (int)writer.BaseStream.Position;
            foreach (int offset in columnValueOffsets)
            {
                writer.Write(offset, true);
            }

            writer.WritePadding(0, 0x10);

            // Update the main table pointer at 0x1C
            writer.WriteAtPosition(ptrColumnOffsetTable, baseOffset + 0x1C, true);
            #endregion


            ///// Header File Size /////
            writer.WriteAtPosition((int)writer.BaseStream.Length, 0xC, true);
        }



        /// <summary>
        /// Recursively writes tables from the lowest level upwards.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/>.</param>
        /// <param name="table">The <see cref="ArmpTableBase"/> to write.</param>
        /// <returns>The pointer to the table.</returns>
        private static uint WriteTableRecursive(BinaryWriter writer, ArmpTableBase table)
        {
            List<string> tableColumns = table.GetColumnNamesByType<ArmpTable>(ordered: true);
            Dictionary<ArmpTableBase, uint> tablePointers = new Dictionary<ArmpTableBase, uint>();

            if (tableColumns.Count > 0)
            {
                ArmpEntry entry;
                IReadOnlyList<ArmpEntry> entries = table.GetAllEntries();
                for (int i = 0; i < entries.Count; i++)
                {
                    if (!table.TableInfo.HasOrderedEntries)
                        entry = entries[i];
                    else
                        entry = entries[(int)table.OrderedEntryIDs[i]];

                    foreach (string column in tableColumns)
                    {
                        try
                        {
                            ArmpTableBase tableValue = (ArmpTableBase)entry.GetValueFromColumn(column);
                            if (tableValue == null) continue;
                            uint tableValuePtr = WriteTableRecursive(writer, tableValue);
                            tablePointers.Add(tableValue, tableValuePtr);
                        }
                        catch
                        {
                        }
                    }
                }
            }

            uint indexerTablePtr = 0;
            if (table.GetType() == typeof(ArmpTable))
            {
                ArmpTable tableMain = (ArmpTable)table;
                if (tableMain.Indexer != null)
                {
                    indexerTablePtr = WriteTableRecursive(writer, tableMain.Indexer);
                    writer.WritePadding(0, 0x10);
                }
            }

            uint pointer = (uint)writer.BaseStream.Position;
            WriteTable(writer, table, tablePointers);
            writer.WritePadding(0, 8);
            writer.WriteAtPosition(indexerTablePtr, pointer + 0x3C);
            return pointer;
        }



        /// <summary>
        /// Writes a DE table to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/>.</param>
        /// <param name="table">The <see cref="ArmpTableBase"/> to write.</param>
        private static void WriteTable(BinaryWriter writer, ArmpTableBase table, Dictionary<ArmpTableBase, uint> tableValuePointers = null)
        {
            if (table.TableInfo.FormatVersion == Version.DragonEngineV2 && table.TableInfo.StorageMode == StorageMode.Structured && table.TableInfo.HasMemberInfo)
            {
                table.PackStructure();
            }

            long baseOffset = writer.BaseStream.Position;
            int ptr = 0;
            bool allTrue, allFalse;
            int paddingWidth = table.TableInfo.FormatVersion == Version.DragonEngineV2 ? 8 : 4; // TODO: A small handful of files have a different padding, manually set.

            writer.WriteTimes(0, 0x50); // Placeholder table


            ///// Entry/Column Count /////
            writer.BaseStream.PushToPosition(baseOffset);
            writer.Write(table.Entries.Count);
            writer.Write(table.Columns.Count);


            ///// Table ID and Storage Mode /////
            writer.BaseStream.Seek(baseOffset + 0x20);
            if (table.TableInfo.FormatVersion == Version.DragonEngineV2)
            {
                writer.WriteInt24(table.TableInfo.TableID);
                byte tableFlags = 0;
                if (table.TableInfo.StorageMode == StorageMode.Structured) tableFlags |= 1;
                if (table.TableInfo.UnknownFlag1) tableFlags |= 2;
                if (table.TableInfo.UnknownFlag2) tableFlags |= 4;
                if (table.TableInfo.UnknownFlag3) tableFlags |= 8;
                if (table.TableInfo.UnknownFlag4) tableFlags |= 16;
                if (table.TableInfo.DoNotUseRaw) tableFlags |= 32;
                if (table.TableInfo.MembersWellFormatted) tableFlags |= 64;
                if (table.TableInfo.IsProcessedForMemory) tableFlags |= 128;
                writer.Write(tableFlags);
            }
            else
            {
                writer.Write(0u);
            }
            writer.BaseStream.PopPosition();


            ///// Default Entry ID /////
            writer.WriteAtPosition(table.TableInfo.DefaultEntryID, baseOffset + 0xC);


            ///// Default Column ID /////
            writer.WriteAtPosition(table.TableInfo.DefaultColumnID, baseOffset + 0x2C);


            ///// Entry Validity //////
            #region EntryValidity

            List<bool> entryValidity = new List<bool>();
            allTrue = true;
            allFalse = true;
            foreach (ArmpEntry entry in table.Entries)
            {
                entryValidity.Add(entry.IsValid);
                allTrue &= entry.IsValid;
                allFalse &= !entry.IsValid;
            }

            if (allTrue && table.Entries.Count > 0) ptr = -1;    // Zero-entry corner case, e.g. ccc_ccc_action_set_pos.bin
            else if (allFalse) ptr = 0;
            else
            {
                ptr = (int)writer.BaseStream.Position;
                Util.WriteBooleanBitmask(writer, entryValidity, false);

                writer.WritePadding(0, paddingWidth);
            }

            // Update the main table pointer at 0x14
            writer.WriteAtPosition(ptr, baseOffset + 0x14);
            #endregion


            ///// Column Validity /////
            #region ColumnValidity

            List<bool> columnValidity = new List<bool>();
            allTrue = table.Columns.Count > 0;
            allFalse = true;
            foreach (ArmpTableColumn column in table.Columns)
            {
                columnValidity.Add(column.IsValid);
                allTrue &= column.IsValid;
                allFalse &= !column.IsValid;
            }

            if (allTrue) ptr = -1;
            else if (allFalse) ptr = 0;
            else
            {
                ptr = (int)writer.BaseStream.Position;
                Util.WriteBooleanBitmask(writer, columnValidity, false);

                writer.WritePadding(0, 4);
            }

            // Update the main table pointer at 0x38
            writer.WriteAtPosition(ptr, baseOffset + 0x38);
            #endregion


            ///// Entry Names /////
            #region EntryNames

            if (table.TableInfo.HasEntryNames)
            {
                List<string> entryNames = new List<string>();
                foreach (ArmpEntry entry in table.Entries)
                {
                    entryNames.Add(entry.Name);
                }
                ptr = Util.WriteText(writer, entryNames, false);

                // Update the main table pointer at 0x10
                writer.WriteAtPosition(ptr, baseOffset + 0x10);
            }
            #endregion


            ///// Column Names /////
            #region ColumnNames

            if (table.TableInfo.HasColumnNames)
            {
                ptr = Util.WriteText(writer, table.GetColumnNames(), false);

                // Update the main table pointer at 0x28
                writer.WriteAtPosition(ptr, baseOffset + 0x28);
            }
            #endregion


            ///// Text /////
            #region Text

            if (table.TableInfo.HasText)
            {
                table.UpdateTextList();

                ptr = Util.WriteText(writer, table.Text, false);

                // Update the main table text offset table pointer at 0x24
                writer.WriteAtPosition(ptr, baseOffset + 0x24);
                // Update the main table text count at 0x8
                writer.WriteAtPosition(table.Text.Count, baseOffset + 0x8);

                writer.WritePadding(0, paddingWidth);
            }
            else
            {
                if (table.TableInfo.FormatVersion == Version.DragonEngineV1)
                {
                    writer.WritePadding(0, 0x10);
                    writer.WriteAtPosition((int)writer.BaseStream.Position, baseOffset + 0x24);
                }
                else
                {
                    writer.WritePadding(0, paddingWidth);
                }
            }
            #endregion


            ///// Column Types /////
            #region ColumnTypes

            writer.WritePadding(0, paddingWidth);
            ptr = (int)writer.BaseStream.Position;
            foreach (ArmpTableColumn column in table.Columns)
            {
                sbyte typeID = column.Type.GetID(table.TableInfo.FormatVersion);
                writer.Write(typeID);
            }

            // Update the main table pointer at 0x18
            writer.WriteAtPosition(ptr, baseOffset + 0x18);

            #endregion


            ///// Member Types (V1) /////
            #region MemberTypes(v1)

            if (table.TableInfo.FormatVersion == Version.DragonEngineV1 && table.TableInfo.HasMemberInfo)
            {
                writer.WritePadding(0, 4); // True for most files
                ptr = (int)writer.BaseStream.Position;
                foreach (ArmpTableColumn column in table.Columns)
                {
                    sbyte typeID = column.Type.GetMemberTypeID(table.TableInfo.FormatVersion);
                    writer.Write(typeID);
                }

                // Update the main table pointer at 0x48
                writer.WriteAtPosition(ptr, baseOffset + 0x48);
                writer.WritePadding(0, 4); // True for most files
            }
            #endregion


            ///// Column Metadata (V1) /////
            #region ColumnMetadata(V1)

            if (table.TableInfo.FormatVersion == Version.DragonEngineV1 && table.TableInfo.HasColumnMetadata)
            {
                ptr = (int)writer.BaseStream.Position;
                foreach (ArmpTableColumn column in table.Columns)
                {
                    writer.Write(column.ColumnMetadata);
                }
                // Update the main table pointer at 0x20
                writer.WriteAtPosition(ptr, baseOffset + 0x20);
            }
            #endregion


            ///// Column Contents /////
            #region ColumnContentsModeColumn

            if (table.TableInfo.StorageMode == StorageMode.Column)
            {
                List<int> columnValueOffsets = new List<int>();
                foreach (ArmpTableColumn column in table.Columns)
                {
                    if (!column.IsValid)
                    {
                        columnValueOffsets.Add(0);
                    }
                    // The column has data
                    else
                    {
                        if (column.Type.CSType == typeof(bool))
                        {
                            writer.WritePadding(0, paddingWidth);

                            List<bool> boolList = new List<bool>();
                            allTrue = table.Entries.Count > 0;
                            allFalse = true;
                            bool temp;
                            foreach (ArmpEntry entry in table.Entries)
                            {
                                temp = (bool)entry.GetValueFromColumn(column.Name);
                                boolList.Add(temp);
                                allTrue &= temp;
                                allFalse &= !temp;
                            }

                            if (allTrue) columnValueOffsets.Add(-1);
                            else if (allFalse) columnValueOffsets.Add(0);
                            else
                            {
                                columnValueOffsets.Add((int)writer.BaseStream.Position);
                                Util.WriteBooleanBitmask(writer, boolList, false);
                            }
                        }
                        else
                        {
                            columnValueOffsets.Add((int)writer.BaseStream.Position);

                            // Write operations based on column type
                            if (column.Type.CSType == typeof(string))
                            {
                                foreach (ArmpEntry entry in table.Entries)
                                {
                                    string value = (string)entry.GetValueFromColumn(column.Name);
                                    if (value != null)
                                    {
                                        int index = table.Text.IndexOf(value);
                                        writer.Write(index);
                                    }
                                    else if (table.TableInfo.FormatVersion == Version.DragonEngineV1)
                                    {
                                        writer.Write(-1);
                                    }
                                    else
                                    {
                                        writer.Write(0);
                                    }
                                }
                            }

                            else if (column.Type.CSType == typeof(ArmpTable))
                            {
                                if (tableValuePointers != null)
                                {
                                    foreach (ArmpEntry entry in table.Entries)
                                    {
                                        if (entry.Data.ContainsKey(column.Name))
                                        {
                                            ArmpTable tableValue = (ArmpTable)entry.GetValueFromColumn(column.Name);
                                            if (tableValue != null)
                                            {
                                                uint tablePtr;
                                                tableValuePointers.TryGetValue(tableValue, out tablePtr);
                                                writer.Write((ulong)tablePtr);
                                            }
                                            else
                                            {
                                                writer.Write(0L);
                                            }
                                        }
                                        else
                                        {
                                            writer.Write(0L);
                                        }
                                    }
                                }
                            }

                            else if (column.Type.CSType == typeof(float))
                            {
                                foreach (ArmpEntry entry in table.Entries)
                                    writer.Write(entry.GetValueFromColumn<float>(column.Name));
                            }

                            else if (column.Type.CSType == typeof(double))
                            {
                                foreach (ArmpEntry entry in table.Entries)
                                    writer.Write(entry.GetValueFromColumn<double>(column.Name));
                            }

                            else if (column.Type.CSType == typeof(byte))
                            {
                                foreach (ArmpEntry entry in table.Entries)
                                    writer.Write(entry.GetValueFromColumn<byte>(column.Name));
                            }

                            else if (column.Type.CSType == typeof(sbyte))
                            {
                                foreach (ArmpEntry entry in table.Entries)
                                    writer.Write(entry.GetValueFromColumn<sbyte>(column.Name));
                            }

                            else if (column.Type.CSType == typeof(UInt16))
                            {
                                foreach (ArmpEntry entry in table.Entries)
                                    writer.Write(entry.GetValueFromColumn<UInt16>(column.Name));
                            }

                            else if (column.Type.CSType == typeof(Int16))
                            {
                                foreach (ArmpEntry entry in table.Entries)
                                    writer.Write(entry.GetValueFromColumn<Int16>(column.Name));
                            }

                            else if (column.Type.CSType == typeof(UInt32))
                            {
                                foreach (ArmpEntry entry in table.Entries)
                                    writer.Write(entry.GetValueFromColumn<UInt32>(column.Name));
                            }

                            else if (column.Type.CSType == typeof(Int32))
                            {
                                foreach (ArmpEntry entry in table.Entries)
                                    writer.Write(entry.GetValueFromColumn<Int32>(column.Name));
                            }

                            else if (column.Type.CSType == typeof(UInt64))
                            {
                                foreach (ArmpEntry entry in table.Entries)
                                    writer.Write(entry.GetValueFromColumn<UInt64>(column.Name));
                            }

                            else if (column.Type.CSType == typeof(Int64))
                            {
                                foreach (ArmpEntry entry in table.Entries)
                                    writer.Write(entry.GetValueFromColumn<Int64>(column.Name));
                            }

                            writer.WritePadding(0, paddingWidth);
                        }
                    }
                }

                if (table.Columns.Count > 0 || table.TableInfo.FormatVersion == Version.DragonEngineV1)
                {
                    // Write the column value offset table
                    writer.WritePadding(0, paddingWidth);
                    int ptrColumnOffsetTable = (int)writer.BaseStream.Position;
                    foreach (int offset in columnValueOffsets)
                    {
                        writer.Write(offset);
                    }

                    // Update the main table pointer at 0x1C
                    writer.WriteAtPosition(ptrColumnOffsetTable, baseOffset + 0x1C);
                }
            }
            #endregion


            #region ColumnContentsModeStructured

            else if (table.TableInfo.StorageMode == StorageMode.Structured)
            {
                List<int> entryValueOffsets = new List<int>();
                ArmpTableColumn column;
                foreach (ArmpEntry entry in table.Entries)
                {
                    writer.WritePadding(0, 0x10);

                    int startOffset = (int)writer.BaseStream.Position;
                    entryValueOffsets.Add(startOffset);

                    foreach (ArmpMemberInfo memberInfo in table.StructureSpec)
                    {
                        column = memberInfo.Column;
                        if (!column.IsValid || memberInfo.Type.IsArray || memberInfo.Position < 0) continue;

                        writer.BaseStream.Position = startOffset + memberInfo.Position;

                        // Write operations based on column type
                        if (memberInfo.Type.CSType == typeof(string))
                        {
                            string value = (string)entry.GetValueFromColumn(column.Name);
                            if (value != null)
                            {
                                long index = table.Text.IndexOf(value);
                                writer.Write(index);
                            }
                            else if (table.TableInfo.HasText)
                            {
                                writer.Write(-1L);
                            }
                            else
                            {
                                writer.Write(0L);
                            }
                        }

                        else if (memberInfo.Type.CSType == typeof(bool))
                        {
                            bool val = (bool)entry.GetValueFromColumn(column.Name);
                            writer.Write(Convert.ToByte(val));
                        }

                        else if (memberInfo.Type.CSType == typeof(ArmpTable))
                        {
                            try
                            {
                                ulong tablePtr = tableValuePointers[(ArmpTable)entry.GetValueFromColumn(column.Name)];
                                writer.Write(tablePtr);
                            }
                            catch
                            {
                                writer.Write(0L);
                            }
                        }

                        else if (memberInfo.Type.CSType == typeof(float))
                        {
                            writer.Write(entry.GetValueFromColumn<float>(column.Name));
                        }

                        else if (memberInfo.Type.CSType == typeof(double))
                        {
                            writer.Write(entry.GetValueFromColumn<double>(column.Name));
                        }

                        else if (memberInfo.Type.CSType == typeof(byte))
                        {
                            writer.Write(entry.GetValueFromColumn<byte>(column.Name));
                        }

                        else if (memberInfo.Type.CSType == typeof(sbyte))
                        {
                            writer.Write(entry.GetValueFromColumn<sbyte>(column.Name));
                        }

                        else if (memberInfo.Type.CSType == typeof(UInt16))
                        {
                            writer.Write(entry.GetValueFromColumn<UInt16>(column.Name));
                        }

                        else if (memberInfo.Type.CSType == typeof(Int16))
                        {
                            writer.Write(entry.GetValueFromColumn<Int16>(column.Name));
                        }

                        else if (memberInfo.Type.CSType == typeof(UInt32))
                        {
                            writer.Write(entry.GetValueFromColumn<UInt32>(column.Name));
                        }

                        else if (memberInfo.Type.CSType == typeof(Int32))
                        {
                            writer.Write(entry.GetValueFromColumn<Int32>(column.Name));
                        }

                        else if (memberInfo.Type.CSType == typeof(UInt64))
                        {
                            writer.Write(entry.GetValueFromColumn<UInt64>(column.Name));
                        }

                        else if (memberInfo.Type.CSType == typeof(Int64))
                        {
                            writer.Write(entry.GetValueFromColumn<Int64>(column.Name));
                        }
                    }
                    writer.BaseStream.Position = startOffset + table.StructureWidth;
                    writer.WritePadding(0, 8);
                }

                if (table.Entries.Count > 0)
                {
                    // Write the structure offset table
                    int ptrStructureOffsetTable = (int)writer.BaseStream.Position;
                    foreach (int offset in entryValueOffsets)
                    {
                        writer.Write(offset);
                    }

                    // Update the main table pointer at 0x1C
                    writer.WriteAtPosition(ptrStructureOffsetTable, baseOffset + 0x1C);
                }
            }
            #endregion


            ///// Blank Cell Flags /////
            #region BlankCellFlags

            if (table.TableInfo.HasBlankCellFlags)
            {
                int[] bcfOffsets = new int[table.Columns.Count];

                List<bool> blankCellFlags;
                List<ArmpEntry> entriesWithData;
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    entriesWithData = table.CellsWithData[table.Columns[i]];
                    if (entriesWithData == null || entriesWithData.Count == 0)
                    {
                        bcfOffsets[i] = -1;
                    }
                    else if (entriesWithData.Count == table.Entries.Count)
                    {
                        bcfOffsets[i] = 0;
                    }
                    else
                    {
                        bcfOffsets[i] = (int)writer.BaseStream.Position;
                        blankCellFlags = new List<bool>(table.Entries.Count);

                        foreach (ArmpEntry entry in table.Entries)
                        {
                            blankCellFlags.Add(!table.CellsWithData[table.Columns[i]].Contains(entry));
                        }

                        Util.WriteBooleanBitmask(writer, blankCellFlags, false);

                        writer.WritePadding(0, 4);
                    }
                }

                writer.WritePadding(0, paddingWidth);

                ptr = (int)writer.BaseStream.Position;
                foreach (int offset in bcfOffsets)
                {
                    writer.Write(offset);
                }

                writer.WriteAtPosition(ptr, baseOffset + 0x44);
            }
            #endregion


            ///// Entry Order /////
            #region EntryOrder

            if (table.TableInfo.HasOrderedEntries)
            {
                writer.WritePadding(0, paddingWidth);
                ptr = (int)writer.BaseStream.Position;

                foreach (uint ID in table.OrderedEntryIDs)
                {
                    writer.Write(ID);
                }

                // Update the main table pointer at 0x30
                writer.WriteAtPosition(ptr, baseOffset + 0x30);
            }
            #endregion


            ///// Column Order /////
            #region ColumnOrder

            if (table.TableInfo.HasOrderedColumns)
            {
                writer.WritePadding(0, paddingWidth);
                ptr = (int)writer.BaseStream.Position;

                foreach (uint ID in table.OrderedColumnIDs)
                {
                    writer.Write(ID);
                }

                // Update the main table pointer at 0x34
                writer.WriteAtPosition(ptr, baseOffset + 0x34);
            }
            #endregion


            ///// Game Var Column IDs /////
            #region GameVarColumnIDs

            if (table.TableInfo.HasGameVarColumns)
            {
                writer.WritePadding(0, paddingWidth);
                ptr = (int)writer.BaseStream.Position;
                foreach (ArmpTableColumn column in table.Columns)
                {
                    writer.Write(column.GameVarID);
                }
                // Update the main table pointer at 0x40
                writer.WriteAtPosition(ptr, baseOffset + 0x40);
            }
            #endregion


            if (table.TableInfo.FormatVersion == Version.DragonEngineV2) writer.WritePadding(0, 0x10);


            ///// Entry Info Flags (V1) /////
            #region EntryInfoFlags(V1)

            if (table.TableInfo.HasExtraFieldInfo && table.TableInfo.FormatVersion == Version.DragonEngineV1)
            {
                writer.WritePadding(0, 8); // Fine for now
                ptr = (int)writer.BaseStream.Position;
                foreach (ArmpEntry entry in table.Entries)
                {
                    string bitstring = "";
                    for (int i = 0; i < entry.Flags.Length; i++)
                    {
                        bitstring += Convert.ToByte(entry.Flags[i]);
                    }
                    byte value = Convert.ToByte(Util.ReverseString(bitstring), 2);
                    writer.Write(value);
                }
                // Update the main table pointer at 0x4C
                writer.WriteAtPosition(ptr, baseOffset + 0x4C);
            }
            #endregion


            ///// Member Specification (V2) /////
            #region MemberInfo(V2)

            if (table.TableInfo.FormatVersion == Version.DragonEngineV2 && table.TableInfo.HasMemberInfo)
            {
                writer.WritePadding(0, 0x10);
                ptr = (int)writer.BaseStream.Position;
                WriteMemberInfoTable(writer, table);
                // Update the main table pointer at 0x48
                writer.WriteAtPosition(ptr, baseOffset + 0x48);
            }
            #endregion


            ///// Array Specifications (V2) /////
            #region ArrayInfo(V2)

            if (table.TableInfo.FormatVersion == Version.DragonEngineV2 && table.TableInfo.HasExtraFieldInfo)
            {
                writer.WritePadding(0, 0x10);
                List<int> offsets = new List<int>();
                foreach (ArmpTableColumn column in table.Columns)
                {
                    if (column.Children?.Count > 0)
                    {
                        offsets.Add((int)writer.BaseStream.Position);

                        foreach (ArmpTableColumn child in column.Children)
                            writer.Write(child != null ? child.ID : 0);
                    }
                }
                offsets.Add(offsets.Count > 0 ? (int)writer.BaseStream.Position : 0);
                writer.WritePadding(0, 8);

                ptr = (int)writer.BaseStream.Position;

                foreach (ArmpTableColumn column in table.Columns)
                {
                    writer.Write(column.Children?.Count ?? 0);
                    writer.Write(offsets[0]);
                    writer.WriteTimes(0, 0x18);
                    if (column.Children?.Count > 0)
                    {
                        offsets.RemoveAt(0);
                    }
                }
                // Update the main table pointer at 0x4C
                writer.WriteAtPosition(ptr, baseOffset + 0x4C);
            }
            #endregion
        }


        /// <summary>
        /// Writes the Member Info table.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/>.</param>
        /// <param name="table">The <see cref="ArmpTableBase"/>.</param>
        private static void WriteMemberInfoTable(BinaryWriter writer, ArmpTableBase table)
        {
            // Use the list of columns to avoid having to sort MemberInfo by column index.
            foreach (ArmpTableColumn column in table.Columns)
            {
                writer.Write((int)column.MemberInfo.Type.GetMemberTypeID(table.TableInfo.FormatVersion)); // Member type
                writer.Write(column.MemberInfo.Position); // Position
                writer.Write(column.MemberInfo.ArraySize); // Array size
                writer.Write(0u); // Reserved bytes
            }
        }
    }
}
