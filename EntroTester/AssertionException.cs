using System;
using System.Linq.Expressions;

namespace EntroTester
{
    public class AssertionException : Exception
    {
        public AssertionException(Expression assertionExpression, int seed, int iteration)
            : base(string.Format("'{0}' assertion failed on seed '{1}' at iteration '{2}'.", assertionExpression, seed, iteration))
        {
        }
    }
}