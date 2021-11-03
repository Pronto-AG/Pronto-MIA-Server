namespace Pronto_MIA.BusinessLogic.API.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using HotChocolate.Execution;
    using HotChocolate.Execution.Instrumentation;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Logs the query request to the console.
    /// </summary>
    public class QueryLogger : ExecutionDiagnosticEventListener
    {
        private static readonly string[] SensitiveParameters;
        private static Stopwatch queryTimer;
        private readonly ILogger<QueryLogger> logger;

        /// <summary>
        /// Initializes static members of the <see cref="QueryLogger"/> class.
        /// This includes getting all attributes marked as sensitive in order
        /// to exclude them from logging.
        /// </summary>
        static QueryLogger()
        {
            var assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();
            var parameters = assemblyTypes
                .SelectMany(x => x.GetMethods())
                .SelectMany(x => x.GetCustomAttributes<SensitiveAttribute>())
                .Select(x => x.QueryPropertyName)
                .Distinct().ToList();

            SensitiveParameters = new string[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
            {
                SensitiveParameters[i] = parameters[i];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryLogger"/>
        /// class.
        /// </summary>
        /// <param name="logger">The logger to be used.</param>
        public QueryLogger(
            ILogger<QueryLogger> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Gets called as soon as a request comes in.
        /// </summary>
        /// <param name="context">The Context of the request.</param>
        /// <returns>An object that will live as long as the request.</returns>
        public override IDisposable ExecuteRequest(IRequestContext context)
        {
            return new RequestScope(this.logger, context);
        }

        private class RequestScope : IDisposable
        {
            private readonly IRequestContext context;
            private readonly ILogger<QueryLogger> logger;

            public RequestScope(
                ILogger<QueryLogger> logger,
                IRequestContext context)
            {
                this.logger = logger;
                this.context = context;
                this.logger.LogInformation(
                    "Request started");

                queryTimer = new Stopwatch();
                queryTimer.Start();
            }

            public void Dispose()
            {
                queryTimer.Stop();
                var query = string.Empty;
                var variables = new List<QueryVariable>();
                if (this.context.Document is not null)
                {
                    var stringDocument = this.context.Document.ToString(true);
                    var sensitiveParameters =
                        GetSensitiveParameters(stringDocument);
                    var sensitiveVariables = GetSensitiveVariables(
                        stringDocument, sensitiveParameters);
                    query = CleanQuery(stringDocument, sensitiveParameters);
                    if (this.context.Variables != null)
                    {
                        variables = CleanVariables(
                                this.context.Variables!.ToList(),
                                sensitiveVariables);
                    }
                }

                this.logger.LogInformation(
                    QueryLoggerFormatter.FormatQuery(
                        query, variables, queryTimer.Elapsed.Milliseconds));
            }

            private static List<string> GetSensitiveParameters(string document)
            {
                return SensitiveParameters
                    .Where(parameter =>
                        StringContains(document, parameter))
                    .ToList();
            }

            // Regex is:
            // {parameter}fcmToken}\s*:\s*\$(?<variableName>[^\s,\)]*)
            private static List<string> GetSensitiveVariables(
                string document, IEnumerable<string> sensitiveParameters)
            {
                var sensitiveVariables = new List<string>();
                foreach (var parameter in sensitiveParameters)
                {
                    var result = Regex.Match(
                        document,
                        $"{parameter}\\s*:\\s*\\$(?<variableName>[^\\s,\\)]*)");
                    var variableName = result.Groups["variableName"].Value;

                    if (variableName != string.Empty)
                    {
                        sensitiveVariables.Add(variableName);
                    }
                }

                return sensitiveVariables;
            }

            private static bool StringContains(
                string content, string keyToLookFor)
            {
                return (content.Length - content.Replace(
                    keyToLookFor, string.Empty).Length)
                    / keyToLookFor.Length > 0;
            }

            // Regex is:
            // {parameter}\s*:\s*(?=")(?:"(?<content>[^"\\]*(?:\\[\s\S]
            //      [^"\\]*)*)")
            private static string ReplaceSensitiveParameters(
                string content, IEnumerable<string> parameters)
            {
                var result = content;
                foreach (var parameter in parameters)
                {
                    result = Regex.Replace(
                        result,
                        new StringBuilder(parameter)
                            .Append("\\s*:\\s*(?=\")(?:\"(?<content>[^\"\\\\]*")
                            .Append("(?:\\\\[\\s\\S][^\"\\\\]*)*)\")")
                            .ToString(),
                        $"{parameter}:\"***\"");
                }

                return result;
            }

            private static string CleanQuery(
                string document, List<string> sensitiveParameters)
            {
                return ReplaceSensitiveParameters(
                    document,
                    sensitiveParameters);
            }

            private static List<QueryVariable> CleanVariables(
                IReadOnlyCollection<VariableValue> variables,
                ICollection<string> sensitiveVariables)
            {
                var cleanVariables = new List<QueryVariable>();
                if (variables.Count <= 0)
                {
                    return cleanVariables;
                }

                foreach (var variable in variables)
                {
                    QueryVariable newVariable = default(QueryVariable);
                    newVariable.Name = variable.Name.ToString();
                    newVariable.Type = variable.Type.ToString();
                    newVariable.Value = variable.Value.ToString();
                    if (sensitiveVariables.Contains(newVariable.Name))
                    {
                        newVariable.Value = "***";
                    }

                    cleanVariables.Add(newVariable);
                }

                return cleanVariables;
            }
        }
    }
}
