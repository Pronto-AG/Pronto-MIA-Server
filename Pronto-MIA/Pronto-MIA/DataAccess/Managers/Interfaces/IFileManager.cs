namespace Pronto_MIA.DataAccess.Managers.Interfaces
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using HotChocolate.Types;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;

    /// <summary>
    /// Interface declaring the operations needed of a file manager service.
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// Method that creates a file inside the directory where files are
        /// statically served by ASP.NET.
        /// </summary>
        /// <param name="subDirectory">The subdirectory in which the new file
        /// should be placed.</param>
        /// <param name="fileName">The name the file should be saved as.</param>
        /// <param name="file">The file that will be saved.</param>
        /// <returns>Empty task.</returns>
        /// <exception cref="QueryException">Throws if an error occured during
        /// file creation.</exception>
        public Task
            Create(string subDirectory, string fileName, IFile file);

        /// <summary>
        /// Method that removes a file from the directory statically served by
        /// ASP.NET.
        /// </summary>
        /// <param name="subDirectory">The subdirectory in which the file can
        /// be found.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="fileExtension">The file extension of the file.</param>
        /// <exception cref="QueryException">Throws if the given file could not
        /// be removed.</exception>
        public void Remove(
            string subDirectory,
            string fileName,
            string fileExtension);

        /// <summary>
        /// Method to extract the file extension from a <see cref="IFile"/>
        /// object.
        /// </summary>
        /// <param name="file">The file from which the extension should be
        /// extracted.</param>
        /// <returns>The file extension.</returns>
        /// <exception cref="QueryException">Throws if the file that was
        /// uploaded has no file extension.</exception>
        public static string GetFileExtension(IFile file)
        {
            var ext = Path.GetExtension(file.Name);
            if (string.IsNullOrEmpty(ext))
            {
                throw DataAccess.Error.FileOperationError.AsQueryException();
            }

            return ext!;
        }
    }
}
