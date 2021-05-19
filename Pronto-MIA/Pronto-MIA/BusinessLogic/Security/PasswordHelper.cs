namespace Pronto_MIA.BusinessLogic.Security
{
    using System.Collections.Generic;

    /// <summary>
    /// Class containing functions intended to
    /// assist in password handling.
    /// </summary>
    public static class PasswordHelper
    {
        private static int arraySize = 4;
        private static int containsDigitIndex = 0;
        private static int containsLowercaseIndex = 1;
        private static int containsUppercaseIndex = 2;
        private static int containsNonAlphanumericIndex = 3;

        /// <summary>
        /// Enum representing an error which can occur whilst
        /// checking a password for policy compliance.
        /// </summary>
        public enum PasswordPolicyViolation
        {
            /// <summary>
            /// The password is to short.
            /// </summary>
            Lenght,

            /// <summary>
            /// The password does not contain a non alphanumeric
            /// character.
            /// </summary>
            NonAlphanumeric,

            /// <summary>
            /// The password does not contain a uppercase
            /// character.
            /// </summary>
            Uppercase,

            /// <summary>
            /// The password does not contain a lowercase
            /// character.
            /// </summary>
            Lowercase,

            /// <summary>
            /// The password does not contain a digit.
            /// </summary>
            Digit,

            /// <summary>
            /// The password provided is compliant with
            /// the password policy.
            /// </summary>
            None,
        }

        /// <summary>
        /// Checks if the currently active password policy ist met.
        /// Whilst the password lenght can be defined by the config
        /// the password must contain a digit, a lowercase character,
        /// a uppercase character and a non alphanumeric character.
        /// </summary>
        /// <param name="password">The password to check for policy
        /// compliance.</param>
        /// <param name="minLenght">The minimal lenght the password should have.
        /// </param>
        /// <returns>The first violation of the password policy that
        /// occured or <see cref="PasswordPolicyViolation.None"/> if no
        /// violation occured.</returns>
        public static PasswordPolicyViolation PasswordPolicyMet(
            string password, int minLenght)
        {
            if (password.Length < minLenght)
            {
                return PasswordPolicyViolation.Lenght;
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

        private static PasswordPolicyViolation CheckState(
            IReadOnlyList<bool> state)
        {
            if (!state[containsDigitIndex])
            {
                return PasswordPolicyViolation.Digit;
            }
            else if (!state[containsLowercaseIndex])
            {
                return PasswordPolicyViolation.Lowercase;
            }
            else if (!state[containsUppercaseIndex])
            {
                return PasswordPolicyViolation.Uppercase;
            }
            else if (!state[containsNonAlphanumericIndex])
            {
                return PasswordPolicyViolation.NonAlphanumeric;
            }

            return PasswordPolicyViolation.None;
        }
    }
}
