namespace Tests.TestBusinessLogic.TestAPI.TestInterceptors
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using HotChocolate;
    using HotChocolate.Execution;
    using Microsoft.AspNetCore.Http;
    using NSubstitute;
    using NSubstitute.ReturnsExtensions;
    using Pronto_MIA.BusinessLogic.API.Interceptors;
    using Pronto_MIA.BusinessLogic.API.Types;
    using Pronto_MIA.DataAccess;
    using Xunit;

    public class TestUserStateRequestInterceptor
    {
        private readonly ProntoMiaDbContext dbContext;

        public TestUserStateRequestInterceptor()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
        }

        [Fact]
        public async Task TestNotAuthenticated()
        {
            var context = Substitute.For<HttpContext>();
            var builder = Substitute.For<IQueryRequestBuilder>();

            await UserStateRequestInterceptor.AddUserState(
                TestHelpers.TestConfiguration,
                context,
                builder,
                this.dbContext);

            builder.Received().SetProperty(
                ApiUserGlobalState.ApiUserStateName, null);
        }

        [Fact]
        public async Task TestInvalidId()
        {
            var context = Substitute.For<HttpContext>();
            context.User.Identity.IsAuthenticated.Returns(true);
            context.User.FindFirst(Arg.Any<string>()).ReturnsForAnyArgs(
                new Claim("id", "-5"));
            var builder = Substitute.For<IQueryRequestBuilder>();

            var error = await Assert.ThrowsAsync<GraphQLException>(async () =>
            {
                await UserStateRequestInterceptor.AddUserState(
                    TestHelpers.TestConfiguration,
                    context,
                    builder,
                    this.dbContext);
            });

            Assert.Equal(
                "AUTH_NOT_AUTHORIZED",
                error.Errors[0].Code);
            builder.DidNotReceiveWithAnyArgs().SetProperty(
                ApiUserGlobalState.ApiUserStateName, null);
        }

        [Fact]
        public async Task TestNoIssuedAtToken()
        {
            var user = this.dbContext.Users.Single(u => u.UserName == "Bob");
            user.LastInvalidated = DateTime.UtcNow;
            await this.dbContext.SaveChangesAsync();
            var context = Substitute.For<HttpContext>();
            context.User.Identity.IsAuthenticated.Returns(true);
            context.User.FindFirst(ClaimTypes.NameIdentifier).Returns(
                new Claim("id", user.Id.ToString()));
            context.User.FindFirst("issuedAt").ReturnsNull();
            var builder = Substitute.For<IQueryRequestBuilder>();

            var error = await Assert.ThrowsAsync<GraphQLException>(async () =>
            {
                await UserStateRequestInterceptor.AddUserState(
                    TestHelpers.TestConfiguration,
                    context,
                    builder,
                    this.dbContext);
            });

            Assert.Equal(
                "AUTH_NOT_AUTHORIZED",
                error.Errors[0].Code);
            builder.DidNotReceiveWithAnyArgs().SetProperty(
                ApiUserGlobalState.ApiUserStateName, null);
        }

        [Fact]
        public async Task TestInvalidatedToken()
        {
            var user = this.dbContext.Users.Single(u => u.UserName == "Bob");
            user.LastInvalidated = DateTime.UtcNow;
            await this.dbContext.SaveChangesAsync();
            var context = Substitute.For<HttpContext>();
            context.User.Identity.IsAuthenticated.Returns(true);
            context.User.FindFirst(ClaimTypes.NameIdentifier).Returns(
                new Claim("id", user.Id.ToString()));
            context.User.FindFirst("issuedAt").Returns(
                new Claim(
                    "issued",
                    DateTime.UtcNow.AddSeconds(-5)
                        .ToString(CultureInfo.InvariantCulture)));
            var builder = Substitute.For<IQueryRequestBuilder>();

            var error = await Assert.ThrowsAsync<GraphQLException>(async () =>
            {
                var config = TestHelpers.TestConfiguration;
                await UserStateRequestInterceptor.AddUserState(
                    config, context, builder, this.dbContext);
            });

            Assert.Equal(
                "AUTH_NOT_AUTHORIZED",
                error.Errors[0].Code);
            builder.DidNotReceiveWithAnyArgs().SetProperty(
                ApiUserGlobalState.ApiUserStateName, null);
        }

        [Fact]
        public async Task TestUserState()
        {
            var user = this.dbContext.Users.Single(u => u.UserName == "Bob");
            user.LastInvalidated = DateTime.UtcNow;
            await this.dbContext.SaveChangesAsync();
            var context = Substitute.For<HttpContext>();
            context.User.Identity.IsAuthenticated.Returns(true);
            context.User.FindFirst(ClaimTypes.NameIdentifier).Returns(
                new Claim("id", user.Id.ToString()));
            context.User.FindFirst("issuedAt").Returns(
                new Claim(
                    "issued",
                    DateTime.UtcNow.AddSeconds(5)
                        .ToString(CultureInfo.InvariantCulture)));
            var builder = Substitute.For<IQueryRequestBuilder>();

            await UserStateRequestInterceptor.AddUserState(
                    TestHelpers.TestConfiguration,
                    context,
                    builder,
                    this.dbContext);

            builder.Received().SetProperty(
                ApiUserGlobalState.ApiUserStateName, Arg.Any<ApiUserState>());
        }
    }
}