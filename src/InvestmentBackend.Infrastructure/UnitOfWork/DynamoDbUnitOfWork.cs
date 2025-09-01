using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using InvestmentBackend.Domain.Repositories;
using InvestmentBackend.Domain.UnitOfWork;
using InvestmentBackend.Infrastructure.Repositories;

namespace InvestmentBackend.Infrastructure.UnitOfWork;

public class DynamoDbUnitOfWork : IUnitOfWork
{
    private readonly IAmazonDynamoDB _dynamoDbClient;
    private readonly IDynamoDBContext _dynamoDbContext;
    
    // Repositories
    private IWalletRepository? _wallets;
    private IClientRepository? _clients;
    private ICurrencyRepository? _currencies;
    private ITransactionRepository? _transactions;
    private IInvestmentFundRepository? _investmentFunds;
    
    // Transaction state
    private bool _disposed = false;
    private bool _transactionStarted = false;

    public DynamoDbUnitOfWork(IAmazonDynamoDB dynamoDbClient, IDynamoDBContext dynamoDbContext)
    {
        _dynamoDbClient = dynamoDbClient;
        _dynamoDbContext = dynamoDbContext;
    }

    public IWalletRepository Wallets =>
        _wallets ??= new DynamoDbWalletRepository(_dynamoDbContext, _dynamoDbClient);

    public IClientRepository Clients =>
        _clients ??= new DynamoDbClientRepository(_dynamoDbContext, _dynamoDbClient);

    public ICurrencyRepository Currencies =>
        _currencies ??= new DynamoDbCurrencyRepository(_dynamoDbContext, _dynamoDbClient);

    public ITransactionRepository Transactions =>
        _transactions ??= new DynamoDbTransactionRepository(_dynamoDbContext, _dynamoDbClient);

    public IInvestmentFundRepository InvestmentFunds =>
        _investmentFunds ??= new DynamoDbInvestmentFundRepository(_dynamoDbContext, _dynamoDbClient);

    public async Task<int> SaveChangesAsync()
    {
        // En DynamoDB, las operaciones son inmediatas, pero podríamos implementar
        // batch operations aquí si fuera necesario
        await Task.CompletedTask;
        return 1; // Simular que se guardó una unidad
    }

    public async Task BeginTransactionAsync()
    {
        // DynamoDB no tiene transacciones tradicionales como SQL,
        // pero podemos usar TransactWrite para operaciones atómicas
        _transactionStarted = true;
        await Task.CompletedTask;
    }

    public async Task CommitTransactionAsync()
    {
        if (!_transactionStarted)
            throw new InvalidOperationException("No transaction was started");

        // En una implementación real, aquí ejecutaríamos todas las operaciones
        // acumuladas usando DynamoDB TransactWrite
        _transactionStarted = false;
        await Task.CompletedTask;
    }

    public async Task RollbackTransactionAsync()
    {
        if (!_transactionStarted)
            return;

        // En DynamoDB, el rollback sería manejado automáticamente
        // si alguna operación en TransactWrite falla
        _transactionStarted = false;
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            // Cleanup resources if needed
            _disposed = true;
        }
    }
}
