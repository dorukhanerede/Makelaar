using AutoFixture;
using FluentAssertions;
using Makelaar.Clients.FundaClient;
using Makelaar.Functions.GetTopEstateAgents.Contracts.Models.Requests;
using Makelaar.Services.FundaService.Contracts.Models.Responses;
using Makelaar.Services.FundaService.Contracts.Models.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using RestSharp;

namespace Makelaar.Tests.Services.FundaService;

public class FundaServiceTests
{
    private readonly Mock<IFundaClient> _fundaClientMock = new();
    private readonly Mock<ILogger<Makelaar.Services.FundaService.FundaService>> _loggerMock = new();
    private readonly Makelaar.Services.FundaService.FundaService _fundaService;
    private readonly Fixture _fixture = new();

    public FundaServiceTests()
    {
        _fundaService = new Makelaar.Services.FundaService.FundaService(_fundaClientMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllPropertiesAsync_ShouldReturnSuccess_WhenApiCallSucceeds()
    {
        // Arrange
        var request = _fixture.Create<GetTopEstateAgentsRequest>();

        var properties = _fixture.CreateMany<Property>(5).ToList();

        var fundaClientResponse = _fixture.Build<GetPropertyListingResponse>()
            .With(x => x.Objects, properties)
            .Create();
        _fundaClientMock.Setup(x =>
                x.ExecuteAsync<GetPropertyListingResponse>(It.IsAny<RestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundaClientResponse);
        
        // Act
        var result = await _fundaService.GetAllPropertiesAsync(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(5);
    }
}