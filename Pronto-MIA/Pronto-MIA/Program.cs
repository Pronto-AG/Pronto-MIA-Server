namespace Pronto_MIA
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Class for starting the application.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Method to create and configure a builder object which may then be
        /// used by Asp.Net.
        /// </summary>
        /// <param name="args">Possible arguments needed.</param>
        /// <returns>The created builder object.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables(prefix: "pronto-mia_");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
    }
}
