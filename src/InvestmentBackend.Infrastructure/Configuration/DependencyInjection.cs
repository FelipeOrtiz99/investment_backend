using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.DependencyInjection;
using Amazon;
using Amazon.Runtime;
using InvestmentBackend.Domain.Repositories;
using InvestmentBackend.Infrastructure.Repositories;

namespace InvestmentBackend.Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string? accessKey = null, string? secretKey = null, string? region = null)
    {
        // Configure DynamoDB
        services.AddSingleton<IAmazonDynamoDB>(provider =>
        {
            var config = new AmazonDynamoDBConfig();
            
            // Set region
            if (!string.IsNullOrEmpty(region))
            {
                config.RegionEndpoint = RegionEndpoint.GetBySystemName(region);
            }
            else
            {
                config.RegionEndpoint = RegionEndpoint.USEast2; // Default
            }

            // Create client with credentials if provided
            if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            {
                var credentials = new BasicAWSCredentials(accessKey, secretKey);
                return new AmazonDynamoDBClient(credentials, config);
            }
            
            // Use default credential provider chain if no credentials provided
            return new AmazonDynamoDBClient(config);
        });

        services.AddSingleton<IDynamoDBContext>(provider =>
        {
            var client = provider.GetRequiredService<IAmazonDynamoDB>();
            return new DynamoDBContextBuilder()
                .WithDynamoDBClient(() => client)
                .Build();
        });

        // Register repositories
        services.AddScoped<IClientRepository, DynamoDbClientRepository>();
        services.AddScoped<ICurrencyRepository, DynamoDbCurrencyRepository>();
        services.AddScoped<IInvestmentFundRepository, DynamoDbInvestmentFundRepository>();
        services.AddScoped<ITransactionRepository, DynamoDbTransactionRepository>();

        return services;
    }
}
