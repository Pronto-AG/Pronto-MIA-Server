namespace Tests
{
    using System;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            const bool result = false;
            Assert.False(result, "Result should be false.");
        }

        [Fact]
        public void Test2()
        {
            var deploymentPlan = new DeploymentPlan(
                DateTime.UtcNow.Date, DateTime.UtcNow);
            Assert.Equal(string.Empty, deploymentPlan.FileExtension);
        }
    }
}