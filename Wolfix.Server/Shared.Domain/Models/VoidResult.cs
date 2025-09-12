using System.Net;

namespace Shared.Domain.Models;

//todo: добавить DebuggerDisplay
public sealed class VoidResult
{
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
    
    //todo: добавить проверки
    public static VoidResult Failure(VoidResult result)
        => new(result.ErrorMessage!, result.StatusCode);
    
    public static VoidResult Failure<TValue>(Result<TValue> result)
        => new(result.ErrorMessage!, result.StatusCode);

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