using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using InvestmentBackend.Domain.Entities;
using InvestmentBackend.Domain.Repositories;

namespace InvestmentBackend.Infrastructure.Repositories;

public class DynamoDbClientRepository : IClientRepository
{
    private readonly IDynamoDBContext _dynamoDbContext;
    private readonly IAmazonDynamoDB _dynamoDbClient;

    public DynamoDbClientRepository(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB dynamoDbClient)
    {
        _dynamoDbContext = dynamoDbContext;
        _dynamoDbClient = dynamoDbClient;
    }

    public async Task<Client?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _dynamoDbContext.LoadAsync<Client>(id, cancellationToken);
    }

    public async Task<IEnumerable<Client>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var scan = _dynamoDbContext.ScanAsync<Client>(new List<ScanCondition>());
            return await scan.GetRemainingAsync(cancellationToken);
        }
        catch (ResourceNotFoundException)
        {
            await EnsureTableExistsAsync();
            // Intentar de nuevo después de crear la tabla
            var scan = _dynamoDbContext.ScanAsync<Client>(new List<ScanCondition>());
            return await scan.GetRemainingAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<Client>> GetActiveClientsAsync(CancellationToken cancellationToken = default)
    {
        var conditions = new List<ScanCondition>
        {
            new("State", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, true)
        };

        var scan = _dynamoDbContext.ScanAsync<Client>(conditions);
        return await scan.GetRemainingAsync(cancellationToken);
    }

    public async Task<Client> CreateAsync(Client client, CancellationToken cancellationToken = default)
    {
        await _dynamoDbContext.SaveAsync(client, cancellationToken);
        return client;
    }

    public async Task<Client> UpdateAsync(Client client, CancellationToken cancellationToken = default)
    {
        await _dynamoDbContext.SaveAsync(client, cancellationToken);
        return client;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _dynamoDbContext.DeleteAsync<Client>(id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        var client = await _dynamoDbContext.LoadAsync<Client>(id, cancellationToken);
        return client != null;
    }

    private async Task EnsureTableExistsAsync()
    {
        try
        {
            await _dynamoDbClient.DescribeTableAsync("client");
        }
        catch (ResourceNotFoundException)
        {
            // La tabla no existe, la creamos
            var createTableRequest = new CreateTableRequest
            {
                TableName = "client",
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "id",
                        KeyType = KeyType.HASH
                    }
                },
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "id",
                        AttributeType = ScalarAttributeType.S
                    }
                },
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5,
                    WriteCapacityUnits = 5
                }
            };

            await _dynamoDbClient.CreateTableAsync(createTableRequest);
            
            // Esperar un poco para que la tabla se cree
            await Task.Delay(5000);
        }
    }
}
