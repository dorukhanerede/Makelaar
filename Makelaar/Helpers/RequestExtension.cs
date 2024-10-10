using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using Makelaar.Contracts.Result;

namespace Makelaar.Helpers;

public static class RequestExtension
{
    public static async Task<Result> ValidateWith<T, V>(this T request, V validator)
        where V : AbstractValidator<T>
        where T : class
    {
        var result = await validator.ValidateAsync(request);

        return result.IsValid switch
        {
            false => Result.CreateFailure(HttpStatusCode.BadRequest, result.Errors.Select(error => new ErrorObject { Text = error.ErrorMessage })),
            true => Result.CreateSuccess()
        };
    }
}