using System.Diagnostics;
using System.Net;

namespace Shared.Domain.Models;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Result<TValue>
{
    private string DebuggerDisplay =>
        IsSuccess
        ? $"Success: {Value}"
        : $"Failure: {ErrorMessage} StatusCode: {StatusCode}";
    
    public TValue? Value { get; }
    public string? ErrorMessage { get; }
    public bool IsSuccess => ErrorMessage == null;
    public bool IsFailure => !IsSuccess;
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
    
    public static Result<TValue> Failure(Result<TValue> result)
    {
        if (result.IsSuccess) throw new ArgumentException("Result is success", nameof(result));
        return new Result<TValue>(result.ErrorMessage!, result.StatusCode);
    }

    public static Result<TValue> Failure(VoidResult result)
    {
        if (result.IsSuccess) throw new ArgumentException("Result is success", nameof(result));
        return new Result<TValue>(result.ErrorMessage!, result.StatusCode);
    }

    public TResult Map<TResult>(Func<TValue, TResult> onSuccess, Func<string, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(ErrorMessage!);
    }
}