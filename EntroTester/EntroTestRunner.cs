using System;
using System.Linq;
using System.Linq.Expressions;

namespace EntroTester
{
    public class EntroTestRunner
    {
        public static void Run<T, TResult>(EntroBuilder<T> builder, Func<T, TResult> when, Expression<Func<TResult, bool>> assertionExpression, int count)
            where T : class, new()
        {
            int seed = Environment.TickCount;
            Run(builder, when, assertionExpression, count, seed);
        }
        public static void Run<T, TResult>(EntroBuilder<T> builder, Func<T, TResult> when, Expression<Func<TResult, bool>> assertionExpression, int count, int seed)
            where T : class, new()
        {
            var assertion = assertionExpression.Compile();
            var items = builder.Take(count, seed);
            int i = 0;
            foreach (var item in items)
            {
                var result = when(item);
                if (!assertion(result))
                {
                    throw new AssertionException(assertionExpression, seed, i);
                }
                i++;
            }
        }

        public static void Run<T, TResult, TExpected>(EntroBuilder<T> builder, Func<T, TResult> when, ExpectedResult<TExpected> expectedResult, int count)
            where T : class, new()
        {
            int seed = Environment.TickCount;
            Run(builder, when, expectedResult, count, seed);
        }
        public static void Run<T, TResult, TExpected>(EntroBuilder<T> builder, Func<T, TResult> when, ExpectedResult<TExpected> expectedResult, int count, int seed)
            where T : class, new()
        {
            var items = builder.Take(count, seed);
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
                    throw new ExpectedResultException<TExpected>(expectedResult, seed, i);
                }
                i++;
            }
        }

        public static T Replay<T>(EntroBuilder<T> builder, int seed, int iteration)
            where T : class, new()
        {
            var items = builder.Take(iteration + 1, seed);
            var last = items.Last();
            return last;
        }
    }
}
