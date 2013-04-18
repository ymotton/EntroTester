using System;
using System.Collections.Generic;

namespace EntroTester
{
    public class TypedResult<T> : ExpectedResult<T>
    {
        public TypedResult()
        {
            Any = true;
        }
        public TypedResult(T value)
        {
            Value = value;
        }

        public bool Any { get; set; }
        public T Value { get; set; }

        public override bool IsValid(Type type, object value)
        {
            if (type != typeof(T))
            {
                return false;
            }

            if (!Any)
            {
                bool areEqual = EqualityComparer<T>.Default.Equals((T)value, Value);
                return areEqual;
            }
            return true;
        }
    }
}