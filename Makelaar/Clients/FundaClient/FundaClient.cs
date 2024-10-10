using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Makelaar.Contracts.Result;
using Makelaar.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.RateLimit;
using Polly.Wrap;
using RestSharp;

namespace Makelaar.Clients.FundaClient;

public class FundaClient : IFundaClient
{
    private readonly RestClient _restClient;
    private readonly ILogger<FundaClient> _logger;
    private readonly AsyncPolicyWrap _policy;
    
    public FundaClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<FundaClient> logger)
    {
        _logger = logger;
        var fundaApiBaseUrl = configuration.RetrieveConfigurationValue("FundaApiBaseUrl");
        var apiKey = configuration.RetrieveConfigurationValue("FundaApiKey");

        var httpClientInstance = httpClientFactory.CreateClient(nameof(FundaClient));
        _restClient = new RestClient(httpClientInstance, new RestClientOptions($"{fundaApiBaseUrl}{apiKey}/"));
        
        var rateLimitPolicy = Policy.RateLimitAsync(100, TimeSpan.FromMinutes(1));
        var retryPolicy = Policy.Handle<WebException>()
            .Or<HttpRequestException>()
            .WaitAndRetryAsync(new[] { TimeSpan.FromMinutes(1) });
        
        _policy = Policy.WrapAsync(rateLimitPolicy, retryPolicy);
        
        _logger.LogInformation("FundaClient initialized.");
    }
    
    public async Task<Result<T>> ExecuteAsync<T>(RestRequest request, CancellationToken cancellationToken)
        where T : new()
    {
        var restResponseContent = string.Empty;
        try
        {
            var restResponse = await _policy
                .ExecuteAsync(() => _restClient.ExecuteAsync(request, cancellationToken));
            
            restResponseContent = restResponse.Content ?? string.Empty;
            if (restResponse.IsSuccessful)
            {
                var response = JsonConvert.DeserializeObject<T>(restResponseContent);
                return Result<T>.CreateSuccess(response);
            }

            var errorObject = JsonConvert.DeserializeObject<Result>(restResponseContent);
            return Result<T>.CreateFailure(restResponse.StatusCode, errorObject?.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to send request. Exception: {exception}", ex.Message);

            return ex switch
            {
                JsonSerializationException => Result<T>.CreateFailure(HttpStatusCode.InternalServerError,
                    new ErrorObject { Text = $"Failed to deserialize response => {restResponseContent}" }),
                
                RateLimitRejectedException => Result<T>.CreateFailure(HttpStatusCode.TooManyRequests,
                    new ErrorObject { Text = "Rate limit exceeded. Please try again later." }),

                _ => Result<T>.CreateFailure(HttpStatusCode.InternalServerError,
                    new ErrorObject { Text = "An error occurred while sending the request." })
            };
        }
    }
}