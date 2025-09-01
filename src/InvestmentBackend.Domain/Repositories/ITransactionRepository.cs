using InvestmentBackend.Domain.Entities;

namespace InvestmentBackend.Domain.Repositories;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaction>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaction>> GetByClientIdAsync(string clientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaction>> GetByStatusAsync(bool status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<Transaction> CreateAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task<Transaction> UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
}
