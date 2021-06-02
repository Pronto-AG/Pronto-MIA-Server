namespace Tests.TestBusinessLogic.TestAPI.TestTypes.TestQuery
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.Domain.Entities;

    public class QueryTestHelpers
    {
        public static async Task<User> CreateUserWithAcl(
            ProntoMiaDbContext dbContext,
            string username,
            AccessControlList acl = null)
        {
            var user = new User(
                username, new byte[5], "{}", "{}");
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            if (acl == null)
            {
                acl = new AccessControlList(user.Id);
            }
            else
            {
                acl.UserId = user.Id;
            }

            dbContext.AccessControlLists.Add(acl);
            await dbContext.SaveChangesAsync();

            return user;
        }
    }
}