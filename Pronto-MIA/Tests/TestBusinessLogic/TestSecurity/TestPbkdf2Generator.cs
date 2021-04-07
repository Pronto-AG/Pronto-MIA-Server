namespace Tests.TestBusinessLogic.TestSecurity
{
    using System;
    using Pronto_MIA.BusinessLogic.Security;
    using Xunit;

    public class TestPbkdf2Generator
    {
        private readonly Pbkdf2GeneratorOptions options;
        private readonly string referenceHash;

        public TestPbkdf2Generator()
        {
            this.referenceHash =
                "WDSEKRbLCIcSdEKpX0rtaImLdanMO6lVkHzJv+SLkPb1Ug8A/ECasf9WMuXN" +
                "FNLq2EgbuOMgi/H63dFih+cprjzfRGXXKG2XmLqG0BR8XD30ovf8SX2cTbM8" +
                "gwwMWcpYWtXkm4+mEEomXfG7RcZBttEADPGqhQYix/7FsZiqgzc=";
            var salt = "QUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUE=";
            this.options = new Pbkdf2GeneratorOptions(
                1500,
                128,
                32,
                Convert.FromBase64String(salt));
        }

        [Fact]
        public void TestHashGeneration()
        {
            var generator = new Pbkdf2Generator(this.options);

            var hash = generator.HashPassword("HelloWorld");

            Assert.Equal(
                this.referenceHash,
                Convert.ToBase64String(hash));
        }

        [Fact]
        public void TestHashValidation()
        {
            var generator = new Pbkdf2Generator(this.options);

            var result = generator.ValidatePassword(
                "HelloWorld",
                Convert.FromBase64String(this.referenceHash));

            Assert.True(result);
        }

        [Fact]
        public void TestSaltGeneration()
        {
            var saltSize = 128;
            var optionsWithoutSalt = new Pbkdf2GeneratorOptions(
                1000,
                512,
                saltSize);

            var generator = new Pbkdf2Generator(optionsWithoutSalt);
            var hash = generator.HashPassword("HelloWorld");

            var generatedSalt =
                ((Pbkdf2GeneratorOptions)generator.GetOptions()).Salt;

            Assert.Equal(saltSize, generatedSalt.Length);
        }

        [Fact]
        public void TestInvalidSaltDetection()
        {
            var optionsWithInvalidSalt = new Pbkdf2GeneratorOptions(
                1500,
                512,
                128,
                Convert.FromBase64String("QUFBQUFBQUE="));

            Assert.Throws<ArgumentException>(
                () => new Pbkdf2Generator(optionsWithInvalidSalt));
        }

        [Fact]
        public void TestIdentifier()
        {
            var generator = new Pbkdf2Generator(this.options);
            Assert.Equal(Pbkdf2Generator.Identifier, generator.GetIdentifier());
            Assert.Equal("Pbkdf2Generator", Pbkdf2Generator.Identifier);
        }
    }
}