namespace Eman.Application.Common;

/// <summary>
/// Cấu trúc phản hồi thống nhất của API EMAN.
/// </summary>
public sealed class ApiResponse<T>
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public T? Data { get; init; }

    public static ApiResponse<T> Ok(T data, string message = "Thành công")
        => new()
        {
            Success = true,
            Message = message,
            Data = data
        };

    public static ApiResponse<T> Fail(string message, T? data = default)
        => new()
        {
            Success = false,
            Message = message,
            Data = data
        };
}
