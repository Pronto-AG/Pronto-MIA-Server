namespace Tests.TestBusinessLogic.TestSecurity.TestAuthorization
{
    using System;
    using System.Collections.Generic;
    using Pronto_MIA.BusinessLogic.Security.Authorization;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestAccessObjectRequirement
    {
        [Fact]
        public void TestIDepartmentComparableNoError()
        {
            var requirement = new AccessObjectRequirement(
                typeof(Department),
                new Dictionary<AccessControl, AccessMode>());

            Assert.NotNull(requirement);
        }
    }
}