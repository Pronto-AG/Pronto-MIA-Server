namespace Pronto_MIA.BusinessLogic.API.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Helper class with methods used to format query logs.
    /// </summary>
    public static class QueryLoggerFormatter
    {
        /// <summary>
        /// Method to create a formatted query log string.
        /// </summary>
        /// <param name="query">The query to be logged.</param>
        /// <param name="variables">The variables to be included.</param>
        /// <param name="timeNeeded">The time that was needed to complete
        /// the query.</param>
        /// <returns>String representing a query log message.</returns>
        public static string FormatQuery(
            string query, List<QueryVariable> variables, double timeNeeded)
        {
            StringBuilder stringBuilder = new StringBuilder(
                "Request ended. Content was:");
            stringBuilder.AppendLine();
            stringBuilder.Append("+-+-+-+-+-+-+-+-+-+");
            stringBuilder.AppendLine();
            stringBuilder.Append(query);
            FormatVariables(stringBuilder, variables);
            stringBuilder.Append("+-+-+-+-+-+-+-+-+-+");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat(
                $"Time needed: {timeNeeded:0.#} milliseconds.");

            return stringBuilder.ToString();
        }

        private static void FormatVariables(
            StringBuilder stringBuilder,
            IReadOnlyCollection<QueryVariable> variables)
        {
            if (!variables.Any())
            {
                return;
            }

            stringBuilder.AppendLine();
            stringBuilder.Append("+-+-+-+-+-+-+-+-+-+");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat($"Variables: ");
            stringBuilder.AppendLine();
            try
            {
                foreach (var variable in variables)
                {
                    FormatVariable(stringBuilder, variable);
                }
            }
            catch
            {
                // all input type records will land here.
                stringBuilder.Append(
                    "  Formatting Variables Error. Continuing...");
                stringBuilder.AppendLine();
            }
        }

        private static void FormatVariable(
            StringBuilder stringBuilder,
            QueryVariable variable)
        {
            var padName = PaddingHelper(variable.Name, 20);
            var padValue =
                PaddingHelper(variable.Value, 20);
            stringBuilder.AppendFormat(
                $"  {padName} : {padValue} : {variable.Type}");
            stringBuilder
                .AppendFormat($"{Environment.NewLine}");
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
    }
}
