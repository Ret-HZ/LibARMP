using System;

namespace LibARMP.Exceptions
{
    [Serializable]
    public class EntryNotFoundException : Exception
    {
        private static readonly string messageId = "No entry with ID {0}.";
        private static readonly string messageName = "No entry with name '{0}'.";
        private static readonly string messageNone = "No entries found.";

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryNotFoundException"/> class.
        /// </summary>
        public EntryNotFoundException()
            : base(messageNone)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="inner">Inner exception.</param>
        internal EntryNotFoundException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryNotFoundException"/> class.
        /// </summary>
        /// <param name="entryId">The entry ID.</param>
        public EntryNotFoundException(uint entryId)
            : base(GetMessage(entryId))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryNotFoundException"/> class.
        /// </summary>
        /// <param name="entryName">The entry name.</param>
        public EntryNotFoundException(string entryName)
            : base(GetMessage(entryName))
        {

        }


        private static string GetMessage(uint entryId)
        {
            return string.Format(messageId, entryId);
        }

        private static string GetMessage(string entryName)
        {
            return string.Format(messageName, entryName);
        }
    }
}
