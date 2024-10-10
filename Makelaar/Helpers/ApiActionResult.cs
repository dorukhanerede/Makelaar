using System.Collections.Generic;
using System.Linq;
using System.Net;
using Makelaar.Contracts.Result;
using Microsoft.AspNetCore.Mvc;

namespace Makelaar.Helpers;

public static class ApiActionResult
{
    public static IActionResult Build<T>(Result<T> result, HttpStatusCode successCode = HttpStatusCode.OK)
    {
        var mappedErrors = result?.Errors?
            .Select(x => new ApiResultErrorObject { Text = x.Text })
            .ToList() ?? new List<ApiResultErrorObject>();

        switch (result)
        {
            case not null:
                var baseApiResponse = new BaseApiResult<T>
                {
                    Success = result.Success,
                    Errors = mappedErrors,
                    Data = result.Data
                };
                return CreateObjectResult<BaseApiResult<T>>(baseApiResponse, result.ErrorCode, successCode);

            default:
                var serverErrorResult = new BaseApiResult<T>
                {
                    Success = false,
                    Errors = mappedErrors
                };
                return CreateObjectResult<BaseApiResult<T>>(serverErrorResult, (int)HttpStatusCode.InternalServerError,
                    HttpStatusCode.InternalServerError);
        }
    }

    public static IActionResult Build(Result result)
    {
        return new ObjectResult(result)
        {
            StatusCode = result.Success ? (int)HttpStatusCode.OK : result.ErrorCode
        };
    }

    private static IActionResult CreateObjectResult<T>(BaseApiResult result, int errorCode, HttpStatusCode successCode)
    {
        if (result.Success == false)
        {
            return new ObjectResult(result)
            {
                StatusCode = errorCode
            };
        }

        return IsNoContentResultCodes(successCode)
            ? new StatusCodeResult((int)successCode)
            : new ObjectResult(result) { StatusCode = (int)successCode };
    }

    private static bool IsNoContentResultCodes(HttpStatusCode statusCode)
    {
        return statusCode is HttpStatusCode.NoContent or HttpStatusCode.NotFound;
    }
}