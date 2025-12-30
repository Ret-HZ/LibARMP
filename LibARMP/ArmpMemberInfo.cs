using System;

namespace LibARMP
{
    [Serializable]
    internal class ArmpMemberInfo
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


        /// <summary>
        /// Creates a copy of this <see cref="ArmpMemberInfo"/>.
        /// </summary>
        /// <param name="column">The associated column.</param>
        /// <param name="keepPosition">Should the original position be preserved? Default value is <see langword="false"/>.
        /// <para>This value may be overwritten if the structure needs to be packed later on.</para>
        /// <para><b>Not keeping the position will leave it at the default value of -1</b></para></param>
        /// <returns>A copy of this <see cref="ArmpMemberInfo"/>.</returns>
        internal ArmpMemberInfo Copy(ArmpTableColumn column = null, bool keepPosition = false)
        {
            ArmpMemberInfo copy = new ArmpMemberInfo();
            copy.Type = Type;
            copy.Position = keepPosition ? Position : -1;
            copy.ArraySize = ArraySize;
            copy.Column = column;
            return copy;
        }
    }
}
