namespace Tests
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Pronto_MIA.DataAccess;

    public static class TestHelpers
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

        public static IConfiguration TestConfiguration
        {
            get
            {
                IConfigurationBuilder configurationBuilder =
                    new ConfigurationBuilder();
                configurationBuilder.AddJsonFile(
                    "./Files/appsettings.test.json", false, false);
                return configurationBuilder.Build();
            }
        }
    }
}