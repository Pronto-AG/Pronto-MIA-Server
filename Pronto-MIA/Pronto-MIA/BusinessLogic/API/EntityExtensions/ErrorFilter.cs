namespace Pronto_MIA.BusinessLogic.API.EntityExtensions
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using HotChocolate;
    using Npgsql;

    /// <summary>
    /// Class which handles errors globbally before they are given to the query
    /// engine.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ErrorFilter : IErrorFilter
    {
        /// <inheritdoc/>
        public IError OnError(
            IError error)
        {
            error = this.SetTraceId(error);

            switch (error.Exception)
            {
                case null:
                    return error;
                case InvalidOperationException _
                    when error.Exception.InnerException is NpgsqlException:
                case NpgsqlException:
                    return error
                        .WithCode(DataAccess.Error.DatabaseOperationError
                            .ToString())
                        .WithMessage(
                            DataAccess.Error.DatabaseOperationError.Message());
                default:
                    return error
                        .WithCode(DataAccess.Error.UnknownError
                            .ToString())
                        .WithMessage(
                            DataAccess.Error.UnknownError.Message());
            }
        }

        private IError SetTraceId(IError error)
        {
            if (Activity.Current == null || Activity.Current?.Id == null)
            {
                return error;
            }

            var traceId = Activity.Current?.Id.Split("-")[1];

            if (string.IsNullOrEmpty(traceId))
            {
                return error;
            }

            return ErrorBuilder
                .FromError(error)
                .SetExtension(
                    "traceId",
                    Activity.Current?.Id.Split("-")[1])
                .Build();
        }
    }
}
