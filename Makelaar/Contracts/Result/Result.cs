using System.Collections.Generic;
using System.Net;

namespace Makelaar.Contracts.Result;

public record Result
{
    public bool Success { get; init; }
    public int ErrorCode { get; set; }
    public List<ErrorObject>? Errors { get; set; }

    public static Result CreateSuccess()
    {
        return new Result
        {
            Success = true
        };
    }
    
    public static Result CreateFailure(HttpStatusCode httpStatusCode, IEnumerable<ErrorObject>? errors)
    {
        return CreateFailure((int)httpStatusCode, errors);
    }
    
    public static Result CreateFailure(int errorCode, IEnumerable<ErrorObject>? errors)
    {
        return new Result
        {
            Success = false,
            Errors = errors == null ? new List<ErrorObject>() : new List<ErrorObject>(errors),
            ErrorCode = errorCode
        };
    }
}

public record Result<T> : Result
{
    public T? Data { get; init; }
    
    public static implicit operator Result<T>(T data)
    {
        return CreateSuccess(data);
    }
    
    public static Result<T> CreateSuccess(T data)
    {
        return new Result<T>
        {
            Success = true,
            Data = data
        };
    }
    
    public new static Result<T> CreateFailure(HttpStatusCode httpStatusCode, ErrorObject error)
    {
        return CreateFailure((int)httpStatusCode, error);
    }
    
    public new static Result<T> CreateFailure(int errorCode, ErrorObject error)
    {
        return CreateFailure(errorCode, new List<ErrorObject> { error });
    }
    
    public new static Result<T> CreateFailure(HttpStatusCode httpStatusCode, IEnumerable<ErrorObject>? errors)
    {
        return CreateFailure((int)httpStatusCode, errors);
    }
    
    public new static Result<T> CreateFailure(int errorCode, IEnumerable<ErrorObject>? errors)
    {
        return new Result<T>
        {
            Success = false,
            Errors = errors == null ? new List<ErrorObject>() : new List<ErrorObject>(errors),
            ErrorCode = errorCode
        };
    }
}