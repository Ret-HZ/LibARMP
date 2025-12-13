using System;

namespace LibARMP.Exceptions
{
    [Serializable]
    public class InvalidTypeConversionException : InvalidCastException
    {
        private static readonly string message = "Cannot convert from {0} to {1}.";
        public static Type SourceType { get; private set; }
        public static Type TargetType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTypeConversionException"/> class.
        /// </summary>
        internal InvalidTypeConversionException()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTypeConversionException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="inner">Inner exception.</param>
        internal InvalidTypeConversionException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTypeConversionException"/> class.
        /// </summary>
        /// <param name="source">The source <see cref="Type"/>.</param>
        /// <param name="target">The target <see cref="Type"/>.</param>
        public InvalidTypeConversionException(Type source, Type target)
            : base(GetMessage(source, target))
        {
            SourceType = source;
            TargetType = target;
        }


        private static string GetMessage(Type source, Type target)
        {
            return string.Format(message, source, target);
        }
    }
}
