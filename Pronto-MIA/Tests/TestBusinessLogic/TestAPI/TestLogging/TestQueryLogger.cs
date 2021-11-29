namespace Tests.TestBusinessLogic.TestAPI.TestLogging
{
    using System.Collections.Generic;
    using System.Linq;
    using HotChocolate.Execution;
    using HotChocolate.Language;
    using HotChocolate.Types;
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
                call.GetArguments()[2].ToString()!);
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
                call.GetArguments()[2].ToString()!);
        }

        [Fact]
        public void TestNonSensitiveDocumentParameter()
        {
            var requestContext = Substitute.For<IRequestContext>();
            var document = this.CreateDocument("Name", "Bob");
            requestContext.Document.Returns(document);
            requestContext.Variables.ReturnsNull();

            var scope = this.queryLogger.ExecuteRequest(requestContext);
            scope.Dispose();

            this.logger.Received().LogInformation("Request started");
            var call = this.logger.ReceivedCalls().Last();
            Assert.Contains(
                "Request ended.",
                call.GetArguments()[2].ToString()!);
            Assert.Contains(
                "Bob",
                call.GetArguments()[2].ToString()!);
        }

        [Fact]
        public void TestSensitiveDocumentParameter()
        {
            var requestContext = Substitute.For<IRequestContext>();
            var document = this.CreateDocument("password", "Bob");
            requestContext.Document.Returns(document);
            requestContext.Variables.ReturnsNull();

            var scope = this.queryLogger.ExecuteRequest(requestContext);
            scope.Dispose();

            this.logger.Received().LogInformation("Request started");
            var call = this.logger.ReceivedCalls().Last();
            Assert.Contains(
                "Request ended.",
                call.GetArguments()[2].ToString()!);
            Assert.DoesNotContain(
                "Bob",
                call.GetArguments()[2].ToString()!);
        }

        [Fact]
        public void TestNonSensitiveDocumentVariable()
        {
            var requestContext = Substitute.For<IRequestContext>();
            var document = this.CreateDocument("Name", "Name", true);
            requestContext.Document.Returns(document);
            var collection = this.GetVariableCollection("Name", "Bob");
            requestContext.Variables.Returns(collection);

            var scope = this.queryLogger.ExecuteRequest(requestContext);
            scope.Dispose();

            this.logger.Received().LogInformation("Request started");
            var call = this.logger.ReceivedCalls().Last();
            Assert.Contains(
                "Request ended.",
                call.GetArguments()[2].ToString()!);
            Assert.Contains(
                "Bob",
                call.GetArguments()[2].ToString()!);
        }

        [Fact]
        public void TestSensitiveDocumentVariable()
        {
            var requestContext = Substitute.For<IRequestContext>();
            var document = this.CreateDocument("password", "password", true);
            requestContext.Document.Returns(document);
            var collection = this.GetVariableCollection("password", "Bob");
            requestContext.Variables.Returns(collection);

            var scope = this.queryLogger.ExecuteRequest(requestContext);
            scope.Dispose();

            this.logger.Received().LogInformation("Request started");
            var call = this.logger.ReceivedCalls().Last();
            Assert.Contains(
                "Request ended.",
                call.GetArguments()[2].ToString()!);
            Assert.DoesNotContain(
                "Bob",
                call.GetArguments()[2].ToString()!);
        }

        private DocumentNode CreateDocument(
            string argumentName,
            string argumentValue,
            bool isVariable = false)
        {
            var argument = this.GetArgument(
                argumentName, argumentValue, isVariable);
            var selectionNode = new FieldNode(
                null,
                new NameNode("Authentication"),
                null,
                new List<DirectiveNode>(),
                new List<ArgumentNode>() { argument },
                null);
            var selectionSet = new SelectionSetNode(
                new List<ISelectionNode>() { selectionNode });
            var definition = new OperationDefinitionNode(
                null,
                null,
                OperationType.Query,
                new List<VariableDefinitionNode>(),
                new List<DirectiveNode>(),
                selectionSet);
            var document = new DocumentNode(
                new List<IDefinitionNode>()
                {
                    definition,
                });
            return document;
        }

        private ArgumentNode GetArgument(
            string argumentName,
            string argumentValue,
            bool isVariable)
        {
            ArgumentNode argument;
            if (isVariable)
            {
                argument = new ArgumentNode(
                    argumentName,
                    new VariableNode(argumentValue));
            }
            else
            {
                argument = new ArgumentNode(argumentName, argumentValue);
            }

            return argument;
        }

        private IVariableValueCollection GetVariableCollection(
            string variableName,
            string variableValue)
        {
            var collection = Substitute.For<IVariableValueCollection>();
            collection.GetEnumerator().Returns(new List<VariableValue>()
            {
                new VariableValue(
                    variableName,
                    new StringType(),
                    new StringValueNode(variableValue)),
            }.GetEnumerator());

            return collection;
        }
    }
}
