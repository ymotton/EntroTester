using System;
namespace EntroTester
{
    public class Returns
    {
        public static ExpectedResult<T> Any<T>()
        {
            return new TypedResult<T>();
        }
        public static ExpectedResult<T> Exactly<T>(T value)
        {
            return new TypedResult<T>(value);
        }
        public static ExpectedResult<T> Throws<T>() where T : Exception
        {
            return new ExceptionResult<T>();
        }
    }
}
