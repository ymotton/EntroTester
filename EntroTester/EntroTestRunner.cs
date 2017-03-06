using System;
using System.Linq;
using System.Linq.Expressions;
using EntroBuilder;

namespace EntroTester
{
    public class EntroTestRunner
    {
        public static void Run<T, TResult>(Action<Builder<T>> configureBuilder, Func<T, TResult> when, Expression<Func<T, TResult, bool>> assertionExpression, int count)
        {
            Run(configureBuilder, when, assertionExpression, count, Environment.TickCount);
        }
        public static void Run<T, TResult>(Action<Builder<T>> configureBuilder, Func<T, TResult> when, Expression<Func<T, TResult, bool>> assertionExpression, int count, int seed)
        {
            var builder = Builder.Create<T>(seed);
            configureBuilder(builder);

            var assertion = assertionExpression.Compile();
            var items = builder.Take(count);
            int i = 0;
            foreach (var item in items)
            {
                var result = when(item);
                if (!assertion(item, result))
                {
                    // TODO: Shrink item value to minimal failing case
                    // Only makes sense for enumeration types
                    throw AssertionException.Create(assertionExpression, seed, i, item, result);
                }
                i++;
            }
        }

        public static void Run<T, TExpected, TResult>(Action<Builder<T>> configureBuilder, Func<T, TResult> when, ExpectedResult<TExpected> expectedResult, int count)
        {
            Run(configureBuilder, when, expectedResult, count, Environment.TickCount);
        }
        public static void Run<T, TExpected, TResult>(Action<Builder<T>> configureBuilder, Func<T, TResult> when, ExpectedResult<TExpected> expectedResult, int count, int seed)
        {
            var builder = Builder.Create<T>(seed);
            configureBuilder(builder); 

            var items = builder.Take(count);
            int i = 0;
            foreach (var item in items)
            {
                Type resultType;
                object result;
                try
                {
                    result = when(item);
                    resultType = typeof(TResult);
                }
                catch (Exception e)
                {
                    result = e;
                    resultType = e.GetType();
                }

                if (!expectedResult.IsValid(resultType, result))
                {
                    throw ExpectedResultException.Create(expectedResult, seed, i, item, result);
                }
                i++;
            }
        }

        public static T Replay<T>(Action<Builder<T>> configureBuilder, int seed, int iteration)
        {
            var builder = Builder.Create<T>(seed);
            configureBuilder(builder);
            var items = builder.Take(iteration + 1);
            var last = items.Last();
            return last;
        }
    }
}
