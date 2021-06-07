namespace Tests.TestBusinessLogic.TestAPI.TestLogging
{
    using System.Collections.Generic;
    using Pronto_MIA.BusinessLogic.API.Logging;
    using Xunit;

    public class TestQueryLoggerFormatter
    {
        [Fact]
        public void TestFormatQueryWithoutVariables()
        {
            var result = QueryLoggerFormatter.FormatQuery(
                "Test Query", new List<QueryVariable>(), double.MaxValue);

            Assert.Contains("Test Query", result);
            Assert.DoesNotContain("Variables:", result);
        }

        [Fact]
        public void TestFormatQuerySingleVariable()
        {
            var variables = new List<QueryVariable>()
            {
                new ()
                {
                    Name = "PW",
                    Value = "SecretString",
                    Type = "PasswordType",
                },
            };

            var result = QueryLoggerFormatter.FormatQuery(
                "Test Query", variables, double.MaxValue);

            Assert.Contains("Test Query", result);
            Assert.Contains("PW", result);
            Assert.Contains("SecretString", result);
            Assert.Contains("PasswordType", result);
        }

        [Fact]
        public void TestFormatQueryMultipleVariables()
        {
            var variables = new List<QueryVariable>()
            {
                new ()
                {
                    Name = "PW",
                    Value = "SecretString",
                    Type = "PasswordType",
                },
                new ()
                {
                    Name = "Name",
                    Value = "UserName",
                    Type = "NameType",
                },
            };

            var result = QueryLoggerFormatter.FormatQuery(
                "Test Query", variables, double.MaxValue);

            Assert.Contains("Test Query", result);
            Assert.Contains("Name", result);
            Assert.Contains("UserName", result);
            Assert.Contains("NameType", result);
            Assert.Contains("PW", result);
            Assert.Contains("SecretString", result);
            Assert.Contains("PasswordType", result);
        }

        [Fact]
        public void TestNullVariable()
        {
            var variables = new List<QueryVariable>()
            {
                new ()
                {
                    Name = null,
                    Value = null,
                    Type = null,
                },
            };

            var result = QueryLoggerFormatter.FormatQuery(
                "Test Query", variables, double.MaxValue);

            Assert.Contains("Test Query", result);
            Assert.DoesNotContain("null", result);
        }

        [Fact]
        public void TestLongVariable()
        {
            var variables = new List<QueryVariable>()
            {
                new QueryVariable()
                {
                    Name = "TestLong",
                    Value = "ExtremelyLongValueWhichShouldDefinitelyNotBe",
                    Type = "VeryLongType",
                },
            };

            var result = QueryLoggerFormatter.FormatQuery(
                "Test Query", variables, double.MaxValue);

            Assert.Contains("Test Query", result);
            Assert.Contains("TestLong", result);
            Assert.Contains(
                "ExtremelyLongValueWhichShouldDefinitelyNotBe"[..20],
                result);
            Assert.Contains("VeryLongType", result);
        }
    }
}