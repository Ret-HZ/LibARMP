using System;

namespace LibARMP.Exceptions
{
    [Serializable]
    public class ColumnNotFoundException : Exception
    {
        private static readonly string messageId = "No column with ID {0}.";
        private static readonly string messageName = "No column with name '{0}'.";
        private static readonly string messageNone = "No columns found.";

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnNotFoundException"/> class.
        /// </summary>
        public ColumnNotFoundException()
            : base(messageNone)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="inner">Inner exception.</param>
        internal ColumnNotFoundException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnNotFoundException"/> class.
        /// </summary>
        /// <param name="columnId">The column ID.</param>
        public ColumnNotFoundException(uint columnId)
            : base(GetMessage(columnId))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnNotFoundException"/> class.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        public ColumnNotFoundException(string columnName)
            : base(GetMessage(columnName))
        {

        }


        private static string GetMessage(uint columnId)
        {
            return string.Format(messageId, columnId);
        }

        private static string GetMessage(string columnName)
        {
            return string.Format(messageName, columnName);
        }
    }
}
