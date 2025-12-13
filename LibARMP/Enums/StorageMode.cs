namespace LibARMP
{
    /// <summary>
    /// How the data is stored.
    /// </summary>
    public enum StorageMode
    {
        /// <summary>
        /// Data is stored on a per-column basis.
        /// All valid columns are represented.
        /// </summary>
        Column,

        /// <summary>
        /// Data is stored in a structure defined by a member specification.
        /// Only members with associated valid columns are represented.
        /// </summary>
        /// <remarks><b>DRAGON ENGINE V2 ONLY</b></remarks>
        Structured,
    }
}
