namespace Tests
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.Extensions.Configuration;
    using Pronto_MIA.DataAccess;

    public static class TestHelpers
    {
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

        public static DbContextOptions<ProntoMiaDbContext>
            GetInMemoryDbOptions(string name)
        {
            var options = new DbContextOptionsBuilder<ProntoMiaDbContext>()
                .UseInMemoryDatabase(name)
                .ConfigureWarnings(builder =>
                {
                    builder.Ignore(
                        InMemoryEventId.TransactionIgnoredWarning);
                })
                .Options;
            options.Freeze();

            return options;
        }

        public static ProntoMiaDbContext InMemoryDbContext(
            string name = "")
        {
            if (name == string.Empty)
            {
                name = Guid.NewGuid().ToString();
            }

            var options = TestHelpers.GetInMemoryDbOptions(name);
            return new ProntoMiaDbContext(options);
        }
    }
}