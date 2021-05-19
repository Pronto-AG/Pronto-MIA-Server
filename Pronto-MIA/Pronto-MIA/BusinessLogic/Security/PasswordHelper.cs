namespace Pronto_MIA.BusinessLogic.Security
{
    using System.Collections.Generic;

    public static class PasswordHelper
    {
        private static int arraySize = 4;
        private static int containsDigitIndex = 0;
        private static int containsLowercaseIndex = 1;
        private static int containsUppercaseIndex = 2;
        private static int containsNonAlphanumericIndex = 3;

        public enum PasswordPolicyError
        {
            Lenght,
            NonAlphanumeric,
            Uppercase,
            Lowercase,
            Digit,
            None
        }

        /// <summary>
        /// Checks if the currently active password policy ist met.
        /// Whilst the password lenght can be defined by the config
        /// the password must contain a digit, a lowercase character,
        /// a uppercase character and a non alphanumeric character.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="minLenght"></param>
        /// <returns></returns>
        public static PasswordPolicyError PasswordPolicyMet(
            string password, int minLenght)
        {
            if (password.Length < minLenght)
            {
                return PasswordPolicyError.Lenght;
            }

            var state = new bool[arraySize];
            state = SetState(password, state);

            return CheckState(state);
        }

        private static bool[] SetState(string password, bool[] state)
        {
            state[containsDigitIndex] = false;
            state[containsLowercaseIndex] = false;
            state[containsUppercaseIndex] = false;
            state[containsNonAlphanumericIndex] = false;

            foreach (char c in password)
            {
                if (char.IsDigit(c))
                {
                    state[containsDigitIndex] = true;
                }
                else if (char.IsLower(c))
                {
                    state[containsLowercaseIndex] = true;
                }
                else if (char.IsUpper(c))
                {
                    state[containsUppercaseIndex] = true;
                }
                else if (!char.IsLetterOrDigit(c))
                {
                    state[containsNonAlphanumericIndex] = true;
                }
            }

            return state;
        }

        private static PasswordPolicyError CheckState(IReadOnlyList<bool> state)
        {
            if (!state[containsDigitIndex])
            {
                return PasswordPolicyError.Digit;
            }
            else if (!state[containsLowercaseIndex])
            {
                return PasswordPolicyError.Lowercase;
            }
            else if (!state[containsUppercaseIndex])
            {
                return PasswordPolicyError.Uppercase;
            }
            else if (!state[containsNonAlphanumericIndex])
            {
                return PasswordPolicyError.NonAlphanumeric;
            }

            return PasswordPolicyError.None;
        }
    }
}
