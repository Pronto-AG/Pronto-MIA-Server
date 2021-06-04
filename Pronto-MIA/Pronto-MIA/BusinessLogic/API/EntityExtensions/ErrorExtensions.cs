namespace Pronto_MIA.BusinessLogic.API.EntityExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using HotChocolate;
    using HotChocolate.Execution;

    /// <summary>
    /// Class defining the extension methods regarding the
    /// <see cref="DataAccess.Error"/> enum used within this application.
    /// </summary>
    public static class ErrorExtensions
    {
        /// <summary>
        /// Converter for a normal Error into a QueryException that can be
        /// understood by the GraphQL Server.
        /// </summary>
        /// <param name="error">Extension method parameter.</param>
        /// <param name="arguments">Additional error specific arguments.</param>
        /// <returns>QueryException that will be returned to the client of
        /// the API.</returns>
        public static QueryException AsQueryException(
            this DataAccess.Error error,
            Dictionary<string, string> arguments = null)
        {
            if (error != DataAccess.Error.PasswordTooWeak || arguments == null)
            {
                return new QueryException(
                    ErrorBuilder
                        .New()
                        .SetExtension("traceId", string.Empty)
                        .SetMessage(error.Message())
                        .SetCode(error.ToString())
                        .Build());
            }

            return PasswordToWeakWithPolicy(error, arguments);
        }

        /// <summary>
        /// Method to retrieve a human readable error message from an error
        /// object.
        /// </summary>
        /// <param name="error">The error for which the message should be
        /// created.</param>
        /// <returns>Human readable error message.</returns>
        /// <exception cref="ArgumentException">If no message exists for the
        /// provided error.</exception>
        [SuppressMessage(
        "Menees.Analyzers",
        "MEN003",
        Justification = "Simple switch-case which creates many lines.")]
        public static string Message(this DataAccess.Error error)
        {
            switch (error)
            {
                case DataAccess.Error.PasswordTooWeak:
                    return "Minimum password requirement not met:" +
                           " Min. {{MinLenght}} characters, 1 lowercase, " +
                           "1 uppercase, 1 number and 1 non alphanumeric";
                case DataAccess.Error.UserAlreadyExists:
                    return "Username already taken";
                case DataAccess.Error.UserNotFound:
                    return "Could not find given user";
                case DataAccess.Error.DepartmentAlreadyExists:
                    return "Name of department already in use";
                case DataAccess.Error.DepartmentNotFound:
                    return "Could not find given department";
                case DataAccess.Error.DepartmentInUse:
                    return "Department cannot be deleted since it still has " +
                           "members.";
                case DataAccess.Error.DeploymentPlanNotFound:
                    return "Could not find given deployment plan";
                case DataAccess.Error.DeploymentPlanImpossibleTime:
                    return "Available from cannot be earlier than available " +
                           "until value";
                case DataAccess.Error.WrongPassword:
                    return "Wrong password for given user";
                case DataAccess.Error.IllegalUserOperation:
                    return "Given user not allowed to perform this action";
                case DataAccess.Error.DatabaseOperationError:
                    return "Database operation failed";
                case DataAccess.Error.FirebaseOperationError:
                    return "Firebase operation failed";
                case DataAccess.Error.FileOperationError:
                    return "A file operation error occured";
                case DataAccess.Error.UnknownError:
                    return "An unknown internal error occured";
                default:
                    throw new ArgumentException(
                        "Unknown data access exception " +
                        $"\"{error.ToString()}\"");
            }
        }

        /// <summary>
        /// Method that creates a PasswordToWeak exception
        /// which includes the policy which was violated
        /// and the policies that have to be met.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="arguments">The arguments to use
        /// in order to include additional information.</param>
        /// <returns>A query exception with additional password
        /// policy information.</returns>
        private static QueryException PasswordToWeakWithPolicy(
            DataAccess.Error error, Dictionary<string, string> arguments)
        {
            var message = error.Message().Replace(
                "{{MinLenght}}",
                arguments["minLenght"]);
            return new QueryException(
                ErrorBuilder
                    .New()
                    .SetExtension("traceId", string.Empty)
                    .SetMessage(message)
                    .SetCode(error.ToString())
                    .SetExtension(
                        "passwordPolicyViolation",
                        arguments["passwordPolicyViolation"])
                    .SetExtension(
                        "minLength",
                        arguments["minLength"])
                    .Build());
        }
    }
}
