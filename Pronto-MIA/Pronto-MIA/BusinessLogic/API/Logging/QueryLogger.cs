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
    public class QueryLogger : DiagnosticEventListener
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
        public override IActivityScope ExecuteRequest(IRequestContext context)
        {
            return new RequestScope(this.logger, context);
        }

        private class RequestScope : IActivityScope
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
                StringBuilder stringBuilder = new StringBuilder(
                    "Request ended. Content was:");
                stringBuilder.Append(Environment.NewLine);
                queryTimer.Stop();
                if (this.context.Document is not null)
                {
                    var stringDocument = this.context.Document.ToString(true);
                    var sensitiveParameters =
                        GetSensitiveParameters(stringDocument);
                    var sensitiveVariables = GetSensitiveVariables(
                        stringDocument, sensitiveParameters);
                    stringBuilder.Append("+-+-+-+-+-+-+-+-+-+");
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.Append(this.FormatQuery(
                        stringDocument, sensitiveParameters));
                    if (this.context.Variables != null)
                    {
                        stringBuilder.Append(
                            this.FormatVariables(sensitiveVariables));
                    }
                }

                stringBuilder.Append("+-+-+-+-+-+-+-+-+-+");
                stringBuilder.Append(Environment.NewLine);
                var timeNeeded = queryTimer.Elapsed.TotalMilliseconds;
                stringBuilder.AppendFormat(
                    $"Time needed: {timeNeeded:0.#} milliseconds.");
                this.logger.LogInformation(stringBuilder.ToString());
            }

            private static string PaddingHelper(
                string existingString,
                int lengthToPadTo)
            {
                if (string.IsNullOrEmpty(existingString))
                {
                    return string.Empty.PadRight(lengthToPadTo);
                }

                if (existingString.Length > lengthToPadTo)
                {
                    return existingString.Substring(0, lengthToPadTo);
                }

                return existingString + " "
                    .PadRight(lengthToPadTo - existingString.Length);
            }

            private static List<string> GetSensitiveParameters(string document)
            {
                return SensitiveParameters
                    .Where(parameter =>
                        StringContains(document, parameter))
                    .ToList();
            }

            //Regex is:
            // {parameter}fcmToken}\s*:\s*\$(?<variableName>[^\s,\)]*)
            private static List<string> GetSensitiveVariables(
                string document, List<string> sensitiveParameters)
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
                string content, List<string> parameters)
            {
                string result = content;
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

            private static StringBuilder FormatVariable(
                StringBuilder stringBuilder,
                VariableValue variable,
                List<string> sensitiveVariables)
            {
                var variableName = variable.Name.ToString();
                var variableValue = variable.Value.ToString();
                if (sensitiveVariables.Contains(variableName))
                {
                    variableValue = "***";
                }

                var padName = PaddingHelper(variableName, 20);
                var padValue =
                    PaddingHelper(variableValue, 20);
                var type = variable.Type;
                stringBuilder.AppendFormat(
                    $"  {padName} : {padValue} : {type}");
                stringBuilder
                    .AppendFormat($"{Environment.NewLine}");
                return stringBuilder;
            }

            private StringBuilder FormatQuery(
                string document, List<string> sensitiveParameters)
            {
                StringBuilder stringBuilder =
                    new (
                        ReplaceSensitiveParameters(
                            this.context.Document!.ToString(true),
                            sensitiveParameters));
                return stringBuilder;
            }

            private StringBuilder FormatVariables(
                List<string> sensitiveVariables)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine();
                var variablesConcrete = this.context.Variables!.ToList();
                if (variablesConcrete.Count > 0)
                {
                    stringBuilder.Append("+-+-+-+-+-+-+-+-+-+");
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.
                        AppendFormat($"Variables: {Environment.NewLine}");
                    try
                    {
                        foreach (var variable in variablesConcrete)
                        {
                            stringBuilder = FormatVariable(
                                stringBuilder, variable, sensitiveVariables);
                        }
                    }
                    catch
                    {
                        // all input type records will land here.
                        stringBuilder.Append(
                            "  Formatting Variables Error. Continuing...");
                        stringBuilder.AppendFormat($"{Environment.NewLine}");
                    }
                }

                return stringBuilder;
            }
        }
    }
}
