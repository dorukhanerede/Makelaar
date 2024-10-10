namespace Makelaar.Functions.GetTopEstateAgents.Contracts.Models.Responses;

public record GetTopEstateAgentsResponse
{
    public int MakelaarId { get; init; }
    public string MakelaarName { get; init; }
    public int PropertyCount { get; init; }
}