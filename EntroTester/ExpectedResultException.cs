using System;

namespace EntroTester
{
    public class ExpectedResultException<T> : Exception
    {
        public ExpectedResultException(ExpectedResult<T> expectedResult, int seed, int iteration)
            : base(string.Format("Expected result '{0}' on seed '{1}' at iteration '{2}'.", expectedResult, seed, iteration))
        {
        }
    }
}