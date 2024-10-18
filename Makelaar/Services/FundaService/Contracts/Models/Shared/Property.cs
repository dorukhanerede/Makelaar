namespace Makelaar.Services.FundaService.Contracts.Models.Shared;

public record Property
{
    public int MakelaarId { get; init; }
    public string? MakelaarNaam { get; init; }
}