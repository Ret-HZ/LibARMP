﻿using System;

namespace LibARMP
{
    [Serializable]
    public class ArmpTableInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArmpTableInfo"/> class.
        /// </summary>
        internal ArmpTableInfo ()
        {
            //General Info
            this.EntryCount = 0;
            this.ColumnCount = 0;
            this.TextCount = 0;
            this.DefaultEntryIndex = -1;
            this.DefaultColumnIndex = -1;
            this.TableID = 0;
            this.StorageMode = 0;

            //Pointers
            this.ptrEntryNamesOffsetTable = 0;
            this.ptrEntryValidity = 0;
            this.ptrColumnDataTypes = 0;
            this.ptrColumnContentOffsetTable = 0;
            this.ptrTextOffsetTable = 0;
            this.ptrColumnNamesOffsetTable = 0;
            this.ptrEntryIndices = 0;
            this.ptrColumnIndices = 0;
            this.ptrColumnValidity = 0;
            this.ptrIndexerTable = 0;
            this.ptrBlankCellFlagOffsetTable = 0;
            this.ptrMemberInfo = 0;
            this.ptrExtraFieldInfo = 0;
            this.ptrColumnMetadata = 0;

            //Flags
            this.HasText = false;
            this.HasIndexerTable = false;
            this.HasEntryNames = false;
            this.HasColumnNames = false;
            this.HasMemberInfo = false;
            this.HasEntryValidity = false;
            this.HasEntryIndices = false;
            this.HasColumnIndices = false;
            this.HasExtraFieldInfo = false;
            this.HasBlankCellFlags = false;

            //Extra data
            this.FormatVersion = Version.Unknown;
        }


        //General Info

        /// <summary>
        /// Gets the number of entries.
        /// </summary>
        public Int32 EntryCount { get; internal set; }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        public Int32 ColumnCount { get; internal set; }

        /// <summary>
        /// Gets the number of strings.
        /// </summary>
        public Int32 TextCount { get; internal set; }

        /// <summary>
        /// The index of the default entry, if one exists.
        /// This resolves errors when an entry is not found.
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE ONLY</b></para></remarks>
        public Int32 DefaultEntryIndex { get; internal set; }

        /// <summary>
        /// The index of the default column, if one exists.
        /// This resolves errors when a column is not found.
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE ONLY</b></para></remarks>
        public Int32 DefaultColumnIndex { get; internal set; }

        /// <summary>
        /// Gets the table ID (Int24).
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE ONLY</b></para></remarks>
        public Int32 TableID { get; internal set; }

        /// <summary>
        /// Gets the storage mode.
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE V2 ONLY</b></para></remarks>
        public StorageMode StorageMode { get; internal set; } //Flag 0

        public bool UnknownFlag1 { get; internal set; } //Flag 1
        public bool UnknownFlag2 { get; internal set; } //Flag 2
        public bool UnknownFlag3 { get; internal set; } //Flag 3
        public bool UnknownFlag4 { get; internal set; } //Flag 4

        /// <summary>
        /// When set, signals not to point directly to raw entries within the armp in memory.
        /// In other words, this flag forces code to ignore flag 6.
        /// </summary>
        public bool DoNotUseRaw { get; internal set; } //Flag 5

        /// <summary>
        /// When set, signals that member info in the armp (loaded into memory) has been verified to be formatted as expected.
        /// The game's code will then treat armp entries as valid structs where applicable.
        /// </summary>
        /// <remarks>Cannot be set without flag 7.</remarks>
        public bool MembersWellFormatted { get; internal set; } //Flag 6

        /// <summary>
        /// When set, signals that the armp table in memory has been initialized.
        /// In Storage Mode 1, initialization includes string and table pointer adjustments, as well as member info verification.
        /// In Storage Mode 0, this flag is only a rubber stamp.
        /// </summary>
        /// <remarks>Flag 7</remarks>
        public bool IsProcessedForMemory { get; internal set; } //Flag 7



        //Pointers

        /// <summary>
        /// Gets or sets the pointer to the Main Table.
        /// </summary>
        /// <remarks><para>DEBUG</para></remarks>
        internal UInt32 ptrMainTable { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the String Offset Table.
        /// </summary>
        internal UInt32 ptrEntryNamesOffsetTable { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the Entry Validity bitmask.
        /// </summary>
        internal UInt32 ptrEntryValidity { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the Column Data Types.
        /// </summary>
        internal UInt32 ptrColumnDataTypes { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the Column Content Offset Table.
        /// </summary>
        internal UInt32 ptrColumnContentOffsetTable { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the Text Offset Table.
        /// </summary>
        internal UInt32 ptrTextOffsetTable { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the Column Names Offset Table.
        /// </summary>
        internal UInt32 ptrColumnNamesOffsetTable { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the Entry Indices int array.
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE ONLY</b></para></remarks>
        internal UInt32 ptrEntryIndices { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the Column Indices int array.
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE ONLY</b></para></remarks>
        internal UInt32 ptrColumnIndices { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the Column Validity bitmask.
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE ONLY</b></para></remarks>
        internal UInt32 ptrColumnValidity { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the Indexer table.
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE ONLY</b></para></remarks>
        internal UInt32 ptrIndexerTable { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the Blank Cell Flag Offset Table.
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE ONLY</b></para></remarks>
        internal UInt32 ptrBlankCellFlagOffsetTable { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the Member Info table.
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE ONLY</b></para></remarks>
        internal UInt32 ptrMemberInfo { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the additional Field Info.
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE ONLY</b></para></remarks>
        internal UInt32 ptrExtraFieldInfo { get; set; }

        /// <summary>
        /// Gets or sets the pointer to the Column Metadata.
        /// </summary>
        internal UInt32 ptrColumnMetadata { get; set; }



        //Flags

        /// <summary>
        /// Gets a boolean indicating if the table has text.
        /// </summary>
        public bool HasText { get; internal set; }

        /// <summary>
        /// Gets a boolean indicating if the table has an Indexer table.
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE ONLY</b></para></remarks>
        public bool HasIndexerTable { get; internal set; }

        /// <summary>
        /// Gets a boolean indicating if the table has entry names.
        /// </summary>
        public bool HasEntryNames { get; internal set; }

        /// <summary>
        /// Gets a boolean indicating if the table has column names.
        /// </summary>
        public bool HasColumnNames { get; internal set; }

        /// <summary>
        /// Gets or sets the boolean indicating if the table has a structure per entry with member types.
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE ONLY</b></para></remarks>
        internal bool HasMemberInfo { get; set; }

        /// <summary>
        /// Gets a boolean indicating if the table has validity flags for entries.
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE ONLY</b></para></remarks>
        public bool HasEntryValidity { get; internal set; }

        /// <summary>
        /// Gets a boolean indicating if the display order of entries on the table differs from their IDs.
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE ONLY</b></para></remarks>
        public bool HasEntryIndices { get; internal set; }

        /// <summary>
        /// Gets a boolean indicating if the the display order of columns on the table differs from their IDs.
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE ONLY</b></para></remarks>
        public bool HasColumnIndices { get; internal set; }

        /// <summary>
        /// Gets or sets the boolean indicating if the table has flags indicating if cells are blank.
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE ONLY</b></para></remarks>
        internal bool HasBlankCellFlags { get; set; }

        /// <summary>
        /// Gets a boolean indicating if the table has additional field info (varies between format versions).
        /// </summary>
        /// <remarks><para><b>DRAGON ENGINE ONLY</b></para></remarks>
        public bool HasExtraFieldInfo { get; internal set; }

        /// <summary>
        /// Gets a boolean indicating if the table has column metadata.
        /// </summary>
        public bool HasColumnMetadata { get; internal set; }



        //Version Data

        /// <summary>
        /// Which specific version it is using.
        /// </summary>
        /// <remarks><para>Version and Revision numbers are shared between multiple different format versions.</para></remarks>
        public Version FormatVersion { get; internal set; }



        // Memory Address

        /// <summary>
        /// Address where the ARMP starts in memory.
        /// </summary>
        /// <remarks><para>Only used if Flag 7 is set.</para></remarks>
        internal long BaseARMPMemoryAddress { get; set; }
    }
}
