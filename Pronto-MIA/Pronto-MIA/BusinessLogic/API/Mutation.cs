using System;

#nullable enable
namespace Pronto_MIA.BusinessLogic.API
{
    using System.IO;
    using System.Threading.Tasks;
    using HotChocolate.AspNetCore.Authorization;
    using HotChocolate.Types;

    public class Mutation
    {
        /// <summary>
        /// Method to upload a pdf to the server.
        /// </summary>
        /// <returns>If successful.</returns>
        [Authorize]
        public async Task<bool> UploadPdf(IFile upload)
        {
            // you can now work with standard stream functionality of .NET to
            // handle the file.
            try
            {
                Console.WriteLine("Hello?");
                await using Stream stream = upload.OpenReadStream();
                await using (FileStream fs = File.Create("upload.pdf"))
                {
                    await stream.CopyToAsync(fs);
                    stream.Close();
                    fs.Close();
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
