namespace Tests
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Pronto_MIA.DataAccess;

    public class TestHelpers
    {
        public static ProntoMiaDbContext InMemoryDbContext
        {
            get
            {
                var options = new DbContextOptionsBuilder<ProntoMiaDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                options.Freeze();
                return new ProntoMiaDbContext(options);
            }
        }
    }
}