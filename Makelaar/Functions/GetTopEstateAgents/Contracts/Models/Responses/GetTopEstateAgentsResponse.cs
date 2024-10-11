namespace Makelaar.Functions.GetTopEstateAgents.Contracts.Models.Responses;

public record GetTopEstateAgentsResponse
{
    public int EstateAgentId { get; init; }
    public string EstateAgentName { get; init; }
    public int PropertyCount { get; init; }
}