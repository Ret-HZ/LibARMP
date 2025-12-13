using System;
using System.Reflection;

namespace LibARMP
{
    [Serializable]
    public class ArmpTableIndexer : ArmpTableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArmpTableIndexer"/> class.
        /// </summary>
        /// <param name="parentTable">The parent <see cref="ArmpTable"/>.</param>
        internal ArmpTableIndexer(ArmpTable parentTable) : base()
        {
            this.ParentTable = parentTable;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ArmpTableIndexer"/> class and populates it with the properties of the source <see cref="ArmpTableBase"/>.
        /// </summary>
        /// <param name="parentTable">The parent <see cref="ArmpTable"/>.</param>
        /// <param name="armpTable">The source <see cref="ArmpTableBase"/> object.</param>
        internal ArmpTableIndexer(ArmpTable parentTable, ArmpTableBase armpTable) : this(parentTable)
        {
            var srcProperties = armpTable.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var dstProperties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var srcProp in srcProperties)
            {
                foreach (var dstProp in dstProperties)
                {
                    if (srcProp.Name == dstProp.Name && srcProp.PropertyType == dstProp.PropertyType)
                    {
                        dstProp.SetValue(this, srcProp.GetValue(armpTable));
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// The parent table of this indexer table.
        /// </summary>
        public ArmpTable ParentTable { get; internal set; }



        /// <summary>
        /// Creates a copy of this indexer table.
        /// </summary>
        /// <param name="copyEntries">Should entries be copied? Default value is <see langword="true"/>.</param>
        /// <returns>A copy of this <see cref="ArmpTableIndexer"/>.</returns>
        public new ArmpTableIndexer Copy (bool copyEntries = true)
        {
            ArmpTableIndexer copy = new ArmpTableIndexer(ParentTable, base.Copy(copyEntries));
            return copy;
        }
    }
}
