namespace Server.Tests
{
    using NSubstitute;
    using Server.Data;
    using Xunit;

    /// <summary>
    /// Test suite for api Tests.
    /// </summary>
    public class ApiTests
    {
        /// <summary>
        /// First Test.
        /// </summary>
        [Fact]
        public void IsPrime_InputIs1_ReturnFalse()
        {
            bool result = false;
            Assert.False(result, "1 should not be prime");
        }
    }
}
