using System;
using System.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Data;
using Server.Services;

namespace Server
{
    public class Startup
    {
        public Startup(IConfiguration cfg)
        {
            this.cfg = cfg;
        }

        public IConfiguration cfg { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabaseService(this.cfg);
            services
                .AddGraphQLServer()
                .AddQueryType<Query>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            using (var context = serviceProvider.GetService<InformbobDbContext>())
            {
                Speaker dani = new Speaker
                {
                    Bio = "Hello Friends im Dani",
                    Name = "Dani Lombarti",
                    WebSite = "danilombarti.com.uk"
                };
                context.Add(dani);
                context.SaveChanges();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapGraphQL("/"); });
        }
    }
}