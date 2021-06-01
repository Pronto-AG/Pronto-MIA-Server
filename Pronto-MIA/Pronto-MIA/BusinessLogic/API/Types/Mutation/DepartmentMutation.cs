#nullable enable
namespace Pronto_MIA.BusinessLogic.API.Types.Mutation
{
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Attributes;
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
        /// <exception cref="QueryException">Returns DepartmentAlreadyExists
        /// exception if the department with the given name already exists.
        /// </exception>
        [Authorize(Policy = "EditDepartment")]
        public async Task<Department> CreateDepartment(
            [Service] IDepartmentManager departmentManager,
            string name)
        {
            return await departmentManager.Create(name);
        }

        /// <summary>
        /// Method to update a department.
        /// </summary>
        /// <param name="departmentManager">The manager managing
        /// the applications departments.</param>
        /// <param name="id">The id of the department.</param>
        /// <param name="name">The new name of the department.</param>
        /// <returns>The updated department.</returns>
        /// <exception cref="QueryException">Returns DepartmentNotFound
        /// exception if the department with the given id could not be found.
        /// </exception>
        [Authorize(Policy = "EditDepartment")]
        [AccessObjectIdArgument("id")]
        public async Task<Department> UpdateDepartment(
            [Service] IDepartmentManager departmentManager,
            int id,
            string name)
        {
            return await departmentManager.Update(id, name);
        }

        /// <summary>
        /// Method to remove a department.
        /// </summary>
        /// <param name="departmentManager">The manager managing
        /// the applications departments.</param>
        /// <param name="id">The id of the department.</param>
        /// <returns>The id of the removed department.</returns>
        /// <exception cref="QueryException">Returns DepartmentNotFound
        /// exception if the department with the given id could not be found.
        /// </exception>
        [Authorize(Policy = "EditDepartment")]
        [AccessObjectIdArgument("id")]
        public async Task<int> RemoveDepartment(
            [Service] IDepartmentManager departmentManager,
            int id)
        {
            return await departmentManager.Remove(id);
        }
    }
}
