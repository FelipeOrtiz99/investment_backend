using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using InvestmentBackend.Domain.Entities;
using InvestmentBackend.Domain.Repositories;

namespace InvestmentBackend.Infrastructure.Repositories;

public class DynamoDbCurrencyRepository : ICurrencyRepository
{
    private readonly IDynamoDBContext _dynamoDbContext;
    private readonly IAmazonDynamoDB _dynamoDbClient;

    public DynamoDbCurrencyRepository(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB dynamoDbClient)
    {
        _dynamoDbContext = dynamoDbContext;
        _dynamoDbClient = dynamoDbClient;
    }

    public async Task<Currency?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dynamoDbContext.LoadAsync<Currency>(id, cancellationToken);
        }
        catch (ResourceNotFoundException)
        {
            await EnsureTableExistsAsync();
            return await _dynamoDbContext.LoadAsync<Currency>(id, cancellationToken);
        }
    }

    public async Task<IEnumerable<Currency>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var scan = _dynamoDbContext.ScanAsync<Currency>(new List<ScanCondition>());
            return await scan.GetRemainingAsync(cancellationToken);
        }
        catch (ResourceNotFoundException)
        {
            await EnsureTableExistsAsync();
            var scan = _dynamoDbContext.ScanAsync<Currency>(new List<ScanCondition>());
            return await scan.GetRemainingAsync(cancellationToken);
        }
    }

    public async Task<Currency?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var conditions = new List<ScanCondition>
            {
                new("CurrencyCode", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, code)
            };
            
            var scan = _dynamoDbContext.ScanAsync<Currency>(conditions);
            var currencies = await scan.GetRemainingAsync(cancellationToken);
            return currencies.FirstOrDefault();
        }
        catch (ResourceNotFoundException)
        {
            await EnsureTableExistsAsync();
            var conditions = new List<ScanCondition>
            {
                new("CurrencyCode", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, code)
            };
            
            var scan = _dynamoDbContext.ScanAsync<Currency>(conditions);
            var currencies = await scan.GetRemainingAsync(cancellationToken);
            return currencies.FirstOrDefault();
        }
    }

    public async Task<Currency> CreateAsync(Currency currency, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dynamoDbContext.SaveAsync(currency, cancellationToken);
            return currency;
        }
        catch (ResourceNotFoundException)
        {
            await EnsureTableExistsAsync();
            await _dynamoDbContext.SaveAsync(currency, cancellationToken);
            return currency;
        }
    }

    public async Task<Currency> UpdateAsync(Currency currency, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dynamoDbContext.SaveAsync(currency, cancellationToken);
            return currency;
        }
        catch (ResourceNotFoundException)
        {
            await EnsureTableExistsAsync();
            await _dynamoDbContext.SaveAsync(currency, cancellationToken);
            return currency;
        }
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dynamoDbContext.DeleteAsync<Currency>(id, cancellationToken);
        }
        catch (ResourceNotFoundException)
        {
            await EnsureTableExistsAsync();
            await _dynamoDbContext.DeleteAsync<Currency>(id, cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var currency = await _dynamoDbContext.LoadAsync<Currency>(id, cancellationToken);
            return currency != null;
        }
        catch (ResourceNotFoundException)
        {
            await EnsureTableExistsAsync();
            var currency = await _dynamoDbContext.LoadAsync<Currency>(id, cancellationToken);
            return currency != null;
        }
    }

    private async Task EnsureTableExistsAsync()
    {
        try
        {
            await _dynamoDbClient.DescribeTableAsync("Currencies");
        }
        catch (ResourceNotFoundException)
        {
            var createTableRequest = new CreateTableRequest
            {
                TableName = "Currencies",
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
                BillingMode = BillingMode.PAY_PER_REQUEST
            };

            await _dynamoDbClient.CreateTableAsync(createTableRequest);

            // Esperar a que la tabla esté activa
            var tableStatus = TableStatus.CREATING;
            while (tableStatus == TableStatus.CREATING)
            {
                await Task.Delay(1000);
                var tableDescription = await _dynamoDbClient.DescribeTableAsync("Currencies");
                tableStatus = tableDescription.Table.TableStatus;
            }
        }
    }
}
