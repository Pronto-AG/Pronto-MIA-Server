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

    public class TestExternalNewsResolvers
    {
        private readonly ProntoMiaDbContext dbContext;

        private readonly ExternalNewsResolvers externalNewsResolvers;

        public TestExternalNewsResolvers()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
            this.externalNewsResolvers = new ExternalNewsResolvers();
        }

        [Fact]
        public void TestCreatesLink()
        {
            var externalNews = this.GetSampleExternalNews();
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
            var uri = this.externalNewsResolvers
                .Link(externalNews, configuration, httpContextAccessor);
            var expectedPath = "https://localhost/path/static/external_news/"
                + externalNews.FileUuid + externalNews.FileExtension;
            Assert.Equal(expectedPath, uri.AbsoluteUri);
        }

        private ExternalNews GetSampleExternalNews()
        {
            return new ExternalNews(
                "Title",
                "Description",
                DateTime.MinValue,
                Guid.NewGuid(),
                ".png");
        }
    }
}