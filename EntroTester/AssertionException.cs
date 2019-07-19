using System;
using System.Linq.Expressions;
using EntroTester.ObjectDumper;

namespace EntroTester
{
    public class AssertionException : Exception
    {
        public AssertionException(string message) : base(message) { }

        public static AssertionException Create<T, TResult>(Expression assertionExpression, int seed, int iteration, T value, TResult result)
        {
            var message = string.Format(
                "'{0}' assertion failed on seed '{1}' at iteration '{2}'.\nFailing value:\n{3}\nFailing result:\n{4}\n", 
                assertionExpression,
                seed, 
                iteration,
                value.DumpToString("value"),
                result.DumpToString("result"));
            return new AssertionException(message);
        }
    }
}