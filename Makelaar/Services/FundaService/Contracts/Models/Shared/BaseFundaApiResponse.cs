namespace Makelaar.Services.FundaService.Contracts.Models.Shared;

public class BaseFundaApiResponse<T>
{
    public Paging Paging { get; set; }
    public T Objects { get; set; }
    // add more properties here if necessary for the base response
    // for now we only need the paging and the objects
}