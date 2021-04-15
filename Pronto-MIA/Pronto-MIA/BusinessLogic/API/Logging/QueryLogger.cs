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

        static QueryLogger()
        {
            var assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();
            var parameters = assemblyTypes
                .SelectMany(x => x.GetMethods())
                .SelectMany(x => x.GetCustomAttributes<SensitiveAttribute>())
                .Select(x => x.QueryPropertyName)
                .Distinct().ToList();

            SensitiveParameters = new string[parameters.Count];
            int index = 0;
            foreach (var parameter in parameters)
            {
                SensitiveParameters[index] = parameter;
                ++index;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryLogger"/>
        /// class.
        /// </summary>
        /// <param name="logger">The logger to be used.</param>
        public QueryLogger(ILogger<QueryLogger> logger)
        {
            this.logger = logger;
        }

        public override IActivityScope ExecuteRequest(IRequestContext context)
        {
            return new RequestScope(this.logger, context);
        }

        private static string ReplaceSensitiveInfo(string content)
        {
            string result = content;
            foreach (var parameter in SensitiveParameters)
            {
                result = Regex.Replace(
                    result,
                    $"{parameter}\\s*:\\s*(?<!\\\\)(?:\\\\{{2}})*\"(?<content>(?:(?<!\\\\)(?:\\\\{{2}})*\\\\\"|[^\"])*(?<!\\\\)(?:\\\\{{2}})*)\"",
                    $"{parameter}:\"***\"");
            }

            return result;
        }

        private class RequestScope : IActivityScope
        {
            private readonly IRequestContext context;
            private readonly ILogger<QueryLogger> logger;
            private readonly Guid uuid;

            public RequestScope
                (ILogger<QueryLogger> logger,
                     IRequestContext context)
            {
                this.logger = logger;
                this.context = context;
                this.uuid = Guid.NewGuid();
                StringBuilder stringBuilder = new StringBuilder(
                        "Query " + this.uuid.ToString() + " started")
                    .Append(Environment.NewLine);

                if (this.context.Document is not null)
                {
                    stringBuilder.Append(this.FormatQuery());

                    if (this.context.Variables != null)
                    {
                        stringBuilder.Append(this.FormatVariables());
                    }
                }

                this.logger.LogInformation(stringBuilder.ToString());

                queryTimer = new Stopwatch();
                queryTimer.Start();
            }

            public void Dispose()
            {
                this.logger.LogInformation(
                    "Query " + this.uuid.ToString() + " started");
                if (this.context.Document is not null)
                {
                    StringBuilder stringBuilder =
                        new (this.context.Document.ToString(true));

                    stringBuilder.AppendLine();
                    if (this.context.Variables != null)
                    {
                        
                    }
                    queryTimer.Stop();
                    stringBuilder.AppendFormat(
                        $"Ellapsed time for query is {queryTimer.Elapsed.TotalMilliseconds:0.#} milliseconds.");

                    var timer = new Stopwatch();
                    timer.Start();
                    this.logger.LogInformation(
                        ReplaceSensitiveInfo(
                            stringBuilder.ToString()));
                    timer.Stop();
                    this.logger.LogInformation("Regex took: " +
                                               timer.ElapsedMilliseconds
                                               + "ms");
                }
            }

            private StringBuilder FormatQuery()
            {
                StringBuilder stringBuilder =
                    new (
                        ReplaceSensitiveInfo(
                            this.context.Document.ToString(true)));
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
                        AppendFormat($"Variables {Environment.NewLine}");
                    try
                    {
                        foreach (var variableValue in context.Variables!)
                        {
                            string PadRightHelper
                                (string existingString, int lengthToPadTo)
                            {
                                if (string.IsNullOrEmpty(existingString))
                                    return "".PadRight(lengthToPadTo);
                                if (existingString.Length > lengthToPadTo)
                                    return existingString.Substring(0, lengthToPadTo);
                                return existingString + " ".PadRight(lengthToPadTo - existingString.Length);
                            }
                            stringBuilder.AppendFormat(
                                $"  {PadRightHelper(variableValue.Name, 20)} :  {PadRightHelper(variableValue.Value.ToString(), 20)}: {variableValue.Type}");
                            stringBuilder.AppendFormat($"{Environment.NewLine}");
                        }
                    }
                    catch
                    {
                        // all input type records will land here.
                        stringBuilder.Append("  Formatting Variables Error. Continuing...");
                        stringBuilder.AppendFormat($"{Environment.NewLine}");
                    }
                }

                return stringBuilder;
            }
        }
    }
}
