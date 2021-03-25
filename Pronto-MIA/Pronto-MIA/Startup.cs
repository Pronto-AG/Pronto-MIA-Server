namespace Pronto_MIA
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Pronto_MIA.Data;
    using Pronto_MIA.Services;

    /// <summary>
    /// Class for starting up the Asp.Net application.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// Gets the correct configuration by dependency injection.
        /// </summary>
        /// <param name="cfg">Configuration used by the startup class.</param>
        public Startup(IConfiguration cfg)
        {
            this.Cfg = cfg;
        }

        /// <summary>
        /// Gets the configuration used by the startup class.
        /// </summary>
        public IConfiguration Cfg { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add
        /// services to the container. For more information on how to configure
        /// your application, visit
        /// https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </summary>
        /// <param name="services">The Services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabaseService(this.Cfg);
            services
                .AddGraphQLServer()
                .AddQueryType<Query>();
        }

        /// <summary>
        /// Creates a new speaker and inserts it into the database.
        /// </summary>
        /// <param name="serviceProvider">Dependency injected service provider
        /// used for finding the database service. </param>
        /// <exception cref="NullReferenceException">If the database service
        /// cannot be found.</exception>
        public void AddSpeaker(IServiceProvider serviceProvider)
        {
            using (
                var context = serviceProvider.GetService<InformbobDbContext>())
            {
                Speaker dani = new Speaker
                {
                    Bio = "Hello Friends im Dani",
                    Name = "Dani Lombarti",
                    WebSite = "danilombarti.com.uk",
                };

                if (context != null)
                {
                    context.Add(dani);
                    context.SaveChanges();
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure
        /// the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Builder.</param>
        /// <param name="env">Environment.</param>
        /// <param name="serviceProvider">Services.</param>
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IServiceProvider serviceProvider)
        {
            this.AddSpeaker(serviceProvider);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL("/");
            });
        }
    }
}
