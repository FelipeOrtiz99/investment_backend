using InvestmentBackend.Domain.Entities;

namespace InvestmentBackend.Domain.Repositories;

public interface IWalletRepository
{
    Task<Wallet?> GetByIdAsync(string id);
    Task<List<Wallet>> GetAllAsync();
    Task<List<Wallet>> GetByClientIdAsync(string clientId);
    Task<Wallet?> GetByClientAndCurrencyAsync(string clientId, string currencyId);
    Task<string> CreateAsync(Wallet wallet);
    Task UpdateAsync(Wallet wallet);
    Task DeleteAsync(string id);
}
