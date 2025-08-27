using InvestmentBackend.Domain.Entities;

namespace InvestmentBackend.Domain.Repositories;

public interface ICurrencyRepository
{
    Task<Currency?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Currency>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Currency?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Currency> CreateAsync(Currency currency, CancellationToken cancellationToken = default);
    Task<Currency> UpdateAsync(Currency currency, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
}
