#nullable enable
namespace Pronto_MIA.BusinessLogic.API.Types.Mutation
{
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Pronto_MIA.BusinessLogic.API.Logging;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    [ExtendObjectType(typeof(API.Mutation))]
    public class DepartmentMutation
    {
        [Authorize(Policy = "CanEditDepartments")]
        public async Task<Department> CreateDepartment(
            [Service] IDepartmentManager departmentManager,
            string name)
        {
            return await departmentManager.Create(name);
        }
    }
}
