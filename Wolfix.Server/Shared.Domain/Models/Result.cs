using System.Net;

namespace Shared.Domain.Models;

public sealed class Result<TValue>
{
    public TValue? Value { get; }
    public string? ErrorMessage { get; }
    public bool IsSuccess => ErrorMessage == null;
    public HttpStatusCode StatusCode { get; }

    private Result(TValue value, HttpStatusCode statusCode)
    {
        Value = value;
        ErrorMessage = null;
        StatusCode = statusCode;
    }

    private Result(string errorMessage, HttpStatusCode statusCode)
    {
        Value = default;
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
    }

    public static Result<TValue> Success(TValue value, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(value, statusCode);

    public static Result<TValue> Failure(string errorMessage, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new(errorMessage, statusCode);

    public TResult Map<TResult>(Func<TValue, TResult> onSuccess, Func<string, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(ErrorMessage!);
    }
}