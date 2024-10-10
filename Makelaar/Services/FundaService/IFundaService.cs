using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Makelaar.Contracts.Result;
using Makelaar.Functions.GetTopEstateAgents.Contracts.Models.Requests;
using Makelaar.Services.FundaService.Contracts.Models.Shared;

namespace Makelaar.Services.FundaService;

public interface IFundaService
{
    Task<Result<List<Property>>> GetAllPropertiesAsync(GetTopEstateAgentsRequest request, CancellationToken cancellationToken);
}