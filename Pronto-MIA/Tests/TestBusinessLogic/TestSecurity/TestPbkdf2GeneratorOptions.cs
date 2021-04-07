namespace Tests.TestBusinessLogic.TestSecurity
{
    using System;
    using Pronto_MIA.BusinessLogic.Security;
    using Xunit;

    public class TestPbkdf2GeneratorOptions
    {
        [Fact]
        public void TestToJsonConvert()
        {
            var jsonRepresentation =
                "{" +
                    "\"SaltSize\":128," +
                    "\"HashIterations\":1500," +
                    "\"HashSize\":512," +
                    "\"Salt\":null" +
                "}";
            var options = new Pbkdf2GeneratorOptions(
                1500,
                512,
                128);
            var actualJson = options.ToJson();
            Assert.Equal(jsonRepresentation, actualJson);
        }

        [Fact]
        public void TestIsSame()
        {
            var objectOne = new Pbkdf2GeneratorOptions(
                1500,
                512,
                128,
                Convert.FromBase64String("QUJD"));

            var objectTwo = new Pbkdf2GeneratorOptions(
                1500,
                512,
                128,
                Convert.FromBase64String("REVG"));

            Assert.True(objectOne.IsSame(objectTwo));
        }

        [Fact]
        public void TestIsSameNotOk()
        {
            var objectOne = new Pbkdf2GeneratorOptions(
                1500,
                256,
                128,
                Convert.FromBase64String("QUJD"));

            var objectTwo = new Pbkdf2GeneratorOptions(
                1500,
                512,
                128,
                Convert.FromBase64String("QUJD"));

            Assert.False(objectOne.IsSame(objectTwo));
        }

        [Fact]
        public void TestIsSameDifferentClasses()
        {
            var objectOne = new Pbkdf2GeneratorOptions(
                1500,
                256,
                128,
                Convert.FromBase64String("QUJD"));

            var objectTwo = new EmptyOptions();
            Assert.False(objectOne.IsSame(objectTwo));
        }

        [Fact]
        public void TestFromJsonConvert()
        {
            var objectRepresentation = new Pbkdf2GeneratorOptions(
                1500, 512, 128);
            var jsonRepresentation =
                "{" +
                "\"SaltSize\":128," +
                "\"HashIterations\":1500," +
                "\"HashSize\":512," +
                "\"Salt\":null" +
                "}";
            var actualObject = (Pbkdf2GeneratorOptions)Pbkdf2GeneratorOptions
                .FromJson(jsonRepresentation);
            Assert.True(objectRepresentation.IsSame(actualObject));
        }

        [Fact]
        public void TestFromJsonConvertInvalidString()
        {
            var jsonRepresentation = "lasjkdklsa";
            Assert.Throws<ArgumentException>(
                () =>
                    Pbkdf2GeneratorOptions.FromJson(jsonRepresentation));
        }

        [Fact]
        public void TestFromJsonConvertWrongJson()
        {
            var jsonRepresentation = "{\"SaltSize\":\"Hello\"}";
            Assert.Throws<ArgumentException>(
                () =>
                    Pbkdf2GeneratorOptions.FromJson(jsonRepresentation));
        }
    }
}