#nullable enable
namespace Pronto_MIA.DataAccess.Managers.Interfaces
{
    using System.Linq;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Interface declaring the operations needed of a user manager service.
    /// </summary>
    public interface IUserDepartmentManager
    {
        /// <summary>
        /// Method to overwrite the <see cref="ProntoMiaDbContext"/>
        /// used by the manager. This can be used if transactions
        /// over multiple managers have to be implemented.
        /// </summary>
        /// <param name="dbContext">The db context to be used by the
        /// manager.</param>
        public void SetDbContext(ProntoMiaDbContext dbContext);

        /// <summary>
        /// Method that returns a UserDepartment object for the given
        /// departmentId.
        /// </summary>
        /// <param name="departmentId">The id of the department to be
        /// found.</param>
        /// <returns>Department or default if departmentId could not be
        /// found.</returns>
        public Task<UserDepartment?>
            GetByDepartmentId(int departmentId);

        /// <summary>
        /// Method that returns a UserDepartment object for the given userId.
        /// </summary>
        /// <param name="userId">The id of the user to be found.</param>
        /// <returns>User or default if userId could not be found.</returns>
        public Task<UserDepartment?>
            GetByUserId(int userId);

        /// <summary>
        /// Method that returns a UserDepartment object for the given userId.
        /// </summary>
        /// <param name="departmentId">The id of the department to be
        /// found.</param>
        /// <param name="userId">The id of the user to be found.</param>
        /// <returns>UserDepartment or default if userId or departmentId could
        /// not be found.</returns>
        public Task<UserDepartment?>
            GetByDepartmentAndUserId(int departmentId, int userId);

        /// <summary>
        /// Method to get all userDepartment.
        /// </summary>
        /// <returns>All usersDepartments.</returns>
        public IQueryable<UserDepartment> GetAll();

        /// <summary>
        /// Method that creates a new UserDepartment object with the given
        /// departmentId and userId.
        /// </summary>
        /// <param name="departmentId">The departmentId for the new
        /// userDepartment.</param>
        /// <param name="userId">The userId for the new userDepartment.</param>
        /// <returns>The newly created userDepartment.</returns>
        /// <exception cref="QueryException">Returns UserDepartmentAlreadyExists
        /// exception if the departmentId and userId in combination
        /// already exists.
        /// </exception>
        public Task<UserDepartment>
            Create(int departmentId, int userId);

        /// <summary>
        /// Method that updates the user with the given id according
        /// to the provided information.
        /// </summary>
        /// <param name="id">Id of the userDepartment to be updated.</param>
        /// <param name="departmentId">The new departmentId of the
        /// userDepartment.</param>
        /// <param name="userId">The new userId for the userDepartment.</param>
        /// <returns>The updated userDepartment.</returns>
        /// <exception cref="QueryException">Returns UserDepartmentNotFound
        /// exception if the userDepartment with the given id could not be
        /// found.
        /// </exception>
        public Task<UserDepartment> Update(
            int id, int? departmentId, int? userId);

        /// <summary>
        /// Removes the user with the given id.
        /// </summary>
        /// <param name="id">Id of the userDepartment to be removed.</param>
        /// <returns>The id of the userDepartment that was removed.</returns>
        /// <exception cref="QueryException">Returns UserDepartmentNotFound
        /// exception if the userDepartment with the given id could not be
        /// found.
        /// </exception>
        public Task<int> Remove(int id);

        /// <summary>
        /// Method to get a userDepartment with the help of its id.
        /// </summary>
        /// <param name="id">The id of the userDepartment.</param>
        /// <returns>The userDepartment with the given id.</returns>
        /// <exception cref="QueryException">If the userDepartment with the
        /// given id could not be found.</exception>
        public Task<UserDepartment> GetById(int id);
    }
}
