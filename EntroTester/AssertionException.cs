using System;
using System.Linq.Expressions;

namespace EntroTester
{
    [Serializable]
    public class AssertionException : Exception
    {
        public AssertionException(string message) : base(message) { }

        public static AssertionException Create<T>(Expression assertionExpression, int seed, int iteration, T value)
        {
            var message = string.Format(
                "'{0}' assertion failed on seed '{1}' at iteration '{2}'.\nFailing value:\n{3}\n", 
                assertionExpression,
                seed, 
                iteration,
                value.ToJsonOrToString());
            return new AssertionException(message);
        }
    }
}