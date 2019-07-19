using System;
using EntroTester.ObjectDumper;

namespace EntroTester
{
    public class ExpectedResultException : Exception
    {
        public ExpectedResultException(string message) : base(message) { }

        public static ExpectedResultException Create<TValue, TExpected>(ExpectedResult<TExpected> expectedResult, int seed, int iteration, TValue value, object result)
        {
            string message = string.Format(
                "Expected result '{0}' on seed '{1}' at iteration '{2}'.\nFailing value:\n{3}\nFailing result:\n{4}\n", 
                expectedResult,
                seed, 
                iteration,
                value.DumpToString("value"),
                result.DumpToString("result"));
            return new ExpectedResultException(message);
        }
    }
}