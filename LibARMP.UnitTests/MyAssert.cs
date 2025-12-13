using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibARMP.UnitTests
{
    public static class MyAssert
    {
        public static void Throws<T>( Action func ) where T : Exception
        {
            var exceptionThrown = false;
            try
            {
                func.Invoke();
            }
            catch ( T )
            {
                exceptionThrown = true;
            }

            if ( !exceptionThrown )
            {
                throw new AssertFailedException(
                    String.Format("An exception of type {0} was expected, but not thrown", typeof(T))
                    );
            }
        }
    }
}
