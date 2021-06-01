namespace Pronto_MIA.BusinessLogic.Security.Authorization.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class AccessObjectIdArgumentAttribute : Attribute
    {
        public AccessObjectIdArgumentAttribute(
            string accessObjectIdArgumentName, bool isDepartmentId = false)
        {
            this.IsDepartmentId = isDepartmentId;
            this.AccessObjectIdArgumentName = accessObjectIdArgumentName;
        }

        public string AccessObjectIdArgumentName { get; }
        
        public bool IsDepartmentId { get;  }
    }
}
