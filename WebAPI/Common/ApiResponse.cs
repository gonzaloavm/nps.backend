using Domain.Common;
using System.Text.Json.Serialization;

namespace WebAPI.Common
{
    #region API RESPONSE

    public record ApiResponse<T>
    {
        public bool Success { get; init; } = true;
        public T? Data { get; init; }

        protected ApiResponse() { }

        public static ApiResponse<T> Ok(T? data = default) =>
            new() { Success = true, Data = data };
    }

    public record ApiResponse<T, TMeta> : ApiResponse<T>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TMeta? Meta { get; init; }

        protected ApiResponse() { }

        public static ApiResponse<T, TMeta> Ok(T? data = default, TMeta? meta = default) =>
            new() { Success = true, Data = data, Meta = meta };
    }


    #endregion

    #region METADATA

    public record MetaDataPagination(
        int? PageNumber = null,
        int? PageSize = null,
        int? TotalCount = null,
        int? TotalPages = null
    );

    #endregion


}
