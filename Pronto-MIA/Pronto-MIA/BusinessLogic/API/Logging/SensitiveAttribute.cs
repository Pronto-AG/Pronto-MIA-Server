namespace Pronto_MIA.Logging
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class SensitiveAttribute : Attribute
    {
        public SensitiveGraphQLNoLog(string propertyName)
        {
            
        }
    }
}