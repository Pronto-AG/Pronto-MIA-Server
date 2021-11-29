namespace Pronto_MIA.BusinessLogic.Security.Authorization
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authorization;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Requirement used for department based authorization. May be
    /// extended in the future to support more authorization types.
    /// </summary>
    public class AccessObjectRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AccessObjectRequirement"/> class.
        /// </summary>
        /// <param name="objectType">The type of the object to be possibly
        /// checked against the users department. Must implement
        /// <see cref="IDepartmentComparable"/> as the department id of the
        /// object has to be determined.</param>
        /// <param name="controls">The controls of this requirement. Those can
        /// be either without restriction or department bound.</param>
        public AccessObjectRequirement(
            Type objectType,
            Dictionary<AccessControl, AccessMode> controls)
        {
            /*if (!typeof(IDepartmentComparable).IsAssignableFrom(objectType))
            {
                throw new ArgumentException(
                    "Type needs to implement IDepartmentComparable.");
            }*/

            this.ObjectType = objectType;
            this.Controls = controls;
        }

        /// <summary>
        /// Gets the controls associated with this Requirement.
        /// </summary>
        public Dictionary<AccessControl, AccessMode> Controls { get; }

        /// <summary>
        /// Gets the type of the underlying access object.
        /// </summary>
        public Type ObjectType { get; }
    }
}
