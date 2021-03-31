namespace Pronto_MIA.DataAccess
{
    /// <summary>
    /// Enum defining the error codes used within the data access context.
    /// </summary>
    public enum Error
    {
        /// <summary>
        /// Error code representing that the provided password did not meet the
        /// defined password regulations.
        /// </summary>
        PasswordTooWeak,

        /// <summary>
        /// Error code representing that the user which should have been created
        /// already exists and can therefore not be created in a unique manner.
        /// </summary>
        UserAlreadyExists,

        /// <summary>
        /// Error code representing that the user searched for could not be
        /// found.
        /// </summary>
        UserNotFound,

        /// <summary>
        /// Error code representing that the password provided did not match
        /// with the hash stored in the system.
        /// </summary>
        WrongPassword,

        /// <summary>
        /// Error code representing that the current user does not have the
        /// authorization to use the requested operation.
        /// </summary>
        IllegalUserOperation,

        /// <summary>
        /// Error code respresenting that the connection to the database could
        /// not be established.
        /// </summary>
        DatabaseUnavailable,

        /// <summary>
        /// Error code representing that the error which occured could not be
        /// identified.
        /// </summary>
        UnknownError,
    }
}