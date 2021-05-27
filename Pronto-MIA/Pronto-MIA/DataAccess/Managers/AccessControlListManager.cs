#nullable enable
namespace Pronto_MIA.DataAccess.Managers
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class responsible for the lifecycle of a deployment plan within the
    /// application.
    /// </summary>
    public class AccessControlListManager : IAccessControlListManager
    {
        private ProntoMiaDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AccessControlListManager"/> class.
        /// </summary>
        /// <param name="dbContext">The database context where object are
        /// persisted.</param>
        public AccessControlListManager(
            ProntoMiaDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <inheritdoc/>
        public void SetDbContext(ProntoMiaDbContext context)
        {
            this.dbContext = context;
        }

        /// <inheritdoc/>
        public async Task LinkAccessControlList(
            int userId,
            AccessControlList accessControlList)
        {
            var existingAcl = await this.dbContext.AccessControlLists
                .SingleOrDefaultAsync(acl => acl.UserId == userId);

            if (existingAcl != default)
            {
                var oldId = existingAcl.Id;
                this.dbContext.Remove(existingAcl);
                accessControlList.Id = oldId;
            }

            accessControlList.UserId = userId;

            this.dbContext.Add(accessControlList);
            await this.dbContext.SaveChangesAsync();
        }
    }
}
