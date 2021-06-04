namespace Tests.TestBusinessLogic.TestAPI.TestLogging
{
    using System.Collections.Generic;
    using System.Linq;
    using HotChocolate.Execution;
    using HotChocolate.Language;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using NSubstitute.ReturnsExtensions;
    using Pronto_MIA.BusinessLogic.API.Logging;
    using Xunit;

    public class TestQueryLogger
    {
        private QueryLogger queryLogger;
        private ILogger<QueryLogger> logger;

        public TestQueryLogger()
        {
            this.logger = Substitute.For<ILogger<QueryLogger>>();
            this.queryLogger = new QueryLogger(this.logger);
        }

        [Fact]
        public void TestRequestStart()
        {
            var requestContext = Substitute.For<IRequestContext>();
            var scope = this.queryLogger.ExecuteRequest(requestContext);

            this.logger.Received().LogInformation("Request started");
        }

        [Fact]
        public void TestRequestNullDocument()
        {
            var requestContext = Substitute.For<IRequestContext>();
            requestContext.Document.ReturnsNullForAnyArgs();

            var scope = this.queryLogger.ExecuteRequest(requestContext);
            scope.Dispose();

            this.logger.Received().LogInformation("Request started");
            var call = this.logger.ReceivedCalls().Last();
            Assert.Contains(
                "Request ended.",
                call.GetArguments()[2].ToString() !);
        }

        [Fact]
        public void TestRequestEmptyDocument()
        {
            var requestContext = Substitute.For<IRequestContext>();
            requestContext.Document.Returns(
                new DocumentNode(new List<IDefinitionNode>()));
            requestContext.Variables.ReturnsNull();

            var scope = this.queryLogger.ExecuteRequest(requestContext);
            scope.Dispose();

            this.logger.Received().LogInformation("Request started");
            var call = this.logger.ReceivedCalls().Last();
            Assert.Contains(
                "Request ended.",
                call.GetArguments()[2].ToString() !);
        }

        [Fact]
        public void TestNonSensitiveDocumentParameter()
        {
            var requestContext = Substitute.For<IRequestContext>();
            var documentSubstitute = Substitute.For<ISyntaxNode>();
            documentSubstitute.ToString().ReturnsForAnyArgs(
                "Password: \"Test\"");
            requestContext.Document.Returns((DocumentNode)documentSubstitute);
            requestContext.Variables.ReturnsNull();

            var scope = this.queryLogger.ExecuteRequest(requestContext);
            scope.Dispose();

            this.logger.Received().LogInformation("Request started");
            var call = this.logger.ReceivedCalls().Last();
            Assert.Contains(
                "Request ended.",
                call.GetArguments()[2].ToString() !);
        }
    }
}