namespace Tests
{
    using Pronto_MIA.Data;
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
            var speak = new Speaker
            {
                Name = "Dani",
            };
            Assert.Equal("Dani", speak.Name);
        }
    }
}