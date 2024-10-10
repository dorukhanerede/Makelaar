using System.Threading;
using System.Threading.Tasks;
using Makelaar.Contracts.Result;
using RestSharp;

namespace Makelaar.Clients.FundaClient;

public interface IFundaClient
{
    Task<Result<T>> ExecuteAsync<T>(RestRequest request, CancellationToken cancellationToken) where T : new();
}