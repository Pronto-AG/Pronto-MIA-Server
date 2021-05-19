namespace Tests.TestBusinessLogic.TestSecurity
{
    using System;
    using Pronto_MIA.BusinessLogic.Security;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestPasswordHelper
    {
        [Fact]
        public void TestPasswordPolicyToShort()
        {
            var result = PasswordHelper.PasswordPolicyMet("H1-llo", 7);
            Assert.Equal(PasswordHelper.PasswordPolicyError.Lenght, result);
        }

        [Fact]
        public void TestPasswordPolicyNoDigit()
        {
            var result = PasswordHelper.PasswordPolicyMet("He-llo", 4);
            Assert.Equal(PasswordHelper.PasswordPolicyError.Digit, result);
        }

        [Fact]
        public void TestPasswordPolicyNoLowercase()
        {
            var result = PasswordHelper.PasswordPolicyMet("H1-LLO", 4);
            Assert.Equal(PasswordHelper.PasswordPolicyError.Lowercase, result);
        }

        [Fact]
        public void TestPasswordPolicyNoUppercase()
        {
            var result = PasswordHelper.PasswordPolicyMet("h1-llo", 4);
            Assert.Equal(PasswordHelper.PasswordPolicyError.Uppercase, result);
        }

        [Fact]
        public void TestPasswordPolicyNoNonAlphanumeric()
        {
            var result = PasswordHelper.PasswordPolicyMet("H1ello", 4);
            Assert.Equal(
                PasswordHelper.PasswordPolicyError.NonAlphanumeric,
                result);
        }

        [Fact]
        public void TestPasswordPolicyExactLenght()
        {
            var result = PasswordHelper.PasswordPolicyMet("H1-l", 4);
            Assert.Equal(
                PasswordHelper.PasswordPolicyError.None,
                result);
        }
    }
}