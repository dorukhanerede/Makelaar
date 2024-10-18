using System.Collections.Generic;
using System.Linq;
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
        restRequest.AddQueryParameter("zo", $"/{request.City}/{request.Text}");
        restRequest.AddQueryParameter(nameof(request.Type), request.Type);
        restRequest.AddQueryParameter("pagesize", "25");
        restRequest.AddQueryParameter("page", 1);

        var initialResponse =
            await _fundaClient.ExecuteAsync<GetPropertyListingResponse>(restRequest, cancellationToken);

        if (!initialResponse.Success)
        {
            return Result<List<Property>>.CreateFailure(initialResponse.ErrorCode, initialResponse.Errors);
        }

        var totalPages = initialResponse.Data!.Paging.AantalPaginas;
        var properties = new List<Property>(initialResponse.Data.Objects);

        var pageRange = Enumerable.Range(2, totalPages - 1);

        await Parallel.ForEachAsync(pageRange, new ParallelOptions
            {
                MaxDegreeOfParallelism = 5,
                CancellationToken = cancellationToken
            },
            async (page, ct) =>
            {
                var newRequest = new RestRequest();
                newRequest.AddQueryParameter("zo", $"/{request.City}/{request.Text}");
                newRequest.AddQueryParameter(nameof(request.Type), request.Type);
                newRequest.AddQueryParameter("pagesize", "25");
                newRequest.AddOrUpdateParameter("page", page);

                var response = await _fundaClient.ExecuteAsync<GetPropertyListingResponse>(newRequest, ct);

                if (response.Success)
                {
                    lock (properties)
                    {
                        properties.AddRange(response.Data!.Objects);
                    }
                }
                else
                {
                    _logger.LogWarning("Failed to fetch page {page}: {errors}", page, response.Errors);
                }
            });

        return Result<List<Property>>.CreateSuccess(properties);
    }
}