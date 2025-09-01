using InvestmentBackend.Domain.Repositories;

namespace InvestmentBackend.Domain.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IWalletRepository Wallets { get; }
    IClientRepository Clients { get; }
    ICurrencyRepository Currencies { get; }
    ITransactionRepository Transactions { get; }
    IInvestmentFundRepository InvestmentFunds { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
