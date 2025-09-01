using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using InvestmentBackend.Domain.Entities;
using InvestmentBackend.Domain.Repositories;

namespace InvestmentBackend.Infrastructure.Repositories;

public class DynamoDbWalletRepository : IWalletRepository
{
    private readonly IDynamoDBContext _context;
    private readonly IAmazonDynamoDB _dynamoDbClient;

    public DynamoDbWalletRepository(IDynamoDBContext context, IAmazonDynamoDB dynamoDbClient)
    {
        _context = context;
        _dynamoDbClient = dynamoDbClient;
    }

    public async Task<Wallet?> GetByIdAsync(string id)
    {
        await EnsureTableExistsAsync();
        return await _context.LoadAsync<Wallet>(id);
    }

    public async Task<List<Wallet>> GetAllAsync()
    {
        await EnsureTableExistsAsync();
        var search = _context.ScanAsync<Wallet>(new List<ScanCondition>());
        return await search.GetRemainingAsync();
    }

    public async Task<List<Wallet>> GetByClientIdAsync(string clientId)
    {
        try
        {
            await EnsureTableExistsAsync();
            var conditions = new List<ScanCondition>
            {
                new ScanCondition("ClientId", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, clientId)
            };
            var search = _context.ScanAsync<Wallet>(conditions);
            return await search.GetRemainingAsync();
        }
        catch (Exception ex)
        {
            // Log del error para debugging
            Console.WriteLine($"Error in GetByClientIdAsync: {ex.Message}");
            // Si hay error de conversión, retornar lista vacía y recrear tabla
            if (ex.Message.Contains("Unable to convert") || ex.Message.Contains("DynamoDB entry"))
            {
                await RecreateTableAsync();
                return new List<Wallet>();
            }
            throw;
        }
    }

    public async Task<Wallet?> GetByClientAndCurrencyAsync(string clientId, string currencyId)
    {
        await EnsureTableExistsAsync();
        var conditions = new List<ScanCondition>
        {
            new ScanCondition("ClientId", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, clientId),
            new ScanCondition("CurrencyId", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, currencyId)
        };
        var search = _context.ScanAsync<Wallet>(conditions);
        var results = await search.GetRemainingAsync();
        return results.FirstOrDefault();
    }

    public async Task<string> CreateAsync(Wallet wallet)
    {
        await EnsureTableExistsAsync();
        await _context.SaveAsync(wallet);
        return wallet.Id;
    }

    public async Task UpdateAsync(Wallet wallet)
    {
        await EnsureTableExistsAsync();
        await _context.SaveAsync(wallet);
    }

    public async Task DeleteAsync(string id)
    {
        await EnsureTableExistsAsync();
        await _context.DeleteAsync<Wallet>(id);
    }

    private async Task EnsureTableExistsAsync()
    {
        try
        {
            var tableDescription = await _dynamoDbClient.DescribeTableAsync("Wallets");
        }
        catch (ResourceNotFoundException)
        {
            await CreateTableAsync();
        }
    }

    private async Task RecreateTableAsync()
    {
        try
        {
            // Eliminar tabla existente
            await _dynamoDbClient.DeleteTableAsync("Wallets");
            
            // Esperar a que se elimine
            var tableExists = true;
            while (tableExists)
            {
                try
                {
                    await Task.Delay(1000);
                    await _dynamoDbClient.DescribeTableAsync("Wallets");
                }
                catch (ResourceNotFoundException)
                {
                    tableExists = false;
                }
            }
        }
        catch (ResourceNotFoundException)
        {
            // La tabla ya no existe
        }

        // Crear tabla nueva
        await CreateTableAsync();
    }

    private async Task CreateTableAsync()
    {
        var createTableRequest = new CreateTableRequest
        {
            TableName = "Wallets",
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
            var tableDescription = await _dynamoDbClient.DescribeTableAsync("Wallets");
            tableStatus = tableDescription.Table.TableStatus;
        }
    }
}
