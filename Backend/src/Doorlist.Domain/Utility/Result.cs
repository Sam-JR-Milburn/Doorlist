namespace Doorlist.Domain.Utility;

public enum ErrorType
{
    None,
    Validation,             // 400
    Unauthorized,           // 401
    Forbidden,              // 403
    NotFound,               // 404
    Conflict,               // 409
    Unexpected,             // 500
    DependencyFailure,      // 502/503 (Keycloak, DB, External API down)
}

/// <summary>
/// ALlows you to define rich result objects for clean controller flow.
/// </summary>
/// <typeparam name="T">The type that you're expecting to return</typeparam>
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? ErrorMessage { get; }
    public ErrorType ErrorType { get; }

    protected Result(bool isSuccess, T? value, string? errorMessage, ErrorType errorType)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
    }

    public static Result<T> Success(T value) => new(true, value, null, ErrorType.None);
    public static Result<T> Failure(string message, ErrorType type) => new(false, default, message, type);
}