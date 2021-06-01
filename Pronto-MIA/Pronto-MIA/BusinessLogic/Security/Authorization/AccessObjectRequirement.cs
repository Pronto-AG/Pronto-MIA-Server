using System.Collections;

namespace Pronto_MIA.BusinessLogic.Security.Authorization
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authorization;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Interfaces;
    using Pronto_MIA.Domain.Entities;

    public class AccessObjectRequirement : IAuthorizationRequirement
    {
        public AccessObjectRequirement(
            Type objectType,
            Dictionary<AccessControl, AccessMode> controls)
        {
            if (!typeof(IDepartmentComparable).IsAssignableFrom(objectType))
            {
                throw new ArgumentException(
                    "Type needs to implement IDepartmentComparable.");
            }

            this.ObjectType = objectType;
            this.Controls = controls;
        }

        /// <summary>
        /// Gets the controls associated with this Requirement.
        /// </summary>
        public Dictionary<AccessControl, AccessMode> Controls { get; }

        public Type ObjectType { get; }
    }
}
