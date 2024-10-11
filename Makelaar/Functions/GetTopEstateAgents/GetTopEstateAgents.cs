using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Makelaar.Contracts.Result;
using Makelaar.Functions.GetTopEstateAgents.Contracts.Models.Requests;
using Makelaar.Functions.GetTopEstateAgents.Contracts.Models.Responses;
using Makelaar.Functions.GetTopEstateAgents.Contracts.Validators;
using Makelaar.Helpers;
using Makelaar.Services.FundaService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Makelaar.Functions.GetTopEstateAgents;

public sealed class GetTopEstateAgents
{
    private readonly IFundaService _fundaService;
    private readonly ILogger<GetTopEstateAgents> _logger;

    public GetTopEstateAgents(IFundaService fundaService, ILogger<GetTopEstateAgents> logger)
    {
        _fundaService = fundaService;
        _logger = logger;
    }

    [FunctionName("GetTopEstateAgents")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "topagents")]
        HttpRequest req,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetTopEstateAgents function processed a request.");

        var request = new GetTopEstateAgentsRequest
        {
            Text = req.Query["text"],
            City = !string.IsNullOrWhiteSpace(req.Query["city"]) ? req.Query["city"] : "amsterdam",
            Type = !string.IsNullOrWhiteSpace(req.Query["type"]) ? req.Query["type"] : "koop",
            Count = int.TryParse(req.Query["count"], out var count) ? count : 10
        };

        var validationResult = await request.ValidateWith(new RequestValidator());
        if (!validationResult.Success)
        {
            _logger.LogError("Validation failed: {errors}", validationResult.Errors);
            return ApiActionResult.Build(validationResult);
        }

        var result = await _fundaService.GetAllPropertiesAsync(request, cancellationToken);
        if (!result.Success)
        {
            _logger.LogError("FundaService failed. Error Code: {errorCode} - Errors: {errors}", result.ErrorCode, result.Errors);
            return ApiActionResult.Build(result);
        }

        var properties = result.Data;
        var topEstateAgents = properties
            .GroupBy(m => m.MakelaarId)
            .Select(g => new GetTopEstateAgentsResponse
            {
                EstateAgentId = g.Key,
                EstateAgentName = g.FirstOrDefault()?.MakelaarNaam,
                PropertyCount = g.Count()
            })
            .OrderByDescending(x => x.PropertyCount)
            .Take(request.Count)
            .ToList();

        return ApiActionResult.Build(Result<List<GetTopEstateAgentsResponse>>.CreateSuccess(topEstateAgents));
    }
}