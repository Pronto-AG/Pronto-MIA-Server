namespace Pronto_MIA.Services
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// Class representing an authentication service which can be used to
    /// authenticate incoming users with the api.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class AuthenticationService
    {
        /// <summary>
        /// Method to add the authentication service to the service collection.
        /// In this method a JWTBearer authentication scheme is configured and
        /// added to the service collection.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="cfg">Application configuration.</param>
        public static void AddAuthenticationService(
            this IServiceCollection services, IConfiguration cfg)
        {
            var signingKey = cfg.GetValue<string>("JWT:SIGNING_KEY");
            var issuer = cfg.GetValue<string>("JWT:ISSUER");
            var audience = cfg.GetValue<string>("JWT:AUDIENCE");
            var requiresHttps = cfg.GetValue<bool>("JWT:REQUIRE_HTTPS");

            var key = Encoding.ASCII.GetBytes(signingKey);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = issuer,

                            ValidateAudience = true,
                            ValidAudience = audience,

                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(key),

                            ValidateLifetime = true,
                        };
                    options.RequireHttpsMetadata = requiresHttps;
                    options.SaveToken = true;
                });
        }
    }
}
