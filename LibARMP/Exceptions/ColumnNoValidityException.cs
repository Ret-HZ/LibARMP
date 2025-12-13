using System;

namespace LibARMP.Exceptions
{
    [Serializable]
    public class ColumnNoValidityException : Exception
    {
        private static readonly string message = "The table has no column validity.";

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnNoValidityException"/> class.
        /// </summary>
        public ColumnNoValidityException()
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnNoValidityException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="inner">Inner exception.</param>
        internal ColumnNoValidityException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
