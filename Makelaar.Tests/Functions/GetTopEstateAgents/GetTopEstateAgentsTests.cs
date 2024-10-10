using AutoFixture;
using FluentAssertions;
using Makelaar.Contracts.Result;
using Makelaar.Functions.GetTopEstateAgents.Contracts.Models.Requests;
using Makelaar.Functions.GetTopEstateAgents.Contracts.Models.Responses;
using Makelaar.Services.FundaService;
using Makelaar.Services.FundaService.Contracts.Models.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;

namespace Makelaar.Tests.Functions.GetTopEstateAgents;

public class GetTopEstateAgentsTests
{
    private readonly Fixture _fixture = new();
    private readonly Mock<IFundaService> _fundaServiceMock = new();
    private readonly Mock<ILogger<Makelaar.Functions.GetTopEstateAgents.GetTopEstateAgents>> _loggerMock = new();
    private readonly Makelaar.Functions.GetTopEstateAgents.GetTopEstateAgents _getTopEstateAgentsFunction;
    
    public GetTopEstateAgentsTests()
    {
        _getTopEstateAgentsFunction = new Makelaar.Functions.GetTopEstateAgents.GetTopEstateAgents(_fundaServiceMock.Object, _loggerMock.Object);
    }
    
    [Fact]
    public async Task Run_ShouldReturnTopMakelaars_WhenRequestIsSuccessful()
    {

        var request = new Mock<HttpRequest>();
        var queryCollection = new QueryCollection(new Dictionary<string, StringValues>
        {
            { "text", "tuin" },
            { "city", "amsterdam" },
            { "type", "koop" },
            { "count", "10" }
        });
        request.Setup(req => req.Query).Returns(queryCollection);
        
        var expectedProperties = _fixture.Build<Property>()
            .CreateMany(20)
            .ToList();

        var fundaServiceResult = Result<List<Property>>.CreateSuccess(expectedProperties);
        _fundaServiceMock.Setup(s => s.GetAllPropertiesAsync(It.IsAny<GetTopEstateAgentsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fundaServiceResult);

        // Act
        var result = await _getTopEstateAgentsFunction.Run(request.Object, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ObjectResult>();
        var okResult = result as ObjectResult;
        var response = okResult?.Value as Result<List<GetTopEstateAgentsResponse>>;
        response?.Data.Should().NotBeNull();
        response?.Data.Should().HaveCount(10);
    }
}