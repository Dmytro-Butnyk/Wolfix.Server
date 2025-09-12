using System.Net;

namespace Shared.Domain.Models;

//todo: добавить DebuggerDisplay
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
    
    //todo: добавить проверки
    public static Result<TValue> Failure(Result<TValue> result)
        => new(result.ErrorMessage!, result.StatusCode);
    
    public static Result<TValue> Failure(VoidResult result)
        => new(result.ErrorMessage!, result.StatusCode);

    public TResult Map<TResult>(Func<TValue, TResult> onSuccess, Func<string, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(ErrorMessage!);
    }
}