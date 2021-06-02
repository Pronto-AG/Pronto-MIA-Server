namespace Pronto_MIA.BusinessLogic.Security.Authorization.Interfaces
{
    /// <summary>
    /// Interface that has to be implemented in order to make an object
    /// department comparable.
    /// </summary>
    public interface IDepartmentComparable
    {
        /// <summary>
        /// Gets the department id of the underlying object.
        /// </summary>
        public int? DepartmentId { get; }
    }
}
