﻿using System;
using System.Collections.Generic;
using System.Linq;
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
                (Builder<ParameterlessCtorTuple<int, int>> b) =>
                       b.Property(c => c.Item1, Any.ValueBetween(-100, 100))
                        .Property(c => c.Item2, Any.ValueBetween(-100, 100)), 
                SystemUnderTest.HaveAFailingBranch,
                (i, m) => m == true,
                1000000);
        }
        
        [TestMethod]
        public void Replay()
        {
            var faulty = 
                EntroTestRunner.Replay(
                    (Builder<ParameterlessCtorTuple<int, int>> b) =>
                           b.Property(c => c.Item1, Any.ValueBetween(-100, 100))
                            .Property(c => c.Item2, Any.ValueBetween(-100, 100)),
                    55768296,
                    199627);

            var result = SystemUnderTest.HaveAFailingBranch(faulty);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ExpectedResultException))]
        public void Run2()
        {
            EntroTestRunner.Run(
                (Builder<ParameterlessCtorTuple<int, int>> b) =>
                    b.Property(c => c.Item1, Any.ValueBetween(-100, 100))
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
                (Builder<ParameterlessCtorTuple<string>> b) =>
                    b.Property(c => c.Item1, Any.ValueLike(@"(( |\t){1,2}[a-zA-Z0-9]{1,2}){1,2}")),
                SystemUnderTest.ParsesWord,
                (t, r) => r != null && r.Length > 0,
                10000);
        }

        [TestMethod]
        [ExpectedException(typeof(AssertionException))]
        public void Run4()
        {
            EntroTestRunner.Run(
                (Builder<List<int>> b) => { },
                SystemUnderTest.Squares,
                (ts, rs) => rs.Zip(ts, (r, t) => (Math.Sqrt(r.Square) - t) < 1e-5).All(x => x),
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

            public class Container
            {
                public int Square { get; set; }
            }

            public static List<Container> Squares(List<int> values)
            {
                return values.Select(x => new Container {Square = x*x}).ToList();
            }
        }
    }
}
