namespace Pronto_MIA.BusinessLogic.API.EntityExtensions
{
    using System;
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
        /// <returns>QueryException that will be returned to the client of
        /// the API.</returns>
        public static QueryException AsQueryException(
            this DataAccess.Error error)
        {
            return new QueryException(
                ErrorBuilder
                    .New()
                    .SetMessage(error.Message())
                    .SetCode(error.ToString())
                    .Build());
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
        public static string Message(this DataAccess.Error error)
        {
            switch (error)
            {
                case DataAccess.Error.PasswordTooWeak:
                    return "Minimum password requirement not met:" +
                           " Min. 10 characters, 1 uppercase, 1 number";
                case DataAccess.Error.UserAlreadyExists:
                    return "Username already taken";
                case DataAccess.Error.UserNotFound:
                    return "Could not find given user";
                case DataAccess.Error.DeploymentPlanNotFound:
                    return "Could not find given deployment plan";
                case DataAccess.Error.WrongPassword:
                    return "Wrong password for given user";
                case DataAccess.Error.IllegalUserOperation:
                    return "Given user not allowed to perform this action";
                case DataAccess.Error.DatabaseUnavailable:
                    return "Connection to database failed";
                case DataAccess.Error.FileOperationError:
                    return "A file operation error occured";
                case DataAccess.Error.UnknownError:
                    return "An unknown internal error occured";
                default: throw new ArgumentException(
                    "Unhandled data access exception");
            }
        }
    }
}
