namespace Pronto_MIA.DataAccess.Managers.Interfaces
{
    using System.Threading.Tasks;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Interface declaring the operations needed of a access control list
    /// manager service.
    /// </summary>
    public interface IAccessControlListManager
    {
        /// <summary>
        /// Links a given <see cref="AccessControlList"/>
        /// to the user identified by the id. If a
        /// <see cref="AccessControlList"/> for the user
        /// already exists it will be overwritten.
        /// </summary>
        /// <param name="userId">The id of the user to link
        /// the <see cref="AccessControlList"/> to.</param>
        /// <param name="accessControlList"> The <see cref="AccessControlList"/>
        /// to link with the user.</param>
        /// <returns>A task that can be awaited.</returns>
        public Task LinkAccessControlList(
            int userId, AccessControlList accessControlList);
    }
}
