using System.Text.Json.Serialization;

namespace Takas.Api.DTOs.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? Errors { get; set; }

    [JsonIgnore]
    public int StatusCode { get; set; }

    public static ApiResponse<T> SuccessResponse(T? data, string message, int statusCode = StatusCodes.Status200OK)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = null,
            StatusCode = statusCode
        };
    }

    public static ApiResponse<T> FailResponse(string message, int statusCode, IEnumerable<string>? errors = null, T? data = default)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = data,
            Errors = errors?.ToList() ?? new List<string>(),
            StatusCode = statusCode
        };
    }
}
