using System.Diagnostics;
using System.Net;

namespace Shared.Domain.Models;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class VoidResult
{
    private string DebuggerDisplay =>
        IsSuccess
        ? "Success"
        : $"Failure: {ErrorMessage} StatusCode: {StatusCode}";
    
    public bool IsSuccess => ErrorMessage == null;
    public string? ErrorMessage { get; }
    public HttpStatusCode StatusCode { get; }

    private VoidResult(string? errorMessage, HttpStatusCode statusCode)
    {
        ErrorMessage = errorMessage;
        StatusCode = statusCode;
    }

    public static VoidResult Success(HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(null, statusCode);

    public static VoidResult Failure(string errorMessage, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new(errorMessage, statusCode);
    
    public static VoidResult Failure(VoidResult failedResult)
    {
        if (failedResult.IsSuccess) throw new ArgumentException("Result is success", nameof(failedResult));
        return new VoidResult(failedResult.ErrorMessage!, failedResult.StatusCode);
    }

    public static VoidResult Failure<TValue>(Result<TValue> failedResult)
    {
        if (failedResult.IsSuccess) throw new ArgumentException("Result is success", nameof(failedResult));
        return new VoidResult(failedResult.ErrorMessage!, failedResult.StatusCode);
    }

    public TResult Map<TResult>(Func<TResult> onSuccess, Func<string, TResult> onFailure)
        => IsSuccess ? onSuccess() : onFailure(ErrorMessage!);

    //todo
    // public void Map(Action onSuccess, Action<string> onFailure)
    // {
    //     if (IsSuccess)
    //     {
    //         onSuccess();
    //     }
    //     else
    //     {
    //         onFailure(ErrorMessage!);
    //     }
    // }
}