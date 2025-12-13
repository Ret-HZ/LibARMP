using LibARMP.Exceptions;
using System;
using System.Collections.Generic;

namespace LibARMP
{   
    [Serializable]
    public class ArmpEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArmpEntry"/> class.
        /// </summary>
        /// <param name="parentTable">The table that contains this entry.</param>
        internal ArmpEntry(ArmpTableBase parentTable)
        {
            Data = new Dictionary<string, object>();
            ParentTable = parentTable;

            if (!ParentTable.TableInfo.HasOrderedEntries)
                Index = ID;

            IsValid = true;

            if (ParentTable.TableInfo.HasExtraFieldInfo && ParentTable.TableInfo.FormatVersion == Version.DragonEngineV1)
                Flags = new bool[8] { false, false, false, false, false, false, false, false };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmpEntry"/> class.
        /// </summary>
        /// <param name="parentTable">The table that contains this entry.</param>
        /// <param name="id">The entry ID.</param>
        /// <param name="name">The entry name.</param>
        /// <param name="index">The entry index.</param>
        internal ArmpEntry(ArmpTableBase parentTable, uint id, string name, uint index) : this(parentTable)
        {
            ID = id;
            Name = name;
            Index = index;
        }



        /// <summary>
        /// Gets the entry ID.
        /// </summary>
        public uint ID { get; internal set; }

        /// <summary>
        /// Gets or sets the entry name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Data per column.
        /// </summary>
        /// <remarks><para>column name : value</para></remarks>
        internal IDictionary<string, object> Data { get; set; }

        /// <summary>
        /// Gets or sets the entry's display index, which may differ from its ID.
        /// </summary>
        /// <remarks><para>DRAGON ENGINE ONLY</para></remarks>
        public uint Index { get; set; }

        /// <summary>
        /// Gets or sets if the entry is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the entry flags.
        /// </summary>
        /// <remarks><para>Can be null if unused.</para></remarks>
        public bool[] Flags { get; set; }

        /// <summary>
        /// The <see cref="ArmpTableBase"/> this entry belongs to.
        /// </summary>
        internal ArmpTableBase ParentTable { get; set; }



        /// <summary>
        /// Creates a copy of this entry to use in a different table.
        /// </summary>
        /// <param name="parentTable">The table this copy is intended to.</param>
        /// <returns>A copy of this <see cref="ArmpEntry"/>.</returns>
        public ArmpEntry Copy (ArmpTableBase parentTable)
        {
            ArmpEntry copy = new ArmpEntry(parentTable, ID, Name, Index);
            copy.IsValid = IsValid;
            copy.Flags = Flags;
            copy.Data = new Dictionary<string, object>(Data);
            //Copy the table type column values since the dictionary copy does not apply to references
            foreach(ArmpTableColumn column in ParentTable.GetColumnsByType<ArmpTable>())
            {
                if (Data.ContainsKey(column.Name))
                {
                    var originalObject = GetValueFromColumn(column);
                    if (originalObject != null)
                    {
                        ArmpTable original = (ArmpTable)originalObject;
                        copy.Data[column.Name] = original.Copy(true);
                    }
                    else
                    {
                        copy.Data[column.Name] = null;
                    }
                }
            }

            return copy;
        }


        /// <summary>
        /// Sets the value for the column to default.
        /// </summary>
        /// <param name="column">The <see cref="ArmpTableColumn"/>.</param>
        internal void SetDefaultColumnContent (ArmpTableColumn column)
        {
            if (!column.Type.IsArray)
            {
                SetValueFromColumn(column.Name, column.Type.DefaultValue);
            }
        }


        /// <summary>
        /// Sets the value for the column to default.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <exception cref="ColumnNotFoundException">The column name doesn't match any columns in the table.</exception>
        internal void SetDefaultColumnContent (string columnName)
        {
            try
            {
                ArmpTableColumn column = ParentTable.ColumnNameCache[columnName];
                SetDefaultColumnContent(column);
            }
            catch
            {
                throw new ColumnNotFoundException(columnName);
            }
        }


        /// <summary>
        /// Sets the values for all columns to their default.
        /// </summary>
        internal void SetDefaultColumnContent()
        {
            foreach (ArmpTableColumn column in ParentTable.Columns)
            {
                SetDefaultColumnContent(column);
            }
        }


        /// <summary>
        /// Removes the contents of the specified column.
        /// </summary>
        /// <returns>A <see cref="Boolean"/> indicating if the operation completed successfully.</returns>
        internal bool RemoveColumnContent (string columnName)
        {
            try
            {
                Data.Remove(columnName);
                return true;
            }
            catch 
            {
                return false;
            }
        }


        /// <summary>
        /// Removes the contents of the specified column.
        /// </summary>
        /// <returns>A <see cref="Boolean"/> indicating if the operation completed successfully.</returns>
        internal bool RemoveColumnContent (ArmpTableColumn column)
        {
            return RemoveColumnContent(column.Name);
        }


        /// <summary>
        /// Gets the value for the specified column.
        /// </summary>
        /// <param name="column">The <see cref="ArmpTableColumn"/>.</param>
        public object GetValueFromColumn (ArmpTableColumn column)
        {
            if (Data.ContainsKey(column.Name))
            {
                object data = Data[column.Name];
                // Convert to column type if the data is stored as member type for v2 structured
                if (ParentTable.TableInfo.FormatVersion == Version.DragonEngineV2 && ParentTable.TableInfo.HasMemberInfo)
                {
                    if (column.Type.CSType != column.MemberInfo.Type.CSType)
                    {
                        data = Convert.ChangeType(data, column.Type.CSType);
                    }
                }
                return data;
            }
            else // Check if the column doesn't exist or has no data
            {
                try
                {
                    ParentTable.GetColumn(column.Name);
                }
                catch (ColumnNotFoundException)
                {
                    throw;
                }

                throw new ColumnNoDataException(column.Name);
            }
        }

        /// <summary>
        /// Check if column has data.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <returns>A <see cref="Boolean"/> indicating if the column has data.</returns>
        public bool ColumnHasData (string columnName)
        {
            return Data.ContainsKey(columnName);
        }


        /// <summary>
        /// Gets the value for the specified column.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <exception cref="ColumnNotFoundException">The column name doesn't match any columns in the table.</exception>
        /// <exception cref="ColumnNoDataException">The column has no data.</exception>
        public object GetValueFromColumn (string columnName)
        {
            ArmpTableColumn column = ParentTable.GetColumn(columnName);
            return GetValueFromColumn(column);
        }


        /// <summary>
        /// Gets the value for the specified column.
        /// </summary>
        /// <param name="columnID">The column ID.</param>
        /// <exception cref="ColumnNotFoundException">The column name doesn't match any columns in the table.</exception>
        /// <exception cref="ColumnNoDataException">The column has no data.</exception>
        public object GetValueFromColumn (uint columnID)
        {
            ArmpTableColumn column = ParentTable.GetColumn(columnID);
            return GetValueFromColumn(column);
        }


        /// <summary>
        /// Gets the value for the specified column.
        /// </summary>
        /// <param name="column">The <see cref="ArmpTableColumn"/>.</param>
        /// <exception cref="InvalidTypeConversionException">The column type cannot be converted to the requested type.</exception>
        public T GetValueFromColumn<T> (ArmpTableColumn column)
        {
            object result = GetValueFromColumn(column.Name);
            
            try
            {
                // Attempt direct cast first
                if (result is T t)
                    return t;

                // If direct cast is not possible, try ChangeType instead
                Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                object converted = Convert.ChangeType(result, targetType);
                return (T)converted;
            }
            catch
            {
                Type paramType = typeof(T);
                throw new InvalidTypeConversionException(result.GetType(), paramType);
            }
        }


        /// <summary>
        /// Gets the value for the specified column.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <exception cref="InvalidTypeConversionException">The column type cannot be converted to the requested type.</exception>
        public T GetValueFromColumn<T> (string columnName)
        {
            ArmpTableColumn column = ParentTable.GetColumn(columnName);
            return GetValueFromColumn<T>(column);
        }


        /// <summary>
        /// Gets the value for the specified column.
        /// </summary>
        /// <param name="columnID">The column index.</param>
        /// <exception cref="InvalidTypeConversionException">The column type cannot be converted to the requested type.</exception>
        public T GetValueFromColumn<T> (uint columnID)
        {
            ArmpTableColumn column = ParentTable.GetColumn(columnID);
            return GetValueFromColumn<T>(column);
        }


        /// <summary>
        /// Sets the value for the specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="value">The value to write. <see langword="null"/> will set the value to its default.</param>
        /// <exception cref="TypeMismatchException">The column type does not match the type of the provided object and no cast is possible.</exception>
        public void SetValueFromColumn (ArmpTableColumn column, object value = null)
        {
            if (value == null)
            {
                Data[column.Name] = column.Type.DefaultValue;
                return;
            }

            Type targetType = column.Type.CSType;

            // If the stored types needs to be the member type
            if (ParentTable.TableInfo.FormatVersion == Version.DragonEngineV2 && ParentTable.TableInfo.HasMemberInfo)
            {
                targetType = column.MemberInfo.Type.CSType;
            }

            // Try to convert the value before throwing an exception if the types dont match
            if (targetType != value.GetType())
            {
                try
                {
                    value = Convert.ChangeType(value, targetType);
                }
                catch
                {
                    throw new TypeMismatchException(column.Type.CSType, value.GetType());
                }
            }

            Data[column.Name] = value;
            if (value is string) ParentTable.TableInfo.HasText = true;
        }


        /// <summary>
        /// Sets the value for the specified column.
        /// </summary>
        /// <param name="columnID">The column ID.</param>
        /// <param name="value">The value to write. <see langword="null"/> will set the value to its default.</param>
        /// <exception cref="TypeMismatchException">The column type does not match the type of the provided object and no cast is possible.</exception>
        /// <exception cref="ColumnNotFoundException">The column does not exist.</exception>
        public void SetValueFromColumn (uint columnID, object value = null)
        {
            ArmpTableColumn column = ParentTable.GetColumn(columnID);
            SetValueFromColumn(column, value);
        }


        /// <summary>
        /// Sets the value for the specified column.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <param name="value">The value to write. <see langword="null"/> will set the value to its default.</param>
        /// <exception cref="TypeMismatchException">The column type does not match the type of the provided object and no cast is possible.</exception>
        /// <exception cref="ColumnNotFoundException">The column does not exist.</exception>
        public void SetValueFromColumn (string columnName, object value = null)
        {
            ArmpTableColumn column = ParentTable.GetColumn(columnName);
            SetValueFromColumn(column, value);
        }
    }
}
