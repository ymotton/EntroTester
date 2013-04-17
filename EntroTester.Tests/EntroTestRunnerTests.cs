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
                EntroBuilder.Create<Container>()
                            .ForProperty(c => c.Value1, Any.ValueBetween(-100, 100))
                            .ForProperty(c => c.Value2, Any.ValueBetween(-100, 100)), 
                SystemUnderTest.HaveAFailingBranch,
                m => m == true,
                1000000);
        }
        
        [TestMethod]
        public void Replay()
        {
            var faulty = 
                EntroTestRunner.Replay(
                    EntroBuilder.Create<Container>()
                            .ForProperty(c => c.Value1, Any.ValueBetween(-100, 100))
                            .ForProperty(c => c.Value2, Any.ValueBetween(-100, 100)),
                    5889973,
                    40263);

            var result = SystemUnderTest.HaveAFailingBranch(faulty);
            Assert.AreEqual(false, result);
        }
    }

    class Container
    {
        public int Value1 { get; set; }
        public int Value2 { get; set; }
    }
    class SystemUnderTest
    {
        public static bool HaveAFailingBranch(Container container)
        {
            // User would code this function to do something useful
            // But might forget a branch:
            if (container.Value1 == -1 && container.Value2 == 19)
            {
                // We want to detect this case, by random chance
                return false;
            }
            return true;
        }
    }
}
