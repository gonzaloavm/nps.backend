using Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Common
{
    public static class ProblemDetailsMapper
    {
        private const string ErrorsKey = "errors";

        public static ProblemDetails ToProblemDetails(IEnumerable<Error>? errors, int status = StatusCodes.Status400BadRequest, string title = "One or more errors occurred")
        {
            var list = errors?
                .Select(e => new { code = e.Code, message = e.Message, field = e.Field })
                .Cast<object>()
                .ToList() ?? new List<object>();

            var pd = new ProblemDetails
            {
                Title = title,
                Status = status,
                Detail = errors?.FirstOrDefault()?.Message
            };

            pd.Extensions[ErrorsKey] = list;
            return pd;
        }

        public static void AddError(ProblemDetails pd, Error error)
        {
            if (error == null) return;

            if (pd.Extensions.TryGetValue(ErrorsKey, out var existing))
            {
                // Normaliza cualquier IEnumerable a List<object> y añade el nuevo error
                var list = existing as List<object>
                           ?? (existing as System.Collections.IEnumerable)?.Cast<object>().ToList()
                           ?? new List<object>();

                list.Add(new { code = error.Code, message = error.Message, field = error.Field });
                pd.Extensions[ErrorsKey] = list;
                return;
            }

            pd.Extensions[ErrorsKey] = new List<object> { new { code = error.Code, message = error.Message, field = error.Field } };
        }
    }

}
