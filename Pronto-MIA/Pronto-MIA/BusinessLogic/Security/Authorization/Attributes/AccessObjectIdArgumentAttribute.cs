namespace Pronto_MIA.BusinessLogic.Security.Authorization.Attributes
{
    using System;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Interfaces;

    /// <summary>
    /// Attribute used to provide additional information to the authorization
    /// handler. The argument name provided in this argument will be used
    /// by the <see cref="DepartmentAccessAuthorizationHandler"/> in order
    /// to determine a requests department id. If the attribute is not set
    /// department based access will not be granted. If it is set to `IGNORED`
    /// on the other hand the department will not be checked and regardless of
    /// the department access will be granted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class AccessObjectIdArgumentAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AccessObjectIdArgumentAttribute"/> class.
        /// </summary>
        /// <param name="accessObjectIdArgumentName">The name of the
        /// argument containing an access Object implementing
        /// <see cref="IDepartmentComparable"/>. The department id of
        /// that object will then be checked.</param>
        /// <param name="isDepartmentId">If the argument identified
        /// by the name is directly a department id this value has
        /// to be set to `true`.</param>
        public AccessObjectIdArgumentAttribute(
            string accessObjectIdArgumentName, bool isDepartmentId = false)
        {
            this.IsDepartmentId = isDepartmentId;
            this.AccessObjectIdArgumentName = accessObjectIdArgumentName;
        }

        /// <summary>
        /// Gets the name of the argument containing the access object id.
        /// </summary>
        public string AccessObjectIdArgumentName { get; }

        /// <summary>
        /// Gets a value indicating whether the argument does contain a
        /// department id directly.
        /// </summary>
        public bool IsDepartmentId { get; }
    }
}
