using System;

namespace EntroTester
{
    public class ExceptionResult<T> : ExpectedResult<T>
        where T : Exception
    {
        public override bool IsValid(Type type, object value)
        {
            bool isExpectedException = type == typeof(T) && value != null;
            return isExpectedException;
        }
    }
}