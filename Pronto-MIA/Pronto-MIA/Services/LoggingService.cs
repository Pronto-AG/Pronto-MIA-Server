namespace Pronto_MIA.Services
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Class representing the logging service.
    /// </summary>
    public static class LoggingService
    {
        /// <summary>
        /// Method to configure logging within the applications service
        /// collection.
        /// </summary>
        /// <param name="services">Service collection to be adjusted.</param>
        /// <returns>The adjusted service collection.</returns>
        public static IServiceCollection ConfigureLogging(
            this IServiceCollection services)
        {
            services.AddLogging(opt =>
            {
                opt.AddSimpleConsole(options =>
                {
                    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                    options.IncludeScopes = true;
                });
            });

            return services;
        }
    }
}
