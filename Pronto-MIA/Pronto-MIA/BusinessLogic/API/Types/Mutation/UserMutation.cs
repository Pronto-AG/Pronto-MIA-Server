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
    public class UserMutation
    {
        /// <summary>
        /// Creates a new User.
        /// </summary>
        /// <param name="userManager">The user manager responsible for
        /// managing application users.</param>
        /// <param name="aclManager">The manager responsible for managing
        /// the access control lists linked to the users.</param>
        /// <param name="departmentManager">The manager responsible for managing
        /// the departments linked to the users.</param>
        /// <param name="userName">The username of the new user.</param>
        /// <param name="password">The password of the new user.</param>
        /// <param name="accessControlList">The access control list
        /// that should be linked to the new user.</param>
        /// <param name="departmentId">The id of the department the user
        /// should be added to.</param>
        /// <returns>The newly created user.</returns>
        /// <exception cref="QueryException">Returns UserAlreadyExists exception
        /// if the username already exists. Alternatively returns
        /// PasswordTooWeak exception if the provided password does not meet the
        /// policy requirements.
        /// </exception>
        [Authorize(Policy = "CanEditUsers")]
        [Authorize(Policy = "CanEditDepartments")]
        public async Task<User> CreateUser(
            [Service] IUserManager userManager,
            [Service] IAccessControlListManager aclManager,
            [Service] IDepartmentManager departmentManager,
            string userName,
            string password,
            AccessControlList accessControlList,
            int departmentId)
        {
            var user = await userManager.Create(userName, password);
            await departmentManager.AddUser(departmentId, user);
            await aclManager.LinkAccessControlList(user.Id, accessControlList);
            return user;
        }

        /// <summary>
        /// Method that removes the user with the given id.
        /// </summary>
        /// <param name="userManager">The user manager responsible for
        /// managing application users.</param>
        /// <param name="id">Id of the user to be removed.</param>
        /// <returns>The id of the user which was removed.</returns>
        /// <exception cref="QueryException">Returns UserNotFound
        /// exception if the user with the given id could not be found.
        /// </exception>
        [Authorize(Policy = "CanEditUsers")]
        public async Task<int> RemoveUser(
            [Service] IUserManager userManager,
            int id)
        {
            return await userManager.Remove(id);
        }

        /// <summary>
        /// Method that updates the user with the given id according
        /// to the provided information.
        /// </summary>
        /// <param name="userManager">The user manager responsible for
        /// managing application users.</param>
        /// <param name="aclManager">The manager responsible for managing
        /// the access control lists linked to the users.</param>
        /// <param name="departmentManager">The manager responsible for managing
        /// the departments linked to the users.</param>
        /// <param name="id">Id of the user to be updated.</param>
        /// <param name="userName">The new username of the user.</param>
        /// <param name="password">The new password for the user.</param>
        /// <returns>The updated user.</returns>
        /// <param name="accessControlList">The new
        /// <see cref="AccessControlList"/> that will be linked to the user.
        /// </param>
        /// <param name="departmentId">The id of the new department of the
        /// user.</param>
        /// <exception cref="QueryException">Returns UserNotFound
        /// exception if the user with the given id could not be found.
        /// Alternatively returns PasswordTooWeak exception if the
        /// provided password does not meet the policy requirements.
        /// </exception>
        [Authorize(Policy = "CanEditUsers")]
        [Sensitive("password")]
        public async Task<User> UpdateUser(
            [Service] IUserManager userManager,
            [Service] IAccessControlListManager aclManager,
            [Service] IDepartmentManager departmentManager,
            int id,
            string? userName,
            string? password,
            AccessControlList? accessControlList,
            int? departmentId)
        {
            var user = await userManager.Update(id, userName, password);
            if (accessControlList != null)
            {
                await aclManager.LinkAccessControlList(
                    user.Id, accessControlList);
            }

            if (departmentId != null)
            {
                await this.UpdateUserDepartment(
                    departmentManager,
                    departmentId.Value,
                    user);
            }

            return user;
        }

        [Authorize(Policy = "CanEditDepartments")]
        private async Task UpdateUserDepartment(
            IDepartmentManager departmentManager,
            int departmentId,
            User user)
        {
            await departmentManager.AddUser(departmentId, user);
        }
    }
}
