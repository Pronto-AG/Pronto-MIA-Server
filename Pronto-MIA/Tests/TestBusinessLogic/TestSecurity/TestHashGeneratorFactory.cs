namespace Tests.TestBusinessLogic.TestSecurity
{
    using System;
    using Pronto_MIA.BusinessLogic.Security;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestHashGeneratorFactory
    {
        [Fact]
        public void TestPbkdf2Creation()
        {
            var user = new User(
                "Fritz",
                new byte[2],
                Pbkdf2Generator.Identifier);
            var generator = HashGeneratorFactory.GetGeneratorForUser(user);

            Assert.IsType<Pbkdf2Generator>(generator);
        }

        [Fact]
        public void TestUnknownGenerator()
        {
            var user = new User(
                "Fritz",
                new byte[2],
                "Bcrypt");

            Assert.Throws<ArgumentException>(() =>
                HashGeneratorFactory.GetGeneratorForUser(user));
        }
    }
}