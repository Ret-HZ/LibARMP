using System;

namespace LibARMP.Exceptions
{
    [Serializable]
    public class ColumnNoIndexException : Exception
    {
        private static readonly string message = "The table has no column indices.";

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnNoIndexException"/> class.
        /// </summary>
        public ColumnNoIndexException()
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnNoIndexException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="inner">Inner exception.</param>
        internal ColumnNoIndexException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
