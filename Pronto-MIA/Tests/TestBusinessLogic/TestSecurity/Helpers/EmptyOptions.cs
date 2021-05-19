namespace Tests.TestBusinessLogic.TestSecurity
{
    using System;
    using Pronto_MIA.BusinessLogic.Security.Interfaces;

    public class EmptyOptions : IHashGeneratorOptions
    {
        public string ToJson()
        {
            throw new NotSupportedException();
        }

        public bool IsSame(IHashGeneratorOptions other)
        {
            throw new NotSupportedException();
        }
    }
}