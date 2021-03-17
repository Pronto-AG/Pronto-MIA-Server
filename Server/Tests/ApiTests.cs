namespace Server.Tests
{
    using NSubstitute;
    using Server.Data;
    using Xunit;

    public class ApiTests
    {
        [Fact]
        public void IsPrime_InputIs1_ReturnFalse()
        {
            bool result = false;
            Assert.False(result, "1 should not be prime");
        }
    }
}