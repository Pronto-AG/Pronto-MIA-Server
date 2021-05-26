namespace Pronto_MIA.BusinessLogic.Security.Authorization
{
    using Microsoft.AspNetCore.Authorization;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class representing a requirement bound to a
    /// <see cref="AccessControl"/> entry.
    /// </summary>
    public class AccessControlListRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AccessControlListRequirement"/> class.
        /// </summary>
        /// <param name="control">The control that should be checked for the
        /// user in order to fulfill this requirement.</param>
        public AccessControlListRequirement(
            AccessControl control)
        {
            this.Control = control;
        }

        /// <summary>
        /// Gets the control associated with this Requirement.
        /// </summary>
        public AccessControl Control { get; }
    }
}
