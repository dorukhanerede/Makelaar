using System.Collections.Generic;
using Makelaar.Services.FundaService.Contracts.Models.Shared;

namespace Makelaar.Services.FundaService.Contracts.Models.Responses;

public sealed record GetPropertyListingResponse : BaseFundaApiResponse<List<Property>>
{
    // add more properties here if necessary for the response
}