namespace Pronto_MIA
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Abstractions;
    using System.Net;
    using HotChocolate.AspNetCore;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Hosting;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.DataAccess.Managers;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Services;

    /// <summary>
    /// Class for starting up the Asp.Net application.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// Gets the correct configuration by dependency injection.
        /// </summary>
        /// <param name="cfg">Application configuration.</param>
        public Startup(
            IConfiguration cfg)
        {
            this.Cfg = cfg;
        }

        /// <summary>
        /// Gets the configuration used by the startup class.
        /// </summary>
        private IConfiguration Cfg { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add
        /// services to the container. For more information on how to configure
        /// your application, visit
        /// https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </summary>
        /// <param name="services">The Services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureLogging();
            services.AddHttpContextAccessor();
            services.AddScoped<IFileSystem, FileSystem>();
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IFileManager, FileManager>();
            services.AddScoped<IDeploymentPlanManager, DeploymentPlanManager>();
            services.AddScoped<IFirebaseTokenManager, FirebaseTokenManager>();
            services.AddScoped<IFirebaseMessagingManager,
                FirebaseMessagingManager>();
            services.AddScoped<IAccessControlListManager,
                AccessControlListManager>();
            services.AddScoped<IDepartmentManager, DepartmentManager>();
            services.AddDatabaseService(this.Cfg);
            services.AddAuthorization();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                            .SetIsOriginAllowed(origin => origin != "null")
                            .AllowAnyHeader();
                    });
            });
            services.AddAuthenticationService(this.Cfg);
            services.AddAuthorizationService(this.Cfg);
            services.AddGraphQLService();
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
            DbMigrationHelper.Migrate(app);

            this.ConfigureAppExceptions(app, env);

            // app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors();
            this.ConfigureStaticFiles(app);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL("/" +
                    this.Cfg.GetValue<string>("API:GRAPHQL_ENDPOINT"))
                    .WithOptions(
                        new GraphQLServerOptions
                        {
                            Tool = { Enable = env.IsDevelopment() },
                        });
            });
        }

        /// <summary>
        /// Method to set the exception policy of ASP.NET depending on the
        /// running environment.
        /// </summary>
        /// <param name="app">The application builder which will create the
        /// application.</param>
        /// <param name="env">The host environment.</param>
        private void ConfigureAppExceptions(
            IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode = 200;
                        context.Response.ContentType = "application/json";
                        const string msg = "{ \"errors\": " +
                                           "[ { \"message\": " +
                                           "\"Invalid Request\", " +
                                           "\"extensions\":"
                                           + " { \"code\": " +
                                           "\"INVALID_REQUEST\" } } ] }";
                        await context.Response.WriteAsync(msg);
                    });
                });
                app.UseHsts();
            }
        }

        /// <summary>
        /// Method to configure the static file endpoint. This endpoint is used
        /// in order to serve files connected to deployment plans or other
        /// application object.
        /// </summary>
        /// <param name="app">The application builder which will be extended
        /// with the endpoint.</param>
        private void ConfigureStaticFiles(IApplicationBuilder app)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    if (ctx.Context.User.Identity == null ||
                        ctx.Context.User.Identity.IsAuthenticated)
                    {
                        return;
                    }

                    ctx.Context.Response.StatusCode =
                        (int)HttpStatusCode.Unauthorized;
                    ctx.Context.Response.ContentLength = 0;
                    ctx.Context.Response.Body = Stream.Null;
                },
                FileProvider = new PhysicalFileProvider(
                    this.Cfg.GetValue<string>("StaticFiles:ROOT_DIRECTORY")),
                RequestPath = "/" +
                              this.Cfg.GetValue<string>(
                                  "StaticFiles:ENDPOINT"),
            });
        }
    }
}
