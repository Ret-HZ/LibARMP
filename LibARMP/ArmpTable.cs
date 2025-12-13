using System;
using System.Reflection;

namespace LibARMP
{
    [Serializable]
    public class ArmpTable : ArmpTableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArmpTable"/> class.
        /// </summary>
        internal ArmpTable() : base()
        {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ArmpTable"/> class and populates it with the properties of the source <see cref="ArmpTableBase"/>.
        /// </summary>
        /// <param name="armpTable">The source <see cref="ArmpTableBase"/> object.</param>
        internal ArmpTable(ArmpTableBase armpTable) : this()
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
        /// The indexer for this table.
        /// </summary>
        public ArmpTableIndexer Indexer { get; internal set; }



        /// <summary>
        /// Creates a copy of this table. This includes the indexer table if one exists.
        /// </summary>
        /// <param name="copyEntries">Should entries be copied? Default value is <see langword="true"/>.</param>
        /// <returns>A copy of this <see cref="ArmpTable"/>.</returns>
        public new ArmpTable Copy (bool copyEntries = true)
        {
            ArmpTable copy = new ArmpTable(base.Copy(copyEntries));

            if (copy.TableInfo.HasIndexerTable)
            {
                copy.Indexer = Indexer.Copy(copyEntries);
            }

            return copy;
        }
    }
}
