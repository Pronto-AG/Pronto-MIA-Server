namespace Tests.TestBusinessLogic.TestAPI.TestEntityExtensions
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using NSubstitute;
    using Pronto_MIA.BusinessLogic.API.EntityExtensions;
    using Pronto_MIA.DataAccess;
    using Pronto_MIA.Domain.Entities;
    using Xunit;

    public class TestInternalNewsResolvers
    {
        private readonly ProntoMiaDbContext dbContext;

        private readonly InternalNewsResolvers internalNewsResolvers;

        public TestInternalNewsResolvers()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
            this.internalNewsResolvers = new InternalNewsResolvers();
        }

        [Fact]
        public void TestCreatesLink()
        {
            var internalNews = this.GetSampleInternalNews();
            var httpContextAccessor =
                Substitute.For<IHttpContextAccessor>();
            httpContextAccessor.HttpContext
                .Request.Host = new HostString("localhost");
            httpContextAccessor.HttpContext
                .Request.PathBase = new PathString("/path");
            var myConfiguration = new Dictionary<string, string>
            {
                { "StaticFiles:ENDPOINT", "static" },
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration).Build();
            var uri = this.internalNewsResolvers
                .Link(internalNews, configuration, httpContextAccessor);
            var expectedPath = "https://localhost/path/static/internal_news/"
                + internalNews.FileUuid + internalNews.FileExtension;
            Assert.Equal(expectedPath, uri.AbsoluteUri);
        }

        private InternalNews GetSampleInternalNews()
        {
            return new InternalNews(
                "Title",
                "Description",
                DateTime.MinValue,
                Guid.NewGuid(),
                ".png");
        }
    }
}