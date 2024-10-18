namespace Makelaar.Services.FundaService.Contracts.Models.Shared;

public abstract record BaseFundaApiResponse<T>
{
    public Paging Paging { get; init; }
    public T Objects { get; init; }
    public int TotaalAantalObjecten { get; init; }
    // add more properties here if necessary for the base response
    // for now we only need the paging and the objects
}