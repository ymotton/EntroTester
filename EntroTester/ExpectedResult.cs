using System;

namespace EntroTester
{
    public abstract class ExpectedResult<T>
    {
        public abstract bool IsValid(Type type, object value);
    }
}