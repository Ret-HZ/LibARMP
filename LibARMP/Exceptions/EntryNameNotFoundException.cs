using System;

namespace LibARMP.Exceptions
{
    [Serializable]
    public class EntryNameNotFoundException : Exception
    {
        private static readonly string message = "No entry names found.";

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryNameNotFoundException"/> class.
        /// </summary>
        public EntryNameNotFoundException()
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryNameNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="inner">Inner exception.</param>
        internal EntryNameNotFoundException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
