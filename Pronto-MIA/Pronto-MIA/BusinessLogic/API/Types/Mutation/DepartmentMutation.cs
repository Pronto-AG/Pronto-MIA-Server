#nullable enable
namespace Pronto_MIA.BusinessLogic.API.Types.Mutation
{
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Types;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class representing the mutation operation of graphql.
    /// Contains all mutations that concern <see cref="Department"/>
    /// objects.
    /// </summary>
    [ExtendObjectType(typeof(API.Mutation))]
    public class DepartmentMutation
    {
        /// <summary>
        /// Method to create a department.
        /// </summary>
        /// <param name="departmentManager">The manager managing
        /// the applications departments.</param>
        /// <param name="name">The name of the new department.</param>
        /// <returns>The newly created department.</returns>
        [Authorize(Policy = "CanEditDepartments")]
        public async Task<Department> CreateDepartment(
            [Service] IDepartmentManager departmentManager,
            string name)
        {
            return await departmentManager.Create(name);
        }
    }
}
