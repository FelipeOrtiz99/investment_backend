using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using InvestmentBackend.Domain.Entities;
using InvestmentBackend.Domain.Repositories;

namespace InvestmentBackend.Infrastructure.Repositories;

public class DynamoDbTransactionRepository : ITransactionRepository
{
    private readonly IDynamoDBContext _dynamoDbContext;
    private readonly IAmazonDynamoDB _dynamoDbClient;

    public DynamoDbTransactionRepository(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB dynamoDbClient)
    {
        _dynamoDbContext = dynamoDbContext;
        _dynamoDbClient = dynamoDbClient;
    }

    public async Task<Transaction?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _dynamoDbContext.LoadAsync<Transaction>(id, cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var scan = _dynamoDbContext.ScanAsync<Transaction>(new List<ScanCondition>());
            return await scan.GetRemainingAsync(cancellationToken);
        }
        catch (ResourceNotFoundException)
        {
            await EnsureTableExistsAsync();
            var scan = _dynamoDbContext.ScanAsync<Transaction>(new List<ScanCondition>());
            return await scan.GetRemainingAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<Transaction>> GetByClientIdAsync(string clientId, CancellationToken cancellationToken = default)
    {
        var conditions = new List<ScanCondition>
        {
            new("IdClient", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, clientId)
        };
        
        var scan = _dynamoDbContext.ScanAsync<Transaction>(conditions);
        return await scan.GetRemainingAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetByStatusAsync(bool status, CancellationToken cancellationToken = default)
    {
        
        var scan = _dynamoDbContext.ScanAsync<Transaction>(new List<ScanCondition>());
        var result =  await scan.GetRemainingAsync(cancellationToken);
        return result.Where(x => x.Status == status);
    }

    public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var conditions = new List<ScanCondition>
        {
            new("Date", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Between, startDate, endDate)
        };
        
        var scan = _dynamoDbContext.ScanAsync<Transaction>(conditions);
        return await scan.GetRemainingAsync(cancellationToken);
    }

    public async Task<Transaction> CreateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        await _dynamoDbContext.SaveAsync(transaction, cancellationToken);
        return transaction;
    }

    public async Task<Transaction> UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        await _dynamoDbContext.SaveAsync(transaction, cancellationToken);
        return transaction;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _dynamoDbContext.DeleteAsync<Transaction>(id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        var transaction = await _dynamoDbContext.LoadAsync<Transaction>(id, cancellationToken);
        return transaction != null;
    }

    private async Task EnsureTableExistsAsync()
    {
        try
        {
            await _dynamoDbClient.DescribeTableAsync("Transactions");
        }
        catch (ResourceNotFoundException)
        {
            var createTableRequest = new CreateTableRequest
            {
                TableName = "Transactions",
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = "Id",
                        KeyType = KeyType.HASH
                    }
                },
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Id",
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
            await Task.Delay(5000);
        }
    }
}
