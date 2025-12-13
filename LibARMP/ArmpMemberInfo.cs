using System;
using System.Collections.Generic;

namespace LibARMP
{
    [Serializable]
    public class ArmpMemberInfo
    {
        /// <summary>
        /// The field's type.
        /// </summary>
        internal ArmpType Type { get; set; }

        /// <summary>
        /// The field's position in the member.
        /// </summary>
        internal int Position { get; set; }

        /// <summary>
        /// Size of the array, if this field begins an array.
        /// </summary>
        internal uint ArraySize { get; set; }

        /// <summary>
        /// The associated column.
        /// </summary>
        internal ArmpTableColumn Column { get; set; }

        public ArmpMemberInfo Copy(ArmpTableColumn column = null)
        {
            ArmpMemberInfo copy = new ArmpMemberInfo();
            copy.Type = Type;
            copy.Position = -1;
            copy.ArraySize = ArraySize;
            copy.Column = column;
            return copy;
        }
    }
}
