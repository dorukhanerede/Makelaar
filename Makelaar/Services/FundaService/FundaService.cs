using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Makelaar.Clients.FundaClient;
using Makelaar.Contracts.Result;
using Makelaar.Functions.GetTopEstateAgents.Contracts.Models.Requests;
using Makelaar.Services.FundaService.Contracts.Models.Responses;
using Makelaar.Services.FundaService.Contracts.Models.Shared;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Makelaar.Services.FundaService;

public class FundaService : IFundaService
{
    private readonly ILogger<FundaService> _logger;
    private readonly IFundaClient _fundaClient;

    public FundaService(IFundaClient fundaClient, ILogger<FundaService> logger)
    {
        _logger = logger;
        _fundaClient = fundaClient;
        _logger.LogInformation("FundaService initialized.");
    }

    public async Task<Result<List<Property>>> GetAllPropertiesAsync(GetTopEstateAgentsRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("FundaService.GetAllPropertiesAsync called with request: {@request}", request);
        var restRequest = new RestRequest();
        
        restRequest.AddQueryParameter("zo",
            $"/{request.City}/{request.Text}");
        restRequest.AddQueryParameter(nameof(request.Type),
            request.Type);
        restRequest.AddQueryParameter("page", "1");
        restRequest.AddQueryParameter("pagesize", "25");
        
        var response = await _fundaClient.ExecuteAsync<GetPropertyListingResponse>(restRequest, cancellationToken);
        if (response.Success == false)
        {
            return Result<List<Property>>.CreateFailure(response.ErrorCode, response.Errors);
        }

        return Result<List<Property>>.CreateSuccess(response.Data.Objects);
    }
}