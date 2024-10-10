namespace Makelaar.Services.FundaService.Contracts.Models.Shared;

public record Paging
{
    public int AantalPaginas { get; init; }
    public int HuidigePagina { get; init; }
    public string? VolgendeUrl { get; init; }
    public string? VorigeUrl { get; init; }
}