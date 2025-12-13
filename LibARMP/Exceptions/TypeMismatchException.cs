using System;

namespace LibARMP.Exceptions
{
    [Serializable]
    public class TypeMismatchException : Exception
    {
        private static readonly string message = "Type mismatch. Expected {0} and got {1}.";
        public static Type ExpectedType { get; private set; }
        public static Type ActualType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeMismatchException"/> class.
        /// </summary>
        internal TypeMismatchException()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeMismatchException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="inner">Inner exception.</param>
        internal TypeMismatchException(string message, Exception inner)
            : base(message, inner)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeMismatchException"/> class.
        /// </summary>
        /// <param name="expected">The expected <see cref="Type"/>.</param>
        /// <param name="actual">The actual <see cref="Type"/>.</param>
        public TypeMismatchException(Type expected, Type actual)
            : base(GetMessage(expected, actual))
        {
            ExpectedType = expected;
            ActualType = actual;
        }


        private static string GetMessage(Type expected, Type actual)
        {
            return string.Format(message, expected, actual);
        }
    }
}
