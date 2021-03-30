namespace Pronto_MIA.DataAccess.Managers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Pronto_MIA.BusinessLogic.Security;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class responsible for the lifecycle of a user within the application.
    /// </summary>
    public class UserManager
    {
        private readonly ProntoMIADbContext dbContext;
        private readonly IConfiguration cfg;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManager"/> class.
        /// </summary>
        /// <param name="logger">The logger to be used in order to document
        /// events regarding this user manager.</param>
        /// <param name="dbContext">The database context where users are
        /// persisted.</param>
        /// <param name="cfg">The configuration of this application.</param>
        public UserManager(
            ILogger<UserManager> logger,
            ProntoMIADbContext dbContext,
            IConfiguration cfg)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.cfg = cfg;
        }

        private string SigningKey =>
            this.cfg.GetValue<string>("JWT:SIGNING_KEY");

        private string Issuer =>
            this.cfg.GetValue<string>("JWT:ISSUER");

        private string Audience =>
            this.cfg.GetValue<string>("JWT:AUDIENCE");

        private int ValidForDays =>
            this.cfg.GetValue<int>("JWT:VALID_FOR_DAYS");

        /// <summary>
        /// Method to authenticate a user.
        /// </summary>
        /// <param name="userName">The username of the user to be authenticated.
        /// </param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A tuple of an error and a string. If no error occured the
        /// error will be null and the string will contain a JWT-Bearer-Token.
        /// If an error occured the string will be null and the error will
        /// contain the corresponding error object.</returns>
        public Tuple<DataAccess.Error?, string> Authenticate(
            string userName, string password)
        {
            var user = this.dbContext.Users.SingleOrDefault(
                u => u.UserName == userName);

            if (user == null)
            {
                this.logger.LogWarning(
                    "Invalid userName {UserName}", userName);
                return new Tuple<DataAccess.Error?, string>(
                    DataAccess.Error.UserNotFound, null);
            }

            var hashGenerator = HashGeneratorFactory.GetGeneratorForUser(user);

            if (!hashGenerator.ValidatePassword(
                password, user.PasswordHash))
            {
                this.logger.LogWarning(
                    "Invalid password for user {UserName}", userName);
                return new Tuple<DataAccess.Error?, string>(
                    DataAccess.Error.WrongPassword, null);
            }

            return new Tuple<DataAccess.Error?, string>(
                null, this.GenerateToken(user));
        }

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(this.SigningKey));
            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha512);
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            var token = new JwtSecurityToken(
                this.Issuer,
                this.Audience,
                claims,
                expires: DateTime.UtcNow.AddDays(this.ValidForDays),
                signingCredentials: credentials);

            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
    }
}
