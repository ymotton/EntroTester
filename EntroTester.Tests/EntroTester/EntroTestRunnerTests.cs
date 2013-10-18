using System;
using EntroBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntroTester.Tests
{
    [TestClass]
    public class EntroTestRunnerTests
    {
        [TestMethod]
        [ExpectedException(typeof(AssertionException))]
        public void Run()
        {
            EntroTestRunner.Run(
                Builder.Create<ParameterlessCtorTuple<int, int>>()
                       .Property(c => c.Item1, Any.ValueBetween(-100, 100))
                       .Property(c => c.Item2, Any.ValueBetween(-100, 100)), 
                SystemUnderTest.HaveAFailingBranch,
                m => m == true,
                1000000);
        }
        
        [TestMethod]
        public void Replay()
        {
            var faulty = 
                EntroTestRunner.Replay(
                    Builder.Create<ParameterlessCtorTuple<int, int>>()
                           .Property(c => c.Item1, Any.ValueBetween(-100, 100))
                           .Property(c => c.Item2, Any.ValueBetween(-100, 100)),
                    5889973,
                    10859);

            var result = SystemUnderTest.HaveAFailingBranch(faulty);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ExpectedResultException<int>))]
        public void Run2()
        {
            EntroTestRunner.Run(
                Builder.Create<ParameterlessCtorTuple<int, int>>()
                       .Property(c => c.Item1, Any.ValueBetween(-100, 100))
                       .Property(c => c.Item2, Any.ValueBetween(-100, 100)),
                SystemUnderTest.DividesByZero,
                Returns.Any<int>(),
                1000000);
        }

        [TestMethod]
        [ExpectedException(typeof(AssertionException))]
        public void Run3()
        {
            EntroTestRunner.Run(
                Builder.Create<ParameterlessCtorTuple<string>>()
                       .Property(c => c.Item1, Any.ValueLike(@"(( |\t){1,2}[a-zA-Z0-9]{1,2}){1,2}")),
                SystemUnderTest.ParsesWord,
                r => r != null && r.Length > 0,
                10000);
        }

        class ParameterlessCtorTuple<T1>
        {
            public T1 Item1 { get; set; }
        }
        class ParameterlessCtorTuple<T1, T2>
        {
            public T1 Item1 { get; set; }
            public T2 Item2 { get; set; }
        }
        class ParameterlessCtorTuple<T1, T2, T3>
        {
            public T1 Item1 { get; set; }
            public T2 Item2 { get; set; }
            public T3 Item3 { get; set; }
        }

        class SystemUnderTest
        {
            public static bool HaveAFailingBranch(ParameterlessCtorTuple<int, int> argument)
            {
                // User would code this function to do something useful
                // But might forget a branch:
                if (argument.Item1 == -1 && argument.Item2 == 19)
                {
                    // We want to detect this case, by random chance
                    return false;
                }
                return true;
            }

            public static int DividesByZero(ParameterlessCtorTuple<int, int> argument)
            {
                int result = argument.Item1 / argument.Item2;
                return result;
            }

            public static string ParsesWord(ParameterlessCtorTuple<string> argument)
            {
                string firstWord = argument.Item1.Split(' ')[0];
                return firstWord;
            }
        }
    }
}
