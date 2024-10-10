using FluentValidation;
using Makelaar.Functions.GetTopEstateAgents.Contracts.Models.Requests;

namespace Makelaar.Functions.GetTopEstateAgents.Contracts.Validators;

public class RequestValidator : AbstractValidator<GetTopEstateAgentsRequest>
{
    public RequestValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required.");

        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("Type is required.");

        RuleFor(x => x.Count)
            .GreaterThan(0)
            .WithMessage("Count must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Count cannot exceed 100.");
    }
}