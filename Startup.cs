using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Demo.Function.Domain.Interface;
using Demo.Function.Domain.Repository;

[assembly: FunctionsStartup(typeof(Demo.Function.Startup))]
namespace Demo.Function;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        _ = builder.Services.AddSingleton(provider => new CosmosClient(
            Environment.GetEnvironmentVariable("CosmosDbUri"),
            Environment.GetEnvironmentVariable("CosmosDbPrimaryKey")
        ));

        _ = builder.Services.AddSingleton<IPersonRepo, PersonRepo>(provider => new PersonRepo(
            db: provider.GetRequiredService<CosmosClient>().GetDatabase(Environment.GetEnvironmentVariable("CosmosDbName"))
        ));
    }
}