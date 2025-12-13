using System;

namespace LibARMP.Exceptions
{
    [Serializable]
    public class TypeNotSupportedException : Exception
    {
        private static readonly string message = "The armp format does not support the type '{0}'.";

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeNotSupportedException"/> class.
        /// </summary>
        /// <param name="type">The unsupported <see cref="Type"/>.</param>
        public TypeNotSupportedException(Type type)
            : base(GetMessage(type))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeNotSupportedException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="inner">Inner exception.</param>
        internal TypeNotSupportedException(string message, Exception inner)
            : base(message, inner)
        {

        }


        private static string GetMessage(Type type)
        {
            return string.Format(message, type.Name);
        }
    }
}
