using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using InvestmentBackend.Domain.Entities;
using InvestmentBackend.Domain.Repositories;

namespace InvestmentBackend.Infrastructure.Repositories;

public class DynamoDbInvestmentFundRepository : IInvestmentFundRepository
{
    private readonly IDynamoDBContext _dynamoDbContext;
    private readonly IAmazonDynamoDB _dynamoDbClient;

    public DynamoDbInvestmentFundRepository(IDynamoDBContext dynamoDbContext, IAmazonDynamoDB dynamoDbClient)
    {
        _dynamoDbContext = dynamoDbContext;
        _dynamoDbClient = dynamoDbClient;
    }

    public async Task<InvestmentFund?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _dynamoDbContext.LoadAsync<InvestmentFund>(id, cancellationToken);
    }

    public async Task<IEnumerable<InvestmentFund>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var scan = _dynamoDbContext.ScanAsync<InvestmentFund>(new List<ScanCondition>());
            return await scan.GetRemainingAsync(cancellationToken);
        }
        catch (ResourceNotFoundException)
        {
            await EnsureTableExistsAsync();
            var scan = _dynamoDbContext.ScanAsync<InvestmentFund>(new List<ScanCondition>());
            return await scan.GetRemainingAsync(cancellationToken);
        }
    }

    public async Task<IEnumerable<InvestmentFund>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var conditions = new List<ScanCondition>
        {
            new("IsActive", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, true)
        };
        
        var scan = _dynamoDbContext.ScanAsync<InvestmentFund>(conditions);
        return await scan.GetRemainingAsync(cancellationToken);
    }

    public async Task<IEnumerable<InvestmentFund>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        var conditions = new List<ScanCondition>
        {
            new("Category", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, category)
        };
        
        var scan = _dynamoDbContext.ScanAsync<InvestmentFund>(conditions);
        return await scan.GetRemainingAsync(cancellationToken);
    }

    public async Task<IEnumerable<InvestmentFund>> GetByCurrencyAsync(int currencyId, CancellationToken cancellationToken = default)
    {
        var conditions = new List<ScanCondition>
        {
            new("CurrencyId", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, currencyId)
        };
        
        var scan = _dynamoDbContext.ScanAsync<InvestmentFund>(conditions);
        return await scan.GetRemainingAsync(cancellationToken);
    }

    public async Task<InvestmentFund> CreateAsync(InvestmentFund fund, CancellationToken cancellationToken = default)
    {
        await _dynamoDbContext.SaveAsync(fund, cancellationToken);
        return fund;
    }

    public async Task<InvestmentFund> UpdateAsync(InvestmentFund fund, CancellationToken cancellationToken = default)
    {
        await _dynamoDbContext.SaveAsync(fund, cancellationToken);
        return fund;
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _dynamoDbContext.DeleteAsync<InvestmentFund>(id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        var fund = await _dynamoDbContext.LoadAsync<InvestmentFund>(id, cancellationToken);
        return fund != null;
    }

    private async Task EnsureTableExistsAsync()
    {
        try
        {
            await _dynamoDbClient.DescribeTableAsync("Investments");
        }
        catch (ResourceNotFoundException)
        {
            var createTableRequest = new CreateTableRequest
            {
                TableName = "Investments",
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
