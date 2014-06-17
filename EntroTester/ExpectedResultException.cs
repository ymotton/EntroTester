using System;

namespace EntroTester
{
    [Serializable]
    public class ExpectedResultException : Exception
    {
        public ExpectedResultException(string message) : base(message) { }

        public static ExpectedResultException Create<TValue, TExpected>(ExpectedResult<TExpected> expectedResult, int seed, int iteration, TValue value)
        {
            string message = string.Format(
                "Expected result '{0}' on seed '{1}' at iteration '{2}'.\nFailing value:\n{3}\n", 
                expectedResult,
                seed, 
                iteration,
                value.ToJsonOrToString());
            return new ExpectedResultException(message);
        }
    }
}