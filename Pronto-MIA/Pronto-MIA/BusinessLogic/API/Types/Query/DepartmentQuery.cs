#nullable enable
namespace Pronto_MIA.BusinessLogic.API.Types.Query
{
    using System.Linq;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Data;
    using HotChocolate.Types;
    using Pronto_MIA.BusinessLogic.Security.Authorization.Attributes;
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
        /// Method which retrieves the available departments. Depending
        /// on the requesting users access rights only a fraction of the
        /// available departments might be returned.
        /// </summary>
        /// <param name="userManager">The manager responsible
        /// for managing application users.</param>
        /// <param name="departmentManager">The department manager responsible
        /// for managing application departments.</param>
        /// <param name="userState">Provides information about the user
        /// requesting this endpoint.</param>
        /// <returns>Queryable of all departments available to the user.
        /// </returns>
        [Authorize(Policy = "ViewDepartment")]
        [AccessObjectIdArgument("IGNORED")]
        [UseFiltering]
        [UseSorting]
        public async Task<IQueryable<Department>> Departments(
            [Service] IUserManager userManager,
            [Service] IDepartmentManager departmentManager,
            [ApiUserGlobalState] ApiUserState userState)
        {
            var user = await userManager.GetById(userState.UserId);
            if (user.AccessControlList.CanViewDepartments)
            {
                return departmentManager.GetAll();
            }

            return departmentManager.GetAll().Where(
                d => d.Id == user.DepartmentId);
        }
    }
}
