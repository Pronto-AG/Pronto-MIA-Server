using Pronto_MIA.BusinessLogic.Security.Authorization;
using Pronto_MIA.BusinessLogic.Security.Authorization.Attributes;

#nullable enable
namespace Pronto_MIA.BusinessLogic.API.Types.Query
{
    using System.Linq;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Data;
    using HotChocolate.Types;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class representing the mutation operation of graphql.
    /// Contains all mutations that concern <see cref="Department"/>
    /// objects.
    /// </summary>
    [ExtendObjectType(typeof(API.Query))]
    public class DepartmentQuery
    {
        /// <summary>
        /// Method which retrieves the available deployment plans.
        /// </summary>
        /// <param name="departmentManager">The department manager responsible
        /// for managing application departments.</param>
        /// <returns>Queryable of all available departments.</returns>
        [Authorize(Policy = "ViewDepartment")]
        [AccessObjectIdArgument("IGNORED")]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Department> Departments(
            [Service] IDepartmentManager departmentManager,
            [ApiUserGlobalState] ApiUserState userState)
        {
            if (userState.User.AccessControlList.CanViewDepartments)
            {
                return departmentManager.GetAll();
            }

            return departmentManager.GetAll().Where(
                d => d.Id == userState.User.DepartmentId);
        }
    }
}
