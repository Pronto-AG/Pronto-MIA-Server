namespace Tests
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Pronto_MIA.DataAccess;

    public class TestHelpers
    {
        public static ProntoMIADbContext InMemoryDbContext
        {
            get
            {
                var options = new DbContextOptionsBuilder<ProntoMIADbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                options.Freeze();
                return new ProntoMIADbContext(options);
            }
        }
    }
}