using System.Collections.Generic;

namespace Makelaar.Contracts.Result;

public class BaseApiResult
{
    public bool Success { get; init; }
    public List<ApiResultErrorObject>? Errors { get; init; }
}

public class BaseApiResult<T> : BaseApiResult
{
    public T? Data { get; init; }
}