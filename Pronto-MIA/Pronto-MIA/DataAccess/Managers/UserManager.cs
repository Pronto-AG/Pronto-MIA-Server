#nullable enable
namespace Pronto_MIA.DataAccess.Managers
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using HotChocolate.Execution;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.BusinessLogic.Security;
    using Pronto_MIA.BusinessLogic.Security.Abstract;
    using Pronto_MIA.BusinessLogic.Security.Interfaces;
    using Pronto_MIA.DataAccess.Managers.Interfaces;
    using Pronto_MIA.Domain.Entities;

    /// <summary>
    /// Class responsible for the lifecycle of a user within the application.
    /// </summary>
    public class UserManager : IUserManager
    {
        private static readonly IHashGeneratorOptions
            DefaultHashGeneratorOptions = new Pbkdf2GeneratorOptions(1500);

        private static readonly HashGenerator DefaultHashGenerator
            = new Pbkdf2Generator(
                (Pbkdf2GeneratorOptions)DefaultHashGeneratorOptions);

        private readonly ProntoMiaDbContext dbContext;
        private readonly IConfiguration cfg;
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManager"/> class.
        /// </summary>
        /// <param name="dbContext">The database context where users are
        /// persisted.</param>
        /// <param name="logger">The logger to be used in order to document
        /// events regarding this user manager.</param>
        /// <param name="cfg">The configuration of this application.</param>
        public UserManager(
            ProntoMiaDbContext dbContext,
            IConfiguration cfg,
            ILogger<UserManager> logger)
        {
            this.dbContext = dbContext;
            this.cfg = cfg;
            this.logger = logger;
        }

        private int MinPasswordLenght =>
            this.cfg.GetValue<int>("User:MIN_PASSWORD_LENGHT");

        private string SigningKey =>
            this.cfg.GetValue<string>("JWT:SIGNING_KEY");

        private string Issuer =>
            this.cfg.GetValue<string>("JWT:ISSUER");

        private string Audience =>
            this.cfg.GetValue<string>("JWT:AUDIENCE");

        private int ValidForDays =>
            this.cfg.GetValue<int>("JWT:VALID_FOR_DAYS");

        /// <inheritdoc/>
        public async Task<string> Authenticate(
            string userName, string password)
        {
            var user = await this.GetByUserName(userName);
            if (user == default)
            {
                throw DataAccess.Error.UserNotFound.AsQueryException();
            }

            var hashGenerator = HashGeneratorFactory.GetGeneratorForUser(user);

            if (!hashGenerator.ValidatePassword(
                password, user.PasswordHash))
            {
                this.logger.LogWarning(
                    "Invalid password for user {UserName}", userName);
                throw DataAccess.Error.WrongPassword.AsQueryException();
            }

            if (hashGenerator.GetIdentifier() !=
                DefaultHashGenerator.GetIdentifier())
            {
                await this.UpdateHash(password, user);
            }

            this.logger.LogDebug(
                "User {UserName} has been authenticated", userName);
            return this.GenerateToken(user);
        }

        /// <inheritdoc/>
        public async Task<User?>
            GetByUserName(string userName)
        {
            var user = await this.dbContext.Users.SingleOrDefaultAsync(
                u => u.UserName == userName);
            if (user == default)
            {
                this.logger.LogWarning(
                    "Invalid username {UserName}", userName);
            }

            return user;
        }

        /// <inheritdoc/>
        public async Task<User> Create(string userName, string password)
        {
            var checkUserName = await this.GetByUserName(userName);
            if (checkUserName != default)
            {
                throw DataAccess.Error.UserAlreadyExists.AsQueryException();
            }

            var checkPassword = PasswordHelper
                .PasswordPolicyMet(password, this.MinPasswordLenght);
            if (checkPassword != PasswordHelper.PasswordPolicyViolation.None)
            {
                var arguments = new Dictionary<string, string>
                {
                    ["minLenght"] = this.MinPasswordLenght.ToString(),
                    ["passwordPolicyViolation"] = checkPassword.ToString(),
                };
                throw DataAccess.Error.PasswordTooWeak
                    .AsQueryException(arguments);
            }

            var user = new User(
                userName,
                DefaultHashGenerator.HashPassword(password),
                DefaultHashGenerator.GetIdentifier(),
                DefaultHashGenerator.GetOptions().ToJson());
            this.dbContext.Users.Add(user);
            await this.dbContext.SaveChangesAsync();
            return user;
        }

        /// <inheritdoc/>
        public async Task<int>
            Remove(int id)
        {
            var deploymentPlan = await this.GetById(id);

            this.dbContext.Remove(deploymentPlan);
            await this.dbContext.SaveChangesAsync();

            return id;
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
            this.logger.LogDebug(
                "Token for user {UserName} has been created",
                user.UserName);

            return tokenString;
        }

        /// <summary>
        /// Method to update a users hash to the latest defined default hash.
        /// This has to be done if the default hash has been adjusted in order
        /// to comply with new security regulations.
        /// </summary>
        /// <param name="password">The password for the user.</param>
        /// <param name="user">The user to be updated.</param>
        private async Task UpdateHash(string password, User user)
        {
            user.PasswordHash = DefaultHashGenerator.HashPassword(password);
            user.HashGenerator = DefaultHashGenerator.GetIdentifier();
            user.HashGeneratorOptions =
                DefaultHashGenerator.GetOptions().ToJson();
            this.dbContext.Users.Update(user);
            await this.dbContext.SaveChangesAsync();

            this.logger.LogDebug(
                "Password-hash for User {UserName} has been updated",
                user.UserName);
        }

        /// <summary>
        /// Method to get a user with the help of its id.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns>The user with the given id.</returns>
        /// <exception cref="QueryException">If the user with the
        /// given id could not be found.</exception>
        private async Task<User> GetById(int id)
        {
            var user = await this.dbContext.Users
                .SingleOrDefaultAsync(u => u.Id == id);
            if (user != default)
            {
                return user;
            }

            this.logger.LogWarning(
                "Invalid user id {Id}", id);
            throw DataAccess.Error.UserNotFound
                .AsQueryException();
        }
    }
}
