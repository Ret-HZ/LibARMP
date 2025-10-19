using BinaryExtensions;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace LibARMP.IO
{
    public static class ArmpFileReader
    {
        static ArmpFileReader()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }


        /// <summary>
        /// Reads an armp file.
        /// </summary>
        /// <param name="stream">The armp file as <see cref="Stream"/>.</param>
        /// <param name="baseARMPMemoryAddress">The memory address where the armp starts at. Only needed if reading from memory.</param>
        /// <returns>An <see cref="ARMP"/> object.</returns>
        public static ARMP ReadARMP(Stream stream, IntPtr baseARMPMemoryAddress = default(IntPtr))
        {
            BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, true);
            reader.BaseStream.Seek(0, SeekOrigin.Begin); // Ensure the position is at the start of the stream

            ARMP armp = new ARMP();

            char[] magic = reader.ReadChars(4);
            int endianness = reader.ReadInt32(); // Only used in OE
            if (endianness == 258)
            {
                armp.FormatVersion = Version.OldEngine;
            }

            if (armp.FormatVersion == Version.OldEngine)
            {
                armp.Version = reader.ReadInt16(true);
                armp.Revision = reader.ReadInt16(true);
            }
            else
            {
                armp.Revision = reader.ReadInt16();
                armp.Version = reader.ReadInt16();
            }

#if DEBUG
            Console.WriteLine("Version: " + armp.Version);
            Console.WriteLine("Revision: " + armp.Revision);
#endif

            int fileSize = reader.ReadInt32(); // Only used in OE

            if (armp.FormatVersion == Version.OldEngine)
            {
                reader.BaseStream.PushToPosition(0xC, SeekOrigin.Current);
                // This value, usually corresponding to the entry validity pointer, is unused in Ishin since it is instead found at 0x4
                if (reader.ReadInt32(true) == 0)
                {
                    armp.FormatVersion = Version.OldEngineIshin;
                    reader.Dispose();
                    reader = new BinaryReader(stream, Encoding.GetEncoding(932), true);
                }
                reader.BaseStream.PopPosition();
                armp.MainTable = ReadTableOE(reader, armp.FormatVersion);
            }
            else
            {
                if (armp.Version == 1) armp.FormatVersion = Version.DragonEngineV1;
                else if (armp.Version == 2) armp.FormatVersion = Version.DragonEngineV2;
                uint ptrMainTable = reader.ReadUInt32();
                armp.MainTable = ReadTable(reader, ptrMainTable, armp.FormatVersion, baseARMPMemoryAddress.ToInt64());
            }

            reader.Dispose();
            return armp;
        }



        /// <summary>
        /// Reads an armp file.
        /// </summary>
        /// <param name="fileBytes">The armp file as byte array.</param>
        /// <param name="offset">The location in the array to start reading data from.</param>
        /// <param name="length">The number of bytes to read from the array.</param>
        /// <param name="baseARMPMemoryAddress">The memory address where the armp starts at. Only needed if reading from memory.</param>
        /// <returns>An <see cref="ARMP"/> object.</returns>
        public static ARMP ReadARMP(byte[] fileBytes, int offset = 0, int length = 0, IntPtr baseARMPMemoryAddress = default(IntPtr))
        {
            if (length == 0) length = fileBytes.Length;
            using (var stream = new MemoryStream(fileBytes, offset, length))
            {
                return ReadARMP(stream, baseARMPMemoryAddress);
            }
        }



        /// <summary>
        /// Reads an armp file.
        /// </summary>
        /// <param name="path">The path to the armp file.</param>
        /// <param name="baseARMPMemoryAddress">The memory address where the armp starts at. Only needed if reading from memory.</param>
        /// <returns>An <see cref="ARMP"/> object.</returns>
        public static ARMP ReadARMP(string path, IntPtr baseARMPMemoryAddress = default(IntPtr))
        {
            using (Stream stream = new MemoryStream(File.ReadAllBytes(path)))
            {
                return ReadARMP(stream, baseARMPMemoryAddress);
            }
        }



        /// <summary>
        /// Reads a Dragon Engine armp table (base).
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/>.</param>
        /// <param name="ptrTable">The pointer to the table.</param>
        /// <param name="baseARMPMemoryAddress">The memory address where the armp starts at. Only needed if reading from memory.</param>
        /// <returns>An <see cref="ArmpTableBase"/> object.</returns>
        private static ArmpTableBase ReadTableBase(BinaryReader reader, long ptrTable, Version version, long baseARMPMemoryAddress)
        {
            ArmpTableBase table = new ArmpTableBase();
            reader.BaseStream.Seek(ptrTable);
            table.TableInfo = GetARMPTableInfo(reader, version);
            table.TableInfo.BaseARMPMemoryAddress = baseARMPMemoryAddress;


            ///// Column Names /////
            #region ColumnNames

            List<string> columnNames = null;
            if (table.TableInfo.HasColumnNames)
            {
                columnNames = Util.IterateStringList(reader, Util.IterateOffsetList(reader, table.TableInfo.ptrColumnNamesOffsetTable, table.TableInfo.ColumnCount, false));
            }
            else
            {
                columnNames = new List<string>(table.TableInfo.ColumnCount);

                // Fill with numbers
                for (int c = 0; c < table.TableInfo.ColumnCount; c++)
                {
                    columnNames.Add(c.ToString());
                }
            }
            #endregion


            ///// Column Raw Data Types /////
            List<ArmpType> columnDataTypes = GetColumnDataTypes(reader, table.TableInfo.ptrColumnDataTypes, table.TableInfo.ColumnCount, version, false);


            ///// Member Info /////
            #region MemberInfo

            List<ArmpType> memberTypes = null;
            if (table.TableInfo.HasMemberInfo)
            {
                if (version == Version.DragonEngineV2)
                {
                    table.StructureSpec = ReadMemberInfoTable(reader, table.TableInfo.ptrMemberInfo, table.TableInfo.ColumnCount);
                }
                else
                {
                    memberTypes = GetColumnDataTypes(reader, table.TableInfo.ptrMemberInfo, table.TableInfo.ColumnCount, version, true);
                }
            }
            #endregion


            ///// Column Validity /////
            #region ColumnValidity

            List<bool> columnValidity = null;
            if (table.TableInfo.ptrColumnValidity != 0 && table.TableInfo.ptrColumnValidity != 0xFFFFFFFF)
            {
                columnValidity = Util.IterateBooleanBitmask(reader, table.TableInfo.ptrColumnValidity, table.TableInfo.ColumnCount, false);
            }
            else
            {
                columnValidity = new List<bool>(table.TableInfo.ColumnCount);
                bool[] values = new bool[table.TableInfo.ColumnCount];
                if (table.TableInfo.ptrColumnValidity == 0xFFFFFFFF)
                {
                    for (int i = 0; i < table.TableInfo.ColumnCount; i++) values[i] = true;
                }
                columnValidity.AddRange(values);
            }
            #endregion


            ///// Column Order /////
            #region ColumnOrder

            if (table.TableInfo.HasOrderedColumns)
            {
                table.OrderedColumnIDs = Util.IterateArray<int>(reader, table.TableInfo.ptrColumnOrder, table.TableInfo.ColumnCount, false);
            }
            else
            {
                table.OrderedColumnIDs = new List<int>(table.TableInfo.ColumnCount);
                for (int i = 0; i < table.TableInfo.ColumnCount; i++)
                    table.OrderedColumnIDs.Add(i);
            }
            #endregion


            ///// game_var Column IDs /////
            List<int> gameVarColumnIDs = null;
            if (table.TableInfo.HasGameVarColumns) gameVarColumnIDs = Util.IterateArray<int>(reader, table.TableInfo.ptrGameVarColumnIDs, table.TableInfo.ColumnCount, false);


            ///// Column Metadata (V1) /////
            List<int> columnMetadata = null;
            if (table.TableInfo.HasColumnMetadata) columnMetadata = Util.IterateArray<int>(reader, table.TableInfo.ptrColumnMetadata, table.TableInfo.ColumnCount, false);


            ///// Create Columns /////
            #region CreateColumns

            for (int c = 0; c < table.TableInfo.ColumnCount; c++)
            {
                ArmpTableColumn column = new ArmpTableColumn((uint)c, columnNames[c], columnDataTypes[c]);

                if (table.TableInfo.HasMemberInfo)
                {
                    // Use the aux type as main for v1. This is better than having to check for the correct type multiple times.
                    if (version == Version.DragonEngineV1)
                    {
                        column.Type = memberTypes[c];
                    }
                    else // v2
                    {
                        column.MemberInfo = table.StructureSpec[c];
                        table.StructureSpec[c].Column = column;
                    }
                }

                column.IsValid = columnValidity[c];
                column.Index = table.TableInfo.HasOrderedColumns ? table.OrderedColumnIDs.IndexOf(c) : c;
                if (table.TableInfo.HasGameVarColumns) column.GameVarID = gameVarColumnIDs[c];
                if (table.TableInfo.HasColumnMetadata) column.ColumnMetadata = columnMetadata[c];

                table.Columns.Add(column);
            }
            #endregion


            ///// Entry Names /////
            #region EntryNames

            if (table.TableInfo.HasEntryNames)
            {
                table.EntryNames = Util.IterateStringList(reader, Util.IterateOffsetList(reader, table.TableInfo.ptrEntryNamesOffsetTable, table.TableInfo.EntryCount, false));
            }
            else
            {
                // Fill with blanks
                table.EntryNames = new List<string>();
                for (int r = 0; r < table.TableInfo.EntryCount; r++)
                {
                    table.EntryNames.Add("");
                }
            }
            #endregion


            ///// Text /////
            if (table.TableInfo.HasText) table.Text = Util.IterateStringList(reader, Util.IterateOffsetList(reader, table.TableInfo.ptrTextOffsetTable, table.TableInfo.TextCount, false));


            ///// Entry Order /////
            #region EntryOrder

            if (table.TableInfo.HasOrderedEntries)
            {
                table.OrderedEntryIDs = Util.IterateArray<uint>(reader, table.TableInfo.ptrEntryOrder, table.TableInfo.EntryCount, false);
            }
            else
            {
                table.OrderedEntryIDs = new List<uint>(table.TableInfo.EntryCount);
                for (uint i = 0; i < table.TableInfo.EntryCount; i++)
                    table.OrderedEntryIDs.Add(i);
            }
            #endregion


            ///// Entry Data /////
            InitializeEntries(table);
            ReadColumnContents(reader, table.TableInfo.ptrColumnContentOffsetTable, table.TableInfo.StorageMode, version, table);


            ///// Entry Validity /////
            #region EntryValidity

            if (table.TableInfo.ptrEntryValidity != 0 && table.TableInfo.ptrEntryValidity != 0xFFFFFFFF)
            {
                table.EntryValidity = Util.IterateBooleanBitmask(reader, table.TableInfo.ptrEntryValidity, table.TableInfo.EntryCount, false);
            }
            else
            {
                table.EntryValidity = new List<bool>(table.TableInfo.EntryCount);
                bool[] values = new bool[table.TableInfo.EntryCount];
                if (table.TableInfo.ptrEntryValidity == 0xFFFFFFFF)
                {
                    for (int i = 0; i < table.TableInfo.EntryCount; i++) values[i] = true;
                }
                table.EntryValidity.AddRange(values);
            }
            SetEntryValidity(table.EntryValidity, table.Entries);
            #endregion


            ///// Extra Field Info /////
            #region ExtraFieldInfo

            if (table.TableInfo.HasExtraFieldInfo)
            {
                if (version == Version.DragonEngineV1) // Flags for each entry
                {
                    IterateEntryInfoFlags(reader, table.TableInfo.ptrExtraFieldInfo, table.Entries);
                }
                if (version == Version.DragonEngineV2)
                {
                    ReadArrayInfoTable(reader, table.TableInfo.ptrExtraFieldInfo, table);
                }
            }
            #endregion


            ///// Blank Cell Flags /////
            #region BlankCellFlags

            if (table.TableInfo.HasBlankCellFlags)
            {
                table.CellsWithData = new Dictionary<ArmpTableColumn, List<ArmpEntry>>();
                List<uint> bcfOffsetList = Util.IterateOffsetList(reader, table.TableInfo.ptrBlankCellFlagOffsetTable, table.TableInfo.ColumnCount, false);

                uint offset;
                ArmpTableColumn column;
                List<bool> blankCellFlags;
                List<ArmpEntry> entriesWithData;
                for (int i = 0; i < table.TableInfo.ColumnCount; i++)
                {
                    column = table.Columns[i];
                    offset = bcfOffsetList[i];

                    if ((int)offset < 0)
                    {
                        table.CellsWithData.Add(column, null); // Empty list will also work on write
                    }
                    else if (offset == 0)
                    {
                        entriesWithData = new List<ArmpEntry>(table.TableInfo.EntryCount);
                        entriesWithData.AddRange(table.Entries);
                        table.CellsWithData.Add(column, entriesWithData);
                    }
                    else
                    {
                        blankCellFlags = Util.IterateBooleanBitmask(reader, offset, table.TableInfo.EntryCount, false);
                        entriesWithData = new List<ArmpEntry>(table.TableInfo.EntryCount);
                        for (int j = 0; j < table.TableInfo.EntryCount; j++)
                        {
                            if (!blankCellFlags[j])
                                entriesWithData.Add(table.Entries[j]);
                        }
                        table.CellsWithData.Add(column, entriesWithData);
                    }
                }
            }
            #endregion


            // Now that file reading is over, the structure spec should be sorted by position to facilitate editing later.
            if (table.TableInfo.HasMemberInfo && version == Version.DragonEngineV2)
            {
                table.StructureSpec.Sort((x, y) => x.Position.CompareTo(y.Position));
                table.StructurePacked = true;

                uint width = 0;
                foreach (ArmpMemberInfo memberInfo in table.StructureSpec)
                {
                    if (memberInfo.Column == null || !memberInfo.Column.IsValid || memberInfo.Position < 0) continue;

                    width = memberInfo.Type.Size;
                    if (memberInfo.Type.IsArray) width *= memberInfo.ArraySize;
                    if (table.StructureWidth < memberInfo.Position + width) table.StructureWidth = (uint)memberInfo.Position + width;
                }
            }

            table.RefreshColumnNameCache();
            return table;
        }



        /// <summary>
        /// Reads a Dragon Engine armp table.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/>.</param>
        /// <param name="ptrTable">The pointer to the table.</param>
        /// <param name="version">The format version.</param>
        /// <param name="baseARMPMemoryAddress">The memory address where the armp starts at. Only needed if reading from memory.</param>
        /// <returns>An <see cref="ArmpTableBase"/> object.</returns>
        private static ArmpTable ReadTable(BinaryReader reader, long ptrTable, Version version, long baseARMPMemoryAddress)
        {
            ArmpTable mainTable = new ArmpTable(ReadTableBase(reader, ptrTable, version, baseARMPMemoryAddress));
            if (mainTable.TableInfo.HasIndexerTable) mainTable.Indexer = new ArmpTableIndexer(mainTable, ReadTableBase(reader, mainTable.TableInfo.ptrIndexerTable, version, baseARMPMemoryAddress));
            return mainTable;
        }



        /// <summary>
        /// Reads an Old Engine armp table (base).
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/>.</param>
        /// <param name="version">The format version.</param>
        /// <returns>An <see cref="ArmpTableBase"/> object.</returns>
        private static ArmpTableBase ReadTableBaseOE(BinaryReader reader, Version version)
        {
            ArmpTableBase table = new ArmpTableBase();

            table.TableInfo = GetARMPTableInfo(reader, version);


            ///// Column Names /////
            #region ColumnNames

            List<string> columnNames = new List<string>();
            if (table.TableInfo.HasColumnNames)
            {
                columnNames = Util.IterateStringList(reader, Util.IterateOffsetList(reader, table.TableInfo.ptrColumnNamesOffsetTable, table.TableInfo.ColumnCount, true));
            }
            else
            {
                // Fill with numbers
                for (int c = 0; c < table.TableInfo.ColumnCount; c++)
                {
                    columnNames.Add(c.ToString());
                }
            }
            #endregion


            ///// Column Data Types /////
            List<ArmpType> columnDataTypes = GetColumnDataTypes(reader, table.TableInfo.ptrColumnDataTypes, table.TableInfo.ColumnCount, version);


            ///// Column Metadata /////
            #region ColumnMetadata

            List<int> columnMetadata = null;
            if (table.TableInfo.HasColumnMetadata)
            {
                columnMetadata = Util.IterateArray<int>(reader, table.TableInfo.ptrColumnMetadata, table.TableInfo.ColumnCount, true);
            }
            #endregion


            ///// Create Columns /////
            #region CreateColumns

            for (uint c = 0; c < table.TableInfo.ColumnCount; c++)
            {
                ArmpTableColumn column = new ArmpTableColumn(c, columnNames[(int)c], columnDataTypes[(int)c]);
                if (table.TableInfo.HasColumnMetadata) column.ColumnMetadata = columnMetadata[(int)c];

                table.Columns.Add(column);
            }
            #endregion


            ///// Entry Names /////
            #region EntryNames

            if (table.TableInfo.HasEntryNames)
            {
                table.EntryNames = Util.IterateStringList(reader, Util.IterateOffsetList(reader, table.TableInfo.ptrEntryNamesOffsetTable, table.TableInfo.EntryCount, true));
            }
            else
            {
                // Fill with blanks
                table.EntryNames = new List<string>();
                for (int r = 0; r < table.TableInfo.EntryCount; r++)
                {
                    table.EntryNames.Add("");
                }
            }
            #endregion


            ///// Text /////
            #region Text

            if (table.TableInfo.HasText)
            {
                List<uint> offsetList = Util.IterateOffsetList(reader, table.TableInfo.ptrTextOffsetTable, table.TableInfo.TextCount, true);
                table.Text = Util.IterateStringList(reader, offsetList);
            }
            #endregion


            ///// Entry Data /////
            InitializeEntries(table);
            ReadColumnContents(reader, table.TableInfo.ptrColumnContentOffsetTable, table);


            ///// Entry Validity /////
            #region EntryValidity

            if (table.TableInfo.HasEntryValidity)
            {
                if (version == Version.OldEngineIshin)
                {
                    List<uint> tempList = Util.IterateArray<uint>(reader, table.TableInfo.ptrEntryValidity, table.TableInfo.EntryCount, true);
                    List<bool> boolList = new List<bool>();
                    foreach (uint integer in tempList)
                    {
                        boolList.Add(Convert.ToBoolean(integer));
                    }
                    table.EntryValidity = boolList;
                    SetEntryValidity(table.EntryValidity, table.Entries);
                }
                else
                {
                    table.EntryValidity = Util.IterateBooleanBitmask(reader, table.TableInfo.ptrEntryValidity, table.TableInfo.EntryCount, true);
                    SetEntryValidity(table.EntryValidity, table.Entries);
                }
            }
            #endregion


            table.RefreshColumnNameCache();
            return table;
        }



        /// <summary>
        /// Reads an Old Engine armp table.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/>.</param>
        /// <param name="version">The format version.</param>
        /// <returns>An <see cref="ArmpTable"/> object.</returns>
        private static ArmpTable ReadTableOE(BinaryReader reader, Version version)
        {
            ArmpTableBase table = ReadTableBaseOE(reader, version);
            return new ArmpTable(table);
        }



        /// <summary>
        /// Reads the table information.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/>.</param>
        /// <param name="IsOldEngine">If the format version is from Old Engine.</param>
        /// <returns>An <see cref="ArmpTableInfo"/> object.</returns>
        private static ArmpTableInfo GetARMPTableInfo(BinaryReader reader, Version version)
        {
            ArmpTableInfo armpTableInfo = new ArmpTableInfo();
            armpTableInfo.FormatVersion = version;

            // Old Engine
            if (version == Version.OldEngine || version == Version.OldEngineIshin)
            {
                armpTableInfo.EntryCount = reader.ReadInt32(true);

                if (version == Version.OldEngineIshin) armpTableInfo.ptrEntryValidity = reader.ReadUInt32(true);
                else reader.ReadBytes(0x4);

                armpTableInfo.ptrEntryNamesOffsetTable = reader.ReadUInt32(true);

                if (version == Version.OldEngine) armpTableInfo.ptrEntryValidity = reader.ReadUInt32(true);
                else reader.ReadBytes(0x4);

                armpTableInfo.ColumnCount = reader.ReadInt32(true);
                armpTableInfo.ptrColumnNamesOffsetTable = reader.ReadUInt32(true);
                armpTableInfo.ptrColumnDataTypes = reader.ReadUInt32(true);
                armpTableInfo.ptrColumnContentOffsetTable = reader.ReadUInt32(true);
                reader.ReadBytes(0x4);
                armpTableInfo.ptrColumnMetadata = reader.ReadUInt32(true);
                armpTableInfo.ptrTextOffsetTable = reader.ReadUInt32(true);
                armpTableInfo.TextCount = reader.ReadInt32(true);

                // Set flags
                if (armpTableInfo.TextCount > 0) armpTableInfo.HasText = true;
                if (armpTableInfo.ptrEntryNamesOffsetTable > 0 && armpTableInfo.ptrEntryNamesOffsetTable < 0xFFFFFFFF) armpTableInfo.HasEntryNames = true;
                if (armpTableInfo.ptrColumnNamesOffsetTable > 0 && armpTableInfo.ptrColumnNamesOffsetTable < 0xFFFFFFFF) armpTableInfo.HasColumnNames = true;
                if (armpTableInfo.ptrEntryValidity > 0 && armpTableInfo.ptrEntryValidity < 0xFFFFFFFF) armpTableInfo.HasEntryValidity = true;
                if (armpTableInfo.ptrColumnMetadata > 0 && armpTableInfo.ptrColumnMetadata < 0xFFFFFFFF) armpTableInfo.HasColumnMetadata = true;

#if DEBUG
                Console.WriteLine("Entry Count: " + armpTableInfo.EntryCount);
                Console.WriteLine("Column Count: " + armpTableInfo.ColumnCount);
                Console.WriteLine("Text Count: " + armpTableInfo.TextCount);
                Console.WriteLine("Pointer to Entry Names Offset Table: " + armpTableInfo.ptrEntryNamesOffsetTable);
                Console.WriteLine("Pointer to Entry Validity: " + armpTableInfo.ptrEntryValidity);
                Console.WriteLine("Pointer to Column Data Types: " + armpTableInfo.ptrColumnDataTypes);
                Console.WriteLine("Pointer to Column Content Offset Table: " + armpTableInfo.ptrColumnContentOffsetTable);
                Console.WriteLine("Pointer to Text Offset Table: " + armpTableInfo.ptrTextOffsetTable);
                Console.WriteLine("Pointer to Column Names Offset Table: " + armpTableInfo.ptrColumnNamesOffsetTable);
                Console.WriteLine("Pointer to Column Metadata: " + armpTableInfo.ptrColumnMetadata);
#endif
            }

            //Dragon Engine
            else
            {
                armpTableInfo.ptrMainTable = (uint)reader.BaseStream.Position; //DEBUG
                armpTableInfo.EntryCount = reader.ReadInt32();
                armpTableInfo.ColumnCount = reader.ReadInt32();
                armpTableInfo.TextCount = reader.ReadInt32();
                armpTableInfo.DefaultEntryID = reader.ReadInt32();
                armpTableInfo.ptrEntryNamesOffsetTable = reader.ReadUInt32();
                armpTableInfo.ptrEntryValidity = reader.ReadUInt32();
                armpTableInfo.ptrColumnDataTypes = reader.ReadUInt32();
                armpTableInfo.ptrColumnContentOffsetTable = reader.ReadUInt32();

                if (version == Version.DragonEngineV2)
                {
                    armpTableInfo.TableID = reader.ReadInt24();
                    byte tableFlags = reader.ReadByte();

                    //Set flags
                    armpTableInfo.StorageMode  = (StorageMode)(tableFlags & (1 << 0));
                    armpTableInfo.UnknownFlag1 = (tableFlags & (1 << 1)) != 0;
                    armpTableInfo.UnknownFlag2 = (tableFlags & (1 << 2)) != 0;
                    armpTableInfo.UnknownFlag3 = (tableFlags & (1 << 3)) != 0;
                    armpTableInfo.UnknownFlag4 = (tableFlags & (1 << 4)) != 0;
                    armpTableInfo.DoNotUseRaw  = (tableFlags & (1 << 5)) != 0;
                    armpTableInfo.MembersWellFormatted = (tableFlags & (1 << 6)) != 0;
                    armpTableInfo.IsProcessedForMemory = (tableFlags & (1 << 7)) != 0;
                }
                else
                {
                    armpTableInfo.ptrColumnMetadata = reader.ReadUInt32();
                }

                armpTableInfo.ptrTextOffsetTable = reader.ReadUInt32();
                armpTableInfo.ptrColumnNamesOffsetTable = reader.ReadUInt32();
                armpTableInfo.DefaultColumnID = reader.ReadInt32();
                armpTableInfo.ptrEntryOrder = reader.ReadUInt32();
                armpTableInfo.ptrColumnOrder = reader.ReadUInt32();
                armpTableInfo.ptrColumnValidity = reader.ReadUInt32();
                armpTableInfo.ptrIndexerTable = reader.ReadUInt32();
                armpTableInfo.ptrGameVarColumnIDs = reader.ReadUInt32();
                armpTableInfo.ptrBlankCellFlagOffsetTable = reader.ReadUInt32();
                armpTableInfo.ptrMemberInfo = reader.ReadUInt32();
                armpTableInfo.ptrExtraFieldInfo = reader.ReadUInt32();

                if (armpTableInfo.ptrColumnMetadata > 0 && armpTableInfo.ptrColumnMetadata < 0xFFFFFFFF) armpTableInfo.HasColumnMetadata = true;
                if (armpTableInfo.TextCount > 0) armpTableInfo.HasText = true;
                if (armpTableInfo.ptrIndexerTable > 0 && armpTableInfo.ptrIndexerTable < 0xFFFFFFFF) armpTableInfo.HasIndexerTable = true;
                if (armpTableInfo.ptrEntryNamesOffsetTable > 0 && armpTableInfo.ptrEntryNamesOffsetTable < 0xFFFFFFFF) armpTableInfo.HasEntryNames = true;
                if (armpTableInfo.ptrColumnNamesOffsetTable > 0 && armpTableInfo.ptrColumnNamesOffsetTable < 0xFFFFFFFF) armpTableInfo.HasColumnNames = true;
                if (armpTableInfo.ptrMemberInfo > 0 && armpTableInfo.ptrMemberInfo < 0xFFFFFFFF) armpTableInfo.HasMemberInfo = true;
                if (armpTableInfo.ptrEntryOrder > 0 && armpTableInfo.ptrEntryOrder < 0xFFFFFFFF) armpTableInfo.HasOrderedEntries = true;
                if (armpTableInfo.ptrColumnOrder > 0 && armpTableInfo.ptrColumnOrder < 0xFFFFFFFF) armpTableInfo.HasOrderedColumns = true;
                if (armpTableInfo.ptrBlankCellFlagOffsetTable > 0 && armpTableInfo.ptrBlankCellFlagOffsetTable < 0xFFFFFFFF) armpTableInfo.HasBlankCellFlags = true;
                if (armpTableInfo.ptrGameVarColumnIDs > 0 && armpTableInfo.ptrGameVarColumnIDs < 0xFFFFFFFF) armpTableInfo.HasGameVarColumns = true;
                if (armpTableInfo.ptrExtraFieldInfo > 0 && armpTableInfo.ptrExtraFieldInfo < 0xFFFFFFFF) armpTableInfo.HasExtraFieldInfo = true;


#if DEBUG
                Console.WriteLine("Entry Count: " + armpTableInfo.EntryCount);
                Console.WriteLine("Column Count: " + armpTableInfo.ColumnCount);
                Console.WriteLine("Text Count: " + armpTableInfo.TextCount);
                Console.WriteLine("Default Entry ID: " + armpTableInfo.DefaultEntryID);
                Console.WriteLine("Pointer to Entry Names Offset Table: " + armpTableInfo.ptrEntryNamesOffsetTable);
                Console.WriteLine("Pointer to Entry Validity: " + armpTableInfo.ptrEntryValidity);
                Console.WriteLine("Pointer to Column Data Types: " + armpTableInfo.ptrColumnDataTypes);
                Console.WriteLine("Pointer to Column Content Offset Table: " + armpTableInfo.ptrColumnContentOffsetTable);
                Console.WriteLine("Table ID: " + armpTableInfo.TableID);
                Console.WriteLine("Storage Mode: " + armpTableInfo.StorageMode);
                Console.WriteLine("Unknown Flag 1: " + armpTableInfo.UnknownFlag1);
                Console.WriteLine("Unknown Flag 2: " + armpTableInfo.UnknownFlag2);
                Console.WriteLine("Unknown Flag 3: " + armpTableInfo.UnknownFlag3);
                Console.WriteLine("Unknown Flag 4: " + armpTableInfo.UnknownFlag4);
                Console.WriteLine("Flag 5 - DoNotUseRaw: " + armpTableInfo.DoNotUseRaw);
                Console.WriteLine("Flag 6 - MembersWellFormatted: " + armpTableInfo.MembersWellFormatted);
                Console.WriteLine("Flag 7 - IsProcessedForMemory: " + armpTableInfo.IsProcessedForMemory);
                Console.WriteLine("Pointer to Text Offset Table: " + armpTableInfo.ptrTextOffsetTable);
                Console.WriteLine("Pointer to Column Names Offset Table: " + armpTableInfo.ptrColumnNamesOffsetTable);
                Console.WriteLine("Default Column ID: " + armpTableInfo.DefaultColumnID);
                Console.WriteLine("Pointer to Entry Display Order: " + armpTableInfo.ptrEntryOrder);
                Console.WriteLine("Pointer to Column Display Order: " + armpTableInfo.ptrColumnOrder);
                Console.WriteLine("Pointer to Column Validity: " + armpTableInfo.ptrColumnValidity);
                Console.WriteLine("Pointer to Indexer table: " + armpTableInfo.ptrIndexerTable);
                Console.WriteLine("Pointer to Game Var Column IDs: " + armpTableInfo.ptrColumnMetadata);
                Console.WriteLine("Pointer to Blank Cell Flag Offset Table: " + armpTableInfo.ptrBlankCellFlagOffsetTable);
                Console.WriteLine("Pointer to Member Info: " + armpTableInfo.ptrMemberInfo);
                Console.WriteLine("Pointer to Array Info: " + armpTableInfo.ptrExtraFieldInfo);
                Console.WriteLine("Has Indexer: " + armpTableInfo.HasIndexerTable);
#endif
            }

            return armpTableInfo;
        }



        /// <summary>
        /// Reads the column data types.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/>.</param>
        /// <param name="ptrDataTypes">The pointer to the column data type array.</param>
        /// <param name="amount">The amount of values in the array.</param>
        /// <param name="version">The format version.</param>
        /// <param name="IsMemberType">Is it the member type array?</param>
        /// <returns>An <see cref="ArmpType"/> list.</returns>
        private static List<ArmpType> GetColumnDataTypes(BinaryReader reader, uint ptrDataTypes, int amount, Version version, bool IsMemberType = false)
        {
            List<ArmpType> returnList = new List<ArmpType>();

            Dictionary<sbyte, ArmpType> typeIdCache = new Dictionary<sbyte, ArmpType>();
            foreach (ArmpType type in DataTypes.Types)
            {
                typeIdCache.TryAdd(type.GetID(version, IsMemberType), type);
            }

            reader.BaseStream.Seek(ptrDataTypes);

            // OE type IDs are stored as int32
            if (version == Version.OldEngine || version == Version.OldEngineIshin)
            {
                for (int i = 0; i < amount; i++)
                {
                    sbyte id = (sbyte)reader.ReadInt32(true);
                    if (typeIdCache.ContainsKey(id))
                        returnList.Add(typeIdCache[id]);
                }
            }
            else
            {
                for (int i = 0; i < amount; i++)
                {
                    sbyte id = reader.ReadSByte();
                    if (typeIdCache.ContainsKey(id))
                        returnList.Add(typeIdCache[id]);
                }
            }

            return returnList;
        }



        /// <summary>
        /// Initializes the entries of a table.
        /// </summary>
        /// <param name="table">The <see cref="ArmpTableBase"/>.</param>
        private static void InitializeEntries(ArmpTableBase table)
        {
            ArmpEntry entry;
            for (uint i = 0; i < table.TableInfo.EntryCount; i++)
            {
                if (!table.TableInfo.HasOrderedEntries)
                    entry = new ArmpEntry(table, i, table.EntryNames[(int)i], i);
                else
                    entry = new ArmpEntry(table, i, table.EntryNames[(int)i], (uint)table.OrderedEntryIDs.IndexOf(i));

                entry.ParentTable = table;
                table.Entries.Add(entry);
            }
        }



        /// <summary>
        /// Reads the column values for each entry.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/>.</param>
        /// <param name="ptrOffsetTable">The pointer to the offset table.</param>
        /// <param name="storageMode">Storage mode used.</param>
        /// <param name="version">The format version.</param>
        /// <param name="table">The table where the data will be added to.</param>
        /// <remarks><para><b>DRAGON ENGINE ONLY</b></para></remarks>
        private static void ReadColumnContents(BinaryReader reader, uint ptrOffsetTable, StorageMode storageMode, Version version, ArmpTableBase table)
        {
            reader.BaseStream.Seek(ptrOffsetTable);

            if (storageMode == StorageMode.Column)
            {
                foreach (ArmpTableColumn column in table.Columns)
                {
                    int ptrData = reader.ReadInt32();
                    long nextPtr = reader.BaseStream.Position;
                    if (column.Type.CSType == typeof(bool) && ptrData == -1 || ptrData == 0)
                    {
                        bool value = ptrData == -1;
                        foreach (ArmpEntry entry in table.Entries)
                        {
                            entry.Data.Add(column.Name, value);
                        }
#if DEBUG
                        Console.WriteLine(String.Format("Bool shortcut column ---> {0}", column.Name));
#endif
                    }
                    else if (ptrData > 0)
                    {
                        reader.BaseStream.Seek(ptrData);
#if DEBUG
                        Console.WriteLine(column.Name + " ----> " + column.Type);
#endif
                        // Only StorageMode.Column
                        List<bool> booleanColumnDataTemp = new List<bool>();
                        if (column.Type.CSType == typeof(bool))
                        {
                            booleanColumnDataTemp = Util.IterateBooleanBitmask(reader, (uint)reader.BaseStream.Position, table.TableInfo.EntryCount, false);
                        }

                        foreach (ArmpEntry entry in table.Entries)
                        {
                            ReadValue(reader, table, version, entry, column, column.Type, booleanColumnDataTemp);
                        }

                    }

                    reader.BaseStream.Seek(nextPtr);
                }
            }


            else if (storageMode == StorageMode.Structured)
            {
                foreach (ArmpEntry entry in table.Entries)
                {
                    int ptrData = reader.ReadInt32();
                    long nextPtr = reader.BaseStream.Position;

                    foreach (ArmpMemberInfo memberInfo in table.StructureSpec)
                    {
                        if (!memberInfo.Column.IsValid || memberInfo.Type.IsArray || memberInfo.Position < 0) continue;
                        reader.BaseStream.Seek(ptrData + memberInfo.Position);
                        ReadValue(reader, table, version, entry, memberInfo.Column, memberInfo.Type);
                    }

                    reader.BaseStream.Seek(nextPtr);
                }
            }
        }



        /// <summary>
        /// Reads a value corresponding to an entry and column.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/>.</param>
        /// <param name="table"><see cref="ArmpTableBase"/> to save the data to.</param>
        /// <param name="version">The format version.</param>
        /// <param name="entry">The entry.</param>
        /// <param name="column">The column.</param>
        /// <param name="booleanColumnDataTemp">(Optional) The boolean column data if its using storage mode 0.</param>
        private static void ReadValue(BinaryReader reader, ArmpTableBase table, Version version, ArmpEntry entry, ArmpTableColumn column, ArmpType type, List<bool> booleanColumnDataTemp = null)
        {
            if (type.CSType == null) // Invalid
            {
                entry.Data.Add(column.Name, null);
            }

            else if (type.CSType == typeof(bool))
            {
                if (booleanColumnDataTemp != null)
                {
                    bool value = booleanColumnDataTemp[(int)entry.ID];
                    entry.Data.Add(column.Name, value);
                }
                else
                {
                    byte value = reader.ReadByte();
                    entry.Data.Add(column.Name, value != 0);
                }
            }

            else if (type.CSType == typeof(string))
            {
                // This should only run if we are loading the armp from memory
                if (table.TableInfo.IsProcessedForMemory)
                {
                    long ptrText = reader.ReadInt64();
                    ptrText = ptrText - table.TableInfo.BaseARMPMemoryAddress;
                    reader.BaseStream.PushToPosition(ptrText);
                    string text = reader.ReadString();
                    reader.BaseStream.PopPosition();
                    entry.Data.Add(column.Name, text);
                }
                else
                {
                    int index = reader.ReadInt32();
                    if (index >= 0 && table.TableInfo.HasText) // Some files have valid string indices despite not having any text.
                        entry.Data.Add(column.Name, table.Text[index]);
                    else
                        entry.Data.Add(column.Name, null);  // Will translate to either -1 or 0 depending on text count.
                }
            }

            else if (type.CSType == typeof(ArmpTable))
            {
                long ptrTable = reader.ReadInt64();
                long currentpos = reader.BaseStream.Position;
                if (ptrTable == 0 || ptrTable == -1) return;

                // This should only run if we are loading the armp from memory
                if (table.TableInfo.IsProcessedForMemory)
                {
                    ptrTable = ptrTable - table.TableInfo.BaseARMPMemoryAddress;
                }

                ArmpTable tableValue = ReadTable(reader, ptrTable, version, table.TableInfo.BaseARMPMemoryAddress);

                entry.Data.Add(column.Name, tableValue);
                reader.BaseStream.Position = currentpos; // Reset position to the offset table
            }

            else if (type.CSType == typeof(float))
            {
                float value = reader.ReadSingle();
                entry.Data[column.Name] = value;
            }

            else if (type.CSType == typeof(double))
            {
                double value = reader.ReadDouble();
                entry.Data[column.Name] = value;
            }

            else if (type.CSType == typeof(byte))
            {
                byte value = reader.ReadByte();
                entry.Data[column.Name] = value;
            }

            else if (type.CSType == typeof(sbyte))
            {
                sbyte value = reader.ReadSByte();
                entry.Data[column.Name] = value;
            }

            else if (type.CSType == typeof(UInt16))
            {
                UInt16 value = reader.ReadUInt16();
                entry.Data[column.Name] = value;
            }

            else if (type.CSType == typeof(Int16))
            {
                Int16 value = reader.ReadInt16();
                entry.Data[column.Name] = value;
            }

            else if (type.CSType == typeof(UInt32))
            {
                UInt32 value = reader.ReadUInt32();
                entry.Data[column.Name] = value;
            }

            else if (type.CSType == typeof(Int32))
            {
                Int32 value = reader.ReadInt32();
                entry.Data[column.Name] = value;
            }

            else if (type.CSType == typeof(UInt64))
            {
                UInt64 value = reader.ReadUInt64();
                entry.Data[column.Name] = value;
            }

            else if (type.CSType == typeof(Int64))
            {
                Int64 value = reader.ReadInt64();
                entry.Data[column.Name] = value;
            }
        }



        /// <summary>
        /// Reads the column values for each entry.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/>.</param>
        /// <param name="ptrOffsetTable">The pointer to the offset table.</param>
        /// <param name="table">The table where the data will be added to.</param>
        /// <remarks><para><b>OLD ENGINE ONLY</b></para></remarks>
        private static void ReadColumnContents(BinaryReader reader, uint ptrOffsetTable, ArmpTableBase table)
        {
            reader.BaseStream.Seek(ptrOffsetTable);

            foreach (ArmpTableColumn column in table.Columns)
            {
                uint ptrData = reader.ReadUInt32(true);
                long nextPtr = reader.BaseStream.Position;
                reader.BaseStream.Seek(ptrData);


                // Read operations based on column type
                if (column.Type.CSType == null)
                {
                    foreach (ArmpEntry entry in table.Entries)
                        entry.Data.Add(column.Name, null);
                }

                else if (column.Type.CSType == typeof(bool))
                {
                    List<bool> booleanColumnDataTemp = new List<bool>();
                    booleanColumnDataTemp = Util.IterateBooleanBitmask(reader, (uint)reader.BaseStream.Position, table.TableInfo.EntryCount, true);
                    foreach (ArmpEntry entry in table.Entries)
                        entry.Data.Add(column.Name, booleanColumnDataTemp[(int)entry.ID]);
                }

                else if (column.Type.CSType == typeof(byte))
                {
                    foreach (ArmpEntry entry in table.Entries)
                    {
                        byte value = reader.ReadByte();
                        entry.Data.Add(column.Name, value);
                    }
                }

                else if (column.Type.CSType == typeof(sbyte))
                {
                    foreach (ArmpEntry entry in table.Entries)
                    {
                        sbyte value = reader.ReadSByte();
                        entry.Data.Add(column.Name, value);
                    }
                }

                else if (column.Type.CSType == typeof(UInt16))
                {
                    foreach (ArmpEntry entry in table.Entries)
                    {
                        UInt16 value = reader.ReadUInt16(true);
                        entry.Data.Add(column.Name, value);
                    }
                }

                else if (column.Type.CSType == typeof(Int16))
                {
                    foreach (ArmpEntry entry in table.Entries)
                    {
                        Int16 value = reader.ReadInt16(true);
                        entry.Data.Add(column.Name, value);
                    }
                }

                else if (column.Type.CSType == typeof(UInt32))
                {
                    foreach (ArmpEntry entry in table.Entries)
                    {
                        UInt32 value = reader.ReadUInt32(true);
                        entry.Data.Add(column.Name, value);
                    }
                }

                else if (column.Type.CSType == typeof(Int32))
                {
                    foreach (ArmpEntry entry in table.Entries)
                    {
                        Int32 value = reader.ReadInt32(true);
                        entry.Data.Add(column.Name, value);
                    }
                }

                reader.BaseStream.Seek(nextPtr);
            }
        }



        /// <summary>
        /// Sets the "Valid" flag for a list of entries.
        /// </summary>
        /// <param name="entryValidity">The validity list.</param>
        /// <param name="entries">The entry list to update.</param>
        private static void SetEntryValidity(List<bool> entryValidity, List<ArmpEntry> entries)
        {
            int iter = 0;
            foreach (bool validity in entryValidity)
            {
                entries[iter].IsValid = validity;
                iter++;
            }
        }



        /// <summary>
        /// Reads the Entry Info Flags. 
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/>.</param>
        /// <param name="ptrArray">The pointer to the array.</param>
        /// <param name="entries">The entry list.</param>
        /// <remarks><para><b>DRAGON ENGINE V1 ONLY</b></para></remarks>
        private static void IterateEntryInfoFlags(BinaryReader reader, uint ptrArray, List<ArmpEntry> entries)
        {
            reader.BaseStream.Seek(ptrArray);

            for (int i = 0; i < entries.Count; i++)
            {
                byte b = reader.ReadByte();
                var bitstring = Convert.ToString(b, 2).PadLeft(8, '0');
                bitstring = Util.ReverseString(bitstring);

                ArmpEntry entry = entries[i];
                entry.Flags = new bool[8];
                int iter = 0;
                foreach (char c in bitstring)
                {
                    bool boolvalue = false;
                    if (c == '1') boolvalue = true;
                    entry.Flags[iter] = boolvalue;
                    iter++;
                }
            }
        }



        /// <summary>
        /// Reads the Array Info table.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/>.</param>
        /// <param name="ptrArrayInfo">The pointer to the Array Info section.</param>
        /// <param name="table">The table.</param>
        /// <remarks><para><b>DRAGON ENGINE V2 ONLY</b></para></remarks>
        private static void ReadArrayInfoTable(BinaryReader reader, uint ptrArrayInfo, ArmpTableBase table)
        {
            reader.BaseStream.Seek(ptrArrayInfo);

            foreach (ArmpTableColumn column in table.Columns)
            {
                int arraySize = reader.ReadInt32();
                int ptrArrayIndices = reader.ReadInt32();
                reader.ReadBytes(0x18); // Padding

                if (arraySize > 0 && ptrArrayIndices > 0)
                {
                    column.Children = new List<ArmpTableColumn>(arraySize);
                    reader.BaseStream.PushToPosition(ptrArrayIndices);

                    int index;
                    for (int j = 0; j < arraySize; j++)
                    {
                        index = reader.ReadInt32();
                        if (index > 0)
                        {
                            ArmpTableColumn child = table.Columns[index];
                            column.Children.Add(child);
                            child.Parent = column;
                        }
                        else
                            column.Children.Add(null);
                    }

                    reader.BaseStream.PopPosition();
                }
            }
        }



        /// <summary>
        /// Reads the Member Info table.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/>.</param>
        /// <param name="ptrTable">The pointer to the auxiliary table.</param>
        /// <param name="columnAmount">The amount of columns in the table.</param>
        private static List<ArmpMemberInfo> ReadMemberInfoTable(BinaryReader reader, uint ptrTable, int columnAmount)
        {
            reader.BaseStream.Seek(ptrTable);
            List<ArmpMemberInfo> MemberInfoTable = new List<ArmpMemberInfo>(columnAmount);

            int type;
            for (int i = 0; i < columnAmount; i++)
            {
                ArmpMemberInfo memberInfo = new ArmpMemberInfo();
                type = reader.ReadInt32();
                foreach (ArmpType armpType in DataTypes.Types)
                {
                    if (armpType.GetMemberTypeID(Version.DragonEngineV2) == type)
                {
                        memberInfo.Type = armpType;
                        break;
                    }
                }
                memberInfo.Position = reader.ReadInt32();
                memberInfo.ArraySize = reader.ReadUInt32();
                reader.ReadInt32(); // Reserved

                MemberInfoTable.Add(memberInfo);
            }

            return MemberInfoTable;
        }



        /// <summary>
        /// Generates an auxiliary type list from the IDs in the aux table.
        /// </summary>
        /// <param name="memberInfoTable">The table of member info entries.</param>
        /// <param name="version">The format version.</param>
        /// <returns>A <see cref="ArmpType"/> list.</returns>
        /// <remarks><para><b>DRAGON ENGINE V2 ONLY</b></para></remarks>
        private static List<ArmpType> ParseMemberInfoTable(List<int[]> memberInfoTable, Version version)
        {
            List<ArmpType> typesList = new List<ArmpType>();

            for (int i = 0; i < memberInfoTable.Count; i++)
            {
                sbyte id = Convert.ToSByte(memberInfoTable[i][0]);
                foreach (ArmpType armpType in DataTypes.Types)
                {
                    if (armpType.GetMemberTypeID(version) == id)
                    {
                        typesList.Add(armpType);
                        break;
                    }
                }
            }

            return typesList;
        }
    }
}
