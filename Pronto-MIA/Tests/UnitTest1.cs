using System;
using Xunit;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            bool result = false;
            Assert.False(result, "Result should be false.");
        }
    }
}