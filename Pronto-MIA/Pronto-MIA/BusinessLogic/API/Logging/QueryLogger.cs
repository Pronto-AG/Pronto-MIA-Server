namespace Pronto_MIA.BusinessLogic.API.Logging
{
    using System;
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

        // Regex is:
        // {parameter}\s*:\s*(?<!\\)(?:\\{{2}})*"(?<content>(?:(?<!\\)(?:\\{{2}}
        //      )*\\"|[^"])*(?<!\\)(?:\\{{2}})*)"
        private static string ReplaceSensitiveInfo(string content)
        {
            string result = content;
            foreach (var parameter in SensitiveParameters)
            {
                result = Regex.Replace(
                    result,
                    new StringBuilder(parameter)
                        .Append("\\s*:\\s*(?<!\\\\)(?:\\\\{{2}})*\"")
                        .Append("(?<content>(?:(?<!\\\\)(?:\\\\{{2}})*\\\\\"|")
                        .Append("[^\"])*(?<!\\\\)(?:\\\\{{2}})*)\"")
                        .ToString(),
                    $"{parameter}:\"***\"");
            }

            return result;
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
                /*this.uuid = Guid.Parse(
                    context.Services
                        .GetApplicationServices()
                        .GetService<ISessionIdAccessor>()
                        ?.GetSessionId() ?? string.Empty)*/

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
                    stringBuilder.Append("###");
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.Append(this.FormatQuery());

                    if (this.context.Variables != null)
                    {
                        stringBuilder.Append(this.FormatVariables());
                    }
                }

                stringBuilder.Append(Environment.NewLine);
                var timeNeeded = queryTimer.Elapsed.TotalMilliseconds;
                stringBuilder.AppendFormat(
                    $"Time needed: {timeNeeded:0.#} milliseconds.");
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append("###");
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

            private StringBuilder FormatQuery()
            {
                StringBuilder stringBuilder =
                    new (
                        ReplaceSensitiveInfo(
                            this.context.Document!.ToString(true)));
                return stringBuilder;
            }

            private StringBuilder FormatVariables()
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine();

                var variablesConcrete =
                            this.context.Variables!.ToList();
                if (variablesConcrete.Count > 0)
                {
                    stringBuilder.
                        AppendFormat($"Variables: {Environment.NewLine}");
                    try
                    {
                        foreach (var variable in this.context.Variables!)
                        {
                            stringBuilder =
                                this.FormatVariable(stringBuilder, variable);
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

            private StringBuilder FormatVariable(
                StringBuilder stringBuilder, VariableValue variable)
            {
                var variableName = variable.Name.ToString();
                var variableValue = "***";

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
        }
    }
}
