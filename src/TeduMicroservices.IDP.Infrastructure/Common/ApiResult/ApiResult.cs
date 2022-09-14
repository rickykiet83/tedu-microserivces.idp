using Microsoft.AspNetCore.Mvc;

namespace  TeduMicroservices.IDP.Infrastructure.Common.ApiResult;

public class ApiResult<T> : IActionResult
{
    public ApiResult()
    {
    }

    public ApiResult(bool isSucceeded, string? message = null)
    {
        Message = message;
        IsSucceeded = isSucceeded;
    }

    public ApiResult(bool isSucceeded, T result, string? message = null)
    {
        Result = result;
        Message = message;
        IsSucceeded = isSucceeded;
    }

    public bool IsSucceeded { get; set; }
    public string? Message { get; set; }
    public T Result { get; set; }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var objectResult = new ObjectResult(this);

        await objectResult.ExecuteResultAsync(context);
    }
}