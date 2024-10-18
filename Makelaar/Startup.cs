using Makelaar;
using Makelaar.Clients.FundaClient;
using Makelaar.Services.FundaService;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Makelaar;

public class Startup : FunctionsStartup
{
    private IConfiguration Config { get; set; }

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
#if DEBUG
        Config = builder.ConfigurationBuilder
            .AddJsonFile("local.settings.json", true)
            .AddEnvironmentVariables()
            .Build();
#else
        Config = builder.ConfigurationBuilder
            .AddEnvironmentVariables()
            .Build();
#endif
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton(Config);
        builder.Services.AddSingleton<IFundaClient, FundaClient>();
        builder.Services.AddTransient<IFundaService, FundaService>();
    }
}