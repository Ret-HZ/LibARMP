﻿using LibARMP.Exceptions;
using System;
using System.Collections.Generic;

namespace LibARMP
{
    [Serializable]
    public class ArmpTable
    {

        internal Dictionary<string, ArmpTableColumn> ColumnNameCache { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmpTable"/> class.
        /// </summary>
        internal ArmpTable()
        {
            ColumnNameCache = new Dictionary<string, ArmpTableColumn>();
            Entries = new List<ArmpEntry>();
            Columns = new List<ArmpTableColumn>();
            EmptyValues = new Dictionary<int, List<bool>>();
            EmptyValuesIsNegativeOffset = new List<bool>();
        }

        /// <summary>
        /// Flags and general information.
        /// </summary>
        public ArmpTableInfo TableInfo { get; internal set; }

        /// <summary>
        /// Entry names.
        /// </summary>
        internal List<string> EntryNames { get; set; }

        /// <summary>
        /// Text.
        /// </summary>
        internal List<string> Text { get; set; }

        /// <summary>
        /// Entry validity. 
        /// </summary>
        internal List<bool> EntryValidity { get; set; }

        /// <summary>
        /// Entry indices.
        /// </summary>
        internal List<int> EntryIndices { get; set; }

        /// <summary>
        /// Columns.
        /// </summary>
        internal List<ArmpTableColumn> Columns { get; set; }

        /// <summary>
        /// List of entries.
        /// </summary>
        internal List<ArmpEntry> Entries { get; set; }

        /// <summary>
        /// Values marked as empty for specific columns (despite actually having a value).
        /// </summary>
        /// <remarks><para>[column index : list<bool> (length = entry count)]</para></remarks>
        internal Dictionary<int, List<bool>> EmptyValues { get; set; }

        /// <summary>
        /// DEBUG: Boolean list (length = column count) to indicate if the offset in the empty values offset list was -1.
        /// </summary>
        /// <remarks><para>The difference between 0 and -1 is unknown.</para></remarks>
        internal List<bool> EmptyValuesIsNegativeOffset { get; set; }

        /// <summary>
        /// PLACEHOLDER: Edited values for patcher.
        /// </summary>
        /// <remarks><para>[column : list of entry ids]</para></remarks>
        internal Dictionary<string, List<uint>> EditedValues = new Dictionary<string, List<uint>>();



        /// <summary>
        /// Creates a copy of this table.
        /// </summary>
        /// <param name="copyEntries">Should entries be copied? Default value is <see langword="true"/>.</param>
        /// <returns>A copy of this <see cref="ArmpTable"/>.</returns>
        public ArmpTable Copy (bool copyEntries = true)
        {
            ArmpTable copy = new ArmpTable();
            copy.TableInfo = Util.DeepCopy(TableInfo);

            foreach (ArmpTableColumn column in Columns)
            {
                ArmpTableColumn copyColumn = column.Copy();
                copy.Columns.Add(copyColumn);

                if (copyColumn.Name.Contains("[") && copyColumn.Name.Contains("]"))
                {
                    string baseName = copyColumn.Name.Split('[')[0];
                    if (copy.ColumnNameCache.ContainsKey(baseName))
                    {
                        ArmpTableColumn parentColumn = copy.ColumnNameCache[baseName];
                        parentColumn.Children.Add(copyColumn);
                        parentColumn.SpecialSize += 1;
                        copyColumn.Parent = parentColumn;
                    }
                }

                copy.RefreshColumnNameCache();
            }

            if (copyEntries)
            {
                foreach(ArmpEntry entry in Entries)
                {
                    copy.Entries.Add(entry.Copy(copy));
                }
            }

            return copy;
        }


        /// <summary>
        /// Refreshes the Column Name Cache.
        /// </summary>
        internal void RefreshColumnNameCache()
        {
            ColumnNameCache.Clear();
            foreach (ArmpTableColumn column in Columns)
            {
                ColumnNameCache.Add(column.Name, column);
            }
        }


        /// <summary>
        /// Returns all entries in the table.
        /// </summary>
        /// <returns>A read-only <see cref="ArmpEntry"/> list.</returns>
        /// <exception cref="EntryNotFoundException">The table has no entries.</exception>
        public IReadOnlyList<ArmpEntry> GetAllEntries()
        {
            try
            {
                return Entries.AsReadOnly();
            }
            catch
            {
                throw new EntryNotFoundException();
            }
        }


        /// <summary>
        /// Returns a specific entry in the table.
        /// </summary>
        /// <param name="id">The entry ID.</param>
        /// <returns>An <see cref="ArmpEntry"/>.</returns>
        /// <exception cref="EntryNotFoundException">The table has no entry with the specified ID.</exception>
        public ArmpEntry GetEntry (uint id)
        {
            try
            {
                return Entries[(int)id];
            }
            catch
            {
                throw new EntryNotFoundException(id);
            }
        }


        /// <summary>
        /// Returns a specific entry in the table.
        /// </summary>
        /// <param name="name">The entry name.</param>
        /// <returns>An <see cref="ArmpEntry"/>.</returns>
        /// <exception cref="EntryNotFoundException">The table has no entry with the specified name.</exception>
        public ArmpEntry GetEntry (string name)
        {
            try
            {
                foreach (ArmpEntry entry in Entries)
                {
                    if (entry.Name == name) return entry;
                }
                throw new EntryNotFoundException(name);
            }
            catch
            {
                throw new EntryNotFoundException(name);
            }
        }


        /// <summary>
        /// Returns a specific entry in the table.
        /// </summary>
        /// <param name="id">The ID of the entry to get.</param>
        /// <param name="entry">When this method returns, contains the entry associated with the specified ID, if the entry is found; otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if an entry with the specified ID exists; otherwise <see langword="false"/>.</returns>
        public bool TryGetEntry (uint id, out ArmpEntry entry)
        {
            if (Entries.Count > id)
            {
                entry = Entries[(int)id];
                return true;
            }
            else
            {
                entry = null;
                return false;
            }
        }


        /// <summary>
        /// Returns a specific entry in the table.
        /// </summary>
        /// <param name="name">The name of the entry to get.</param>
        /// <param name="entry">When this method returns, contains the entry associated with the specified name, if the entry is found; otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if an entry with the specified name exists; otherwise <see langword="false"/>.</returns>
        public bool TryGetEntry (string name, out ArmpEntry entry)
        {
            foreach (ArmpEntry ae in Entries)
            {
                if (ae.Name == name)
                {
                    entry = ae;
                    return true;
                }
            }

            entry = null;
            return false;
        }


        public bool SetEntry (ArmpEntry entry)
        {
            if (entry == null) throw new ArgumentNullException("Entry cannot be null.");

            //TODO need to check that the entry columns and data match the current table
            Entries[(int)entry.ID] = Util.DeepCopy<ArmpEntry>(entry);
            return true; //PLACEHOLDER
        }


        /// <summary>
        /// Gets all entry names.
        /// </summary>
        /// <returns>A <see cref="string"/> list.</returns>
        /// <exception cref="EntryNameNotFoundException">The table has no entry names.</exception>
        public List<string> GetEntryNames()
        {
            if (!TableInfo.HasEntryNames)
                throw new EntryNameNotFoundException();

            List<string> returnList = new List<string>();
            foreach (ArmpEntry entry in Entries)
            {
                returnList.Add(entry.Name);
            }

            return returnList;
        }


        /// <summary>
        /// Gets the name of a specific entry.
        /// </summary>
        /// <param name="id">The entry ID.</param>
        /// <returns>A <see cref="string"/>.</returns>
        /// <exception cref="EntryNameNotFoundException">The table has no entry names.</exception>
        /// <exception cref="EntryNotFoundException">The table has no entry with the specified ID.</exception>
        public string GetEntryName (uint id)
        {
            if (!TableInfo.HasEntryNames)
                throw new EntryNameNotFoundException();

            try
            {
                return Entries[(int)id].Name;
            }
            catch
            {
                throw new EntryNotFoundException(id);
            }
        }


        /// <summary>
        /// Returns all columns in the table.
        /// </summary>
        /// <returns>A read-only <see cref="ArmpTableColumn"/> list.</returns>
        /// <exception cref="ColumnNotFoundException">The table has no columns.</exception>
        public IReadOnlyList<ArmpTableColumn> GetAllColumns()
        {
            try
            {
                return Columns.AsReadOnly();
            }
            catch
            {
                throw new ColumnNotFoundException();
            }
        }


        /// <summary>
        /// Gets a specific column in the table.
        /// </summary>
        /// <param name="id">The column ID.</param>
        /// <returns>An <see cref="ArmpTableColumn"/>.</returns>
        /// <exception cref="ColumnNotFoundException">The table has no column with the specified ID.</exception>
        public ArmpTableColumn GetColumn (uint id)
        {
            try
            {
                return Columns[(int)id];
            }
            catch
            {
                throw new ColumnNotFoundException(id);
            }
        }


        /// <summary>
        /// Gets a specific column in the table.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <returns>An <see cref="ArmpTableColumn"/>.</returns>
        /// <exception cref="ColumnNotFoundException">The table has no column with the specified name.</exception>
        public ArmpTableColumn GetColumn (string columnName)
        {
            try
            {
                return ColumnNameCache[columnName];
            }
            catch
            {
                throw new ColumnNotFoundException(columnName);
            }
        }


        /// <summary>
        /// Returns a specific column in the table.
        /// </summary>
        /// <param name="name">The ID of the column to get.</param>
        /// <param name="entry">When this method returns, contains the column associated with the specified ID, if the column is found; otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if a column with the specified ID exists; otherwise <see langword="false"/>.</returns>
        public bool TryGetColumn (uint id, out ArmpTableColumn column)
        {
            if (Columns.Count > id)
            {
                column = Columns[(int)id];
                return true;
            }
            else
            {
                column = null;
                return false;
            }
        }


        /// <summary>
        /// Returns a specific column in the table.
        /// </summary>
        /// <param name="name">The name of the column to get.</param>
        /// <param name="entry">When this method returns, contains the column associated with the specified name, if the column is found; otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if a column with the specified name exists; otherwise <see langword="false"/>.</returns>
        public bool TryGetColumn (string columnName, out ArmpTableColumn column)
        {
            return ColumnNameCache.TryGetValue(columnName, out column);
        }


        /// <summary>
        /// Gets the column names.
        /// </summary>
        /// <param name="includeSpecials">Include special columns? (Array data types. These columns do not contain data)</param>
        /// <returns>A <see cref="string"/> list.</returns>
        public List<string> GetColumnNames (bool includeSpecials = true)
        {
            List<string> returnList = new List<string>();

            foreach (ArmpTableColumn column in Columns)
            {
                if (column.IsSpecial && !includeSpecials) continue;
                returnList.Add(column.Name);
            }

            return returnList;
        }


        /// <summary>
        /// Gets the column name.
        /// </summary>
        /// <param name="id">The column ID.</param>
        /// <returns>A <see cref="string"/>.</returns>
        /// <exception cref="ColumnNotFoundException">The table has no column with the specified ID.</exception>
        public string GetColumnName (uint id)
        {
            try
            {
                return Columns[(int)id].Name;
            }
            catch
            {
                throw new ColumnNotFoundException(id);
            }
        }


        /// <summary>
        /// Gets the column's data type.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <returns>The column <see cref="Type"/>.</returns>
        /// <exception cref="ColumnNotFoundException">The table has no column with the specified name.</exception>
        public Type GetColumnDataType (string columnName)
        {
            if (ColumnNameCache.ContainsKey(columnName))
            {
                return ColumnNameCache[columnName].Type.CSType;
            }
            throw new ColumnNotFoundException(columnName);   
        }


        /// <summary>
        /// Gets a list of column names matching the type.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to look for.</param>
        /// <returns>A <see cref="string"/> list.</returns>
        public List<string> GetColumnNamesByType (Type type)
        {
            List<string> returnList = new List<string>();
            foreach (ArmpTableColumn column in Columns)
            {
                if (column.Type.CSType == type)
                    returnList.Add(column.Name);
            }

            return returnList;
        }


        /// <summary>
        /// Gets a list of columns matching the type.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to look for.</param>
        /// <returns>An <see cref="ArmpTableColumn"/> list.</returns>
        public List<ArmpTableColumn> GetColumnsByType (Type type)
        {
            List<ArmpTableColumn> returnList = new List<ArmpTableColumn>();
            foreach (ArmpTableColumn column in Columns)
            {
                if (column.Type.CSType == type)
                    returnList.Add(column);
            }

            return returnList;
        }


        /// <summary>
        /// Gets a list of columns matching the type.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to look for.</typeparam>
        /// <returns>An <see cref="ArmpTableColumn"/> list.</returns>
        public List<ArmpTableColumn> GetColumnsByType<T>()
        {
            Type type = typeof(T);
            return GetColumnsByType(type);
        }


        /// <summary>
        /// Gets a list of column names matching the type.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to look for.</typeparam>
        /// <returns>A <see cref="string"/> list.</returns>
        public List<string> GetColumnNamesByType<T>()
        {
            Type type = typeof(T);
            return GetColumnNamesByType(type);
        }


        /// <summary>
        /// Gets a list of column IDs matching the type.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to look for.</param>
        /// <returns>A <see cref="uint"/> list.</returns>
        public List<uint> GetColumnIDsByType (Type type)
        {
            List<uint> returnList = new List<uint>();
            foreach (ArmpTableColumn column in Columns)
            {
                if (column.Type.CSType == type) returnList.Add(column.ID);
            }

            return returnList;
        }


        /// <summary>
        /// Gets a list of column IDs matching the type.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to look for.</typeparam>
        /// <returns>An <see cref="uint"/> list.</returns>
        public List<uint> GetColumnIDsByType<T>()
        {
            Type type = typeof(T);
            return GetColumnIDsByType(type);
        }


        /// <summary>
        /// Gets the column ID by name.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <returns>The column ID.</returns>
        /// <exception cref="ColumnNotFoundException">The table has no column with the specified name.</exception>
        public uint GetColumnID (string columnName)
        {
            if (ColumnNameCache.ContainsKey(columnName))
            {
                return ColumnNameCache[columnName].ID;
            }
            throw new ColumnNotFoundException(columnName);
        }


        /// <summary>
        /// Gets a column's index.
        /// </summary>
        /// <param name="id">The column ID.</param>
        /// <returns>The column index.</returns>
        /// <exception cref="ColumnNoIndexException">The table has no column indices.</exception>
        /// <exception cref="ColumnNotFoundException">The table has no column with the specified ID.</exception>
        public int GetColumnIndex (uint id)
        {
            if (!TableInfo.HasColumnIndices) throw new ColumnNoIndexException();

            try
            {
                return Columns[(int)id].Index;
            }
            catch
            {
                throw new ColumnNotFoundException(id);
            }
        }


        /// <summary>
        /// Gets a column's index.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <returns>The column index.</returns>
        /// <exception cref="ColumnNoIndexException">The table has no column indices.</exception>
        /// <exception cref="ColumnNotFoundException">The table has no column with the specified name.</exception>
        public int GetColumnIndex (string columnName)
        {
            if (!TableInfo.HasColumnIndices) throw new ColumnNoIndexException();

            if (ColumnNameCache.ContainsKey(columnName))
            {
                return ColumnNameCache[columnName].Index;
            }
            throw new ColumnNotFoundException(columnName);
        }


        /// <summary>
        /// Sets a column's index.
        /// </summary>
        /// <param name="id">The column ID.</param>
        /// <param name="newIndex">The new index.</param>
        /// <exception cref="ColumnNoIndexException">The table has no column indices.</exception>
        /// <exception cref="ColumnNotFoundException">The table has no column with the specified ID.</exception>
        public void SetColumnIndex (uint id, int newIndex)
        {
            if (!TableInfo.HasColumnIndices) throw new ColumnNoIndexException();

            try
            {
                Columns[(int)id].Index = newIndex;
            }
            catch
            {
                throw new ColumnNotFoundException(id);
            }
        }


        /// <summary>
        /// Sets a column's index.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="newIndex">The new index.</param>
        /// <exception cref="ColumnNoIndexException">The table has no column indices.</exception>
        /// <exception cref="ColumnNotFoundException">The table has no column with the specified name.</exception>
        public void SetColumnIndex (string columnName, int newIndex)
        {
            if (!TableInfo.HasColumnIndices) throw new ColumnNoIndexException();

            try
            {
                int id = (int)GetColumnID(columnName);
                Columns[id].Index = newIndex;
            }
            catch
            {
                throw new ColumnNotFoundException(columnName);
            }
        }


        /// <summary>
        /// Gets a boolean indicating if the column is valid.
        /// </summary>
        /// <param name="id">The column ID.</param>
        /// <returns>A <see cref="Boolean"/>.</returns>
        /// <exception cref="ColumnNoValidityException">The table has no column validity.</exception>
        /// <exception cref="ColumnNotFoundException">The table has no column with the specified ID.</exception>
        public bool IsColumnValid (uint id)
        {
            if (!TableInfo.HasColumnValidity) throw new ColumnNoValidityException();

            try
            {
                return (bool)Columns[(int)id].IsValid;
            }
            catch
            {
                throw new ColumnNotFoundException(id);
            }
        }


        /// <summary>
        /// Gets a boolean indicating if the column is valid.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <returns>A <see cref="Boolean"/>.</returns>
        /// <exception cref="ColumnNoValidityException">The table has no column validity.</exception>
        /// <exception cref="ColumnNotFoundException">The table has no column with the specified name.</exception>
        public bool IsColumnValid (string columnName)
        {
            if (!TableInfo.HasColumnValidity) throw new ColumnNoValidityException();

            try
            {
                return (bool)ColumnNameCache[columnName].IsValid;
            }
            catch
            {
                throw new ColumnNotFoundException(columnName);
            }
        }


        /// <summary>
        /// Make a column valid/invalid. All entry values of a valid column made invalid will be lost.
        /// All entry values of an invalid column made valid will be set to their defaults.
        /// </summary>
        /// <param name="column">The <see cref="ArmpTableColumn"/>.</param>
        /// <param name="isValid">The new column validity.</param>
        /// <exception cref="ColumnNoValidityException">The table has no column validity.</exception>
        public void SetColumnValidity (ArmpTableColumn column, bool isValid)
        {
            if (TableInfo.HasColumnValidity)
            {
                if (isValid) //Set all values to default
                {
                    foreach(ArmpEntry entry in Entries)
                    {
                        entry.SetDefaultColumnContent(column.Name);
                    }
                    column.IsValid = true;
                }
                else //Remove all values
                {
                    foreach (ArmpEntry entry in Entries)
                    {
                        entry.RemoveColumnContent(column.Name);
                    }
                    column.IsValid = false;
                }
            }
            else
            {
                throw new ColumnNoValidityException();
            }
        }


        /// <summary>
        /// Make a column valid/invalid. All entry values of a valid column made invalid will be lost.
        /// All entry values of an invalid column made valid will be set to their defaults.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="isValid">The new column validity.</param>
        /// <exception cref="ColumnNoValidityException">The table has no column validity.</exception>
        /// <exception cref="ColumnNotFoundException">The table has no column with the specified name.</exception>
        public void SetColumnValidity (string columnName, bool isValid)
        {
            try
            {
                ArmpTableColumn column = ColumnNameCache[columnName];
                SetColumnValidity(column, isValid);
            }
            catch (ColumnNoValidityException ex)
            {
                throw ex;
            }
            catch
            {
                throw new ColumnNotFoundException(columnName);
            }
        }


        /// <summary>
        /// Returns if the column is special (array/list). Only used in v2.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <returns>A <see cref="Boolean"/></returns>
        /// <exception cref="ColumnNotFoundException">The table has no column with the specified name.</exception>
        public bool IsColumnSpecial (string columnName)
        {
            if (ColumnNameCache.ContainsKey(columnName))
            {
                return ColumnNameCache[columnName].IsSpecial;
            }
            throw new ColumnNotFoundException(columnName);
        }


        /// <summary>
        /// Sets the column's value to default for all entries.
        /// </summary>
        /// <param name="column">The <see cref="ArmpTableColumn"/>.</param>
        internal void SetDefaultColumnContent (ArmpTableColumn column)
        {
            foreach (ArmpEntry entry in Entries)
            {
                entry.SetDefaultColumnContent(column);
            }
        }


        /// <summary>
        /// Adds a new column to the table.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="columnType">The column type.</param>
        /// <returns>The newly created column.</returns>
        /// <exception cref="TypeNotSupportedException">The provided C# type is not supported by the armp format.</exception>
        public ArmpTableColumn AddColumn (string columnName, Type columnType)
        {
            uint id = (uint)Columns.Count;
            ArmpType armpType = DataTypes.GetArmpTypeByCSType(columnType);
            ArmpTableColumn column = new ArmpTableColumn(id, columnName, armpType);
            if (TableInfo.HasColumnIndices) column.Index = (int)id;
            if (TableInfo.HasColumnValidity) column.IsValid = true;
            column.IsNoData = false;

            if (TableInfo.FormatVersion == Version.DragonEngineV2)
            {
                //If the type is special
                if (DataTypes.SpecialTypes.Contains(armpType.CSType))
                {
                    column.IsSpecial = true;
                    column.SpecialSize = 0; //currently empty
                    column.Children = new List<ArmpTableColumn>();
                }

                //Check if the name matches an existing special column and assign parent/child
                //NOTE: unsure if allowing the creation of a column with child naming but no parent will cause problems down the line.
                if (columnName.Contains("[") && columnName.Contains("]"))
                {
                    string baseName = columnName.Split('[')[0];
                    if (ColumnNameCache.ContainsKey(baseName))
                    {
                        ArmpTableColumn parentColumn = ColumnNameCache[baseName];
                        parentColumn.Children.Add(column);
                        parentColumn.SpecialSize += 1;
                        column.Parent = parentColumn;
                    }
                }
            }

            Columns.Add(column);
            RefreshColumnNameCache();
            SetDefaultColumnContent(column);
            return column;
        }


        /// <summary>
        /// Adds a new column to the table.
        /// </summary>
        /// <typeparam name="T">The column type.</typeparam>
        /// <param name="columnName">The column name.</param>
        /// <returns>The newly created column.</returns>
        /// <exception cref="TypeNotSupportedException">The provided C# type is not supported by the armp format.</exception>
        public ArmpTableColumn AddColumn<T> (string columnName)
        {
            return AddColumn(columnName, typeof(T));
        }


        /// <summary>
        /// Deletes the specified column from the table.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <returns>A <see cref="Boolean"/> indicating if the operation completed successfully.</returns>
        public bool DeleteColumn (string columnName)
        {
            //TODO: Deleting a column will break how the game reads subsequent columns. Indices may need to be updated. Make it an optional argument?
            if (ColumnNameCache.ContainsKey(columnName))
            {
                ArmpTableColumn column = ColumnNameCache[columnName];

                //Remove column from parent and children
                if (column.Parent != null)
                {
                    column.Parent.Children.Remove(column);
                    column.Parent.SpecialSize -= 1;
                    column.Parent = null;
                }

                foreach (ArmpTableColumn child in column.Children)
                {
                    child.Parent = null;
                }

                Columns.Remove(column);
                RefreshColumnNameCache();
                return true;
            }
            return false;
        }


        /// <summary>
        /// Searches all entries for matching names.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>An <see cref="ArmpEntry"/> list.</returns>
        public List<ArmpEntry> SearchByName (string name)
        {
            List<ArmpEntry> returnList = new List<ArmpEntry>();
            foreach(ArmpEntry entry in Entries)
            {
                if (entry.Name.Contains(name)) returnList.Add(entry);
            }
            return returnList;
        }


        /// <summary>
        /// Searches all entries for matching values in the specified column.
        /// </summary>
        /// <param name="columnName">The column containing the value to find.</param>
        /// <param name="value">The value to find.</param>
        /// <returns>An <see cref="ArmpEntry"/> list.</returns>
        /// <exception cref="ColumnNotFoundException">The table has no column with the specified name.</exception>
        public List<ArmpEntry> SearchByValue (string columnName, object value)
        {
            if (ColumnNameCache.ContainsKey(columnName))
            {
                ArmpTableColumn column = ColumnNameCache[columnName];
                value = Convert.ChangeType(value, column.Type.CSType);

                List<ArmpEntry> returnList = new List<ArmpEntry>();
                foreach (ArmpEntry entry in Entries)
                {
                    if (value.Equals(entry.Data[columnName]))
                    {
                        returnList.Add(entry);
                    }
                }
                return returnList;
            }
            throw new ColumnNotFoundException(columnName);
        }


        /// <summary>
        /// Creates and adds a new entry to the end of the table.
        /// </summary>
        /// <param name="name">The new entry name.</param>
        public ArmpEntry AddEntry (string name = "")
        {
            uint id = (uint)Entries.Count;
            ArmpEntry entry = new ArmpEntry(this, id, name);
            entry.SetDefaultColumnContent();
            if (TableInfo.HasEntryIndices)
                entry.Index = (int)id;
            Entries.Add(entry);
            return entry;
        }


        /// <summary>
        /// Creates and inserts a new entry at the specified ID.
        /// </summary>
        /// <param name="id">The new entry ID.</param>
        /// <param name="name">The new entry name.</param>
        /// <exception cref="EntryInsertException">The specified ID is greater than the amount of entries in the table.</exception>
        public ArmpEntry InsertEntry (uint id, string name = "")
        {
            if (id <= Entries.Count)
            {
                ArmpEntry entry = new ArmpEntry(this, id, name);
                entry.SetDefaultColumnContent();
                Entries.Insert((int)id, entry);

                if (Entries.Count > id)
                {
                    foreach (ArmpEntry e in Entries.GetRange((int)id + 1, Entries.Count - (int)id - 1))
                    {
                        e.ID++;
                    }
                }
                return entry;
            }
            else
            {
                throw new EntryInsertException((int)id);
            }
        }


        /// <summary>
        /// Deletes the specified entry and updates the IDs for any entries after it.
        /// </summary>
        /// <param name="id">The ID of the entry to delete.</param>
        /// <exception cref="EntryNotFoundException">The table has no entry with the specified ID.</exception>
        public void DeleteEntry (uint id)
        {
            if (id < Entries.Count)
            {
                Entries.RemoveAt((int)id);
                foreach (ArmpEntry entry in Entries.GetRange((int)id, Entries.Count - (int)id))
                {
                    entry.ID--;
                }
            }
            else
            {
                throw new EntryNotFoundException(id);
            }
        }


        /// <summary>
        /// Deletes the specified entry and updates the IDs for any entries after it.
        /// </summary>
        /// <param name="name">The name of the entry to delete.</param>
        /// <exception cref="EntryNotFoundException">The table has no entry with the specified name.</exception>
        public void DeleteEntry (string name)
        {
            try
            {
                ArmpEntry entry = GetEntry(name);
                DeleteEntry(entry.ID);
            }
            catch
            {
                throw new EntryNotFoundException(name);
            }
        }


        /// <summary>
        /// Sets the value of a specified column in the specified entry.
        /// </summary>
        /// <param name="id">The entry to modify.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="value">The value to write.</param>
        /// <exception cref="TypeMismatchException">The column type does not match the type of the provided object.</exception>
        /// <exception cref="ColumnNotFoundException">The table has no column with the specified name.</exception>
        public void SetValue (uint id, string columnName, object value)
        {
            if (ColumnNameCache.ContainsKey(columnName))
            {
                ArmpTableColumn column = ColumnNameCache[columnName];
                if (value.GetType() == column.Type.CSType)
                {
                    ArmpEntry entry = GetEntry(id);
                    entry.SetValueFromColumn(columnName, value);

                    //PLACEHOLDER PATCHER CODE
                    if (column.Type.CSType != typeof(string))
                    {
                        if (!EditedValues.ContainsKey(column.Name))
                        {
                            EditedValues.Add(column.Name, new List<uint>());
                        }
                        EditedValues[column.Name].Add(id);
                    }
                }
                else
                {
                    throw new TypeMismatchException(column.Type.CSType, value.GetType());
                }
            }
            else
            {
                throw new ColumnNotFoundException(columnName);
            }
        }


        /// <summary>
        /// Sets the selected column as string type.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <exception cref="ColumnNotFoundException">The table has no column with the specified name.</exception>
        /// <remarks><para><b>This is only needed for Old Engine files with text.</b></para></remarks>
        public void SetTextColumnOE (string columnName)
        {
            if (ColumnNameCache.ContainsKey(columnName))
            {
                ArmpTableColumn column = ColumnNameCache[columnName];
                column.Type = DataTypes.GetArmpTypeByCSType(typeof(string));
                foreach (ArmpEntry entry in Entries)
                {
                    Int16 textIndex = (Int16)entry.Data[columnName];
                    if (textIndex != -1) entry.Data[columnName] = Text[textIndex];
                    else entry.Data[columnName] = null;
                }
            }
            else
            {
                throw new ColumnNotFoundException(columnName);
            }
        }


        /// <summary>
        /// Calculates the distance between data for Storage Mode 1 columns.
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE V2 (STORAGE MODE 1) ONLY</b></para></remarks>
        internal void UpdateColumnDistances()
        {
            int distance = 0;

            foreach (ArmpTableColumn column in Columns)
            {
                if (column.Type.Size == 0)
                {
                    column.Distance = distance;
                    continue;
                }

                int padding = distance % column.Type.Size;
                column.Distance = distance + padding;
                distance = column.Distance + column.Type.Size;
            }
        }


        /// <summary>
        /// Updates the internal text list for writing.
        /// </summary>
        internal void UpdateTextList()
        {
            Text = new List<string>();

            foreach (ArmpTableColumn column in GetColumnsByType<string>())
            {
                foreach (ArmpEntry entry in Entries)
                {
                    string str = entry.GetValueFromColumn<string>(column);
                    if (!Text.Contains(str) && str != null) Text.Add(str);
                }
            }
        }


        /// <summary>
        /// Sets the <see cref="StorageMode"/> for this table and any tables contained within.
        /// </summary>
        /// <param name="storageMode">The <see cref="StorageMode"/> to set the table to.</param>
        /// <remarks><b>Only perform this operation on the armp's main table.</b></remarks>
        public void SetStorageMode (StorageMode storageMode)
        {
            TableInfo.StorageMode = storageMode;

            //Storage mode needs to be the same for all tables inside
            foreach (ArmpTableColumn column in GetColumnsByType<ArmpTableMain>())
            {
                if ((bool)column.IsValid)
                {
                    foreach (ArmpEntry e in Entries)
                    {
                        try
                        {
                            e.GetValueFromColumn<ArmpTableMain>(column);
                        }
                        catch { continue; }
                        if (e.GetValueFromColumn<ArmpTableMain>(column) != null)
                            e.GetValueFromColumn<ArmpTableMain>(column).TableInfo.StorageMode = storageMode;
                    }
                }
            }
        }
    }
}
