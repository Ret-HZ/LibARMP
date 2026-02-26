using LibARMP.Exceptions;
using System;
using System.Collections.Generic;

namespace LibARMP
{
    [Serializable]
    public class ArmpTableColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArmpTableColumn"/> class.
        /// </summary>
        /// <param name="parentTable">The table that contains this column.</param>
        /// <param name="id">The column ID.</param>
        internal ArmpTableColumn(ArmpTableBase parentTable)
        {
            ParentTable = parentTable;
            ColumnMetadata = -1;
            GameVarID = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArmpTableColumn"/> class.
        /// </summary>
        /// <param name="parentTable">The table that contains this column.</param>
        /// <param name="id">The column ID.</param>
        /// <param name="name">The column name.</param>
        /// <param name="type">The column type.</param>
        internal ArmpTableColumn(ArmpTableBase parentTable, string name, ArmpType type) : this(parentTable)
        {
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Gets the column ID.
        /// </summary>
        public uint ID
        {
            get
            {
                if (ParentTable == null) return 0;
                return (uint)ParentTable.Columns.IndexOf(this);
            }
        }

        /// <summary>
        /// Gets the column name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the column type.
        /// </summary>
        internal ArmpType Type { get; set; }

        /// <summary>
        /// Gets the member info, if applicable.
        /// </summary>
        /// <remarks>This reference is kept to avoid unnecessarily sorting the table's MemberInfo List.</remarks>
        internal ArmpMemberInfo MemberInfo { get; set; }

        /// <summary>
        /// Gets the column's display index, which may differ from its ID.
        /// </summary>
        /// <remarks><para>Can be null if unused.</para></remarks>
        public uint Index { get; internal set; }

        /// <summary>
        /// Gets or sets if the column is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the column metadata.
        /// </summary>
        /// <remarks><para><b>OLD ENGINE AND DRAGON ENGINE V1 ONLY</b></para></remarks>
        public int ColumnMetadata { get; set; }

        /// <summary>
        /// Gets or sets the game_var ID.
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE ONLY</b></para></remarks>
        // (Meaning still unknown for version 1. May be maximum, default, or otherwise special column values.)
        public int GameVarID { get; set; }

        /// <summary>
        /// Gets or sets the column's children.
        /// </summary>
        /// <remarks><para>Only used if the column is of an array type.</para><para><b>DRAGON ENGINE V2 ONLY</b></para></remarks>
        internal List<ArmpTableColumn> Children { get; set; }

        /// <summary>
        /// Gets or sets the column's parent.
        /// </summary>
        /// <remarks><para>Only used if the column is child of an array-type column.</para><para><b>DRAGON ENGINE V2 ONLY</b></para></remarks>
        internal ArmpTableColumn Parent { get; set; }

        /// <summary>
        /// The <see cref="ArmpTableBase"/> this column belongs to.
        /// </summary>
        internal ArmpTableBase ParentTable { get; set; }



        /// <summary>
        /// Gets the column's data type.
        /// </summary>
        /// <returns>The column <see cref="System.Type"/>.</returns>
        public Type GetDataType()
        {
            return Type.CSType;
        }


        /// <summary>
        /// Creates a copy of this column.
        /// </summary>
        /// <returns>A copy of this <see cref="ArmpTableColumn"/>.</returns>
        public ArmpTableColumn Copy()
        {
            ArmpTableColumn copy = new ArmpTableColumn(ParentTable, Name, Type);
            copy.Index = Index;
            copy.IsValid = IsValid;
            copy.ColumnMetadata = ColumnMetadata;
            copy.GameVarID = GameVarID;

            if (Type.IsArray)
                copy.Children = new List<ArmpTableColumn>(Children.Count);

            return copy;
        }


        /// <summary>
        /// Sets the display index, which may differ from the <see cref="ID"/>. 
        /// If any other <see cref="ArmpTableColumn"/> is already making use of the provided index, it will be updated with this <see cref="ArmpTableColumn"/>'s previous index.
        /// </summary>
        /// <param name="index">The new index.</param>
        /// <exception cref="ColumnNoIndexException">The does not have column indices.</exception>
        /// <exception cref="IndexOutOfRangeException">The index value is out of range.</exception>
        public void SetIndex(uint index)
        {
            if (ParentTable == null) return; // TODO: Maybe throw an exception here to warn about this edge case
            if (!ParentTable.TableInfo.HasOrderedColumns)
                throw new ColumnNoIndexException();
            if (index >= ParentTable.Columns.Count)
                throw new IndexOutOfRangeException($"The value of 'index' ({index}) cannot be equal or greater than the total amount of columns in the table ({ParentTable.Columns.Count}).");

            int columnID = (int)ID;
            Index = index;
            uint oldIndex = ParentTable.OrderedColumnIDs[columnID];

            // Swap the index value for any column already using it
            for (int i = 0; i < ParentTable.Columns.Count; i++)
            {
                if (ParentTable.OrderedColumnIDs[i] == index)
                {
                    ParentTable.OrderedColumnIDs[i] = oldIndex;
                    ParentTable.Columns[i].Index = oldIndex;
                    break;
                }
            }

            ParentTable.OrderedColumnIDs[columnID] = index;
        }
    }
}
