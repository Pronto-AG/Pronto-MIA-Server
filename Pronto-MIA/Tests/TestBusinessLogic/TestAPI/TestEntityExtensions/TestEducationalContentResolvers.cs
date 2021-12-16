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

    public class TestEducationalContentResolvers
    {
        private readonly ProntoMiaDbContext dbContext;

        private readonly EducationalContentResolvers
            educationalContentResolvers;

        public TestEducationalContentResolvers()
        {
            this.dbContext = TestHelpers.InMemoryDbContext();
            TestDataProvider.InsertTestData(this.dbContext);
            this.educationalContentResolvers =
                new EducationalContentResolvers();
        }

        [Fact]
        public void TestCreatesLink()
        {
            var educationalContent = this.GetSampleEducationalContent();
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
            var uri = this.educationalContentResolvers
                .Link(educationalContent, configuration, httpContextAccessor);
            var expectedPath =
                "https://localhost/path/static/educational_content/"
                    + educationalContent.FileUuid
                    + educationalContent.FileExtension;
            Assert.Equal(expectedPath, uri.AbsoluteUri);
        }

        private EducationalContent GetSampleEducationalContent()
        {
            return new EducationalContent(
                "Title",
                "Description",
                Guid.NewGuid(),
                ".mp4");
        }
    }
}