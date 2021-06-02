#nullable enable
namespace Pronto_MIA.BusinessLogic.Security.Authorization.Interfaces
{
    using System;

    public interface IAttributeProvider
    {
        /// <summary>
        /// Gets the attribute of given type with
        /// given inheritance level.
        /// </summary>
        /// <param name="attributeType">The type of attribute to get.</param>
        /// <param name="inherit">If the attribute may be inherited.</param>
        /// <returns>The attribute or null if no attribute was found.</returns>
        public Attribute? GetCustomAttribute(Type attributeType, bool inherit);
    }
}
