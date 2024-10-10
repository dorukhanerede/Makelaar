namespace Makelaar.Functions.GetTopEstateAgents.Contracts.Models.Requests;

public record GetTopEstateAgentsRequest
{
    public string? Text { get; init; }
    public string City { get; init; } = null!;
    public string Type { get; init; } = null!;
    public int Count { get; init; }
}