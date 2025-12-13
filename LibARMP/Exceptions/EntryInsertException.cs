using System;

namespace LibARMP.Exceptions
{
    [Serializable]
    public class EntryInsertException : Exception
    {
        private static readonly string message = "ID {0} is greater than the amount of entries in the table.";

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryInsertException"/> class.
        /// </summary>
        internal EntryInsertException()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryInsertException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="inner">Inner exception.</param>
        internal EntryInsertException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryInsertException"/> class.
        /// </summary>
        /// <param name="columnId">The column ID.</param>
        public EntryInsertException(int columnId)
            : base(GetMessage(columnId))
        {

        }


        private static string GetMessage(int columnId)
        {
            return string.Format(message, columnId);
        }
    }
}
