using System;

namespace LibARMP.Exceptions
{
    [Serializable]
    public class ColumnNoDataException : Exception
    {
        private static readonly string messageId = "The column with ID {0} has no data.";
        private static readonly string messageName = "The column '{0}' has no data.";

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnNoDataException"/> class.
        /// </summary>
        internal ColumnNoDataException()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnNoDataException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="inner">Inner exception.</param>
        internal ColumnNoDataException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnNoDataException"/> class.
        /// </summary>
        /// <param name="columnId">The column ID.</param>
        public ColumnNoDataException(int columnId)
            : base(GetMessage(columnId))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnNoDataException"/> class.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        public ColumnNoDataException(string columnName)
            : base(GetMessage(columnName))
        {

        }


        private static string GetMessage(int columnId)
        {
            return string.Format(messageId, columnId);
        }

        private static string GetMessage(string columnName)
        {
            return string.Format(messageName, columnName);
        }
    }
}
