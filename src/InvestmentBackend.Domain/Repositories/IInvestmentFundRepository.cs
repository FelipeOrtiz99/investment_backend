using InvestmentBackend.Domain.Entities;

namespace InvestmentBackend.Domain.Repositories;

public interface IInvestmentFundRepository
{
    Task<InvestmentFund?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<InvestmentFund>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<InvestmentFund>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<InvestmentFund>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<IEnumerable<InvestmentFund>> GetByCurrencyAsync(int currencyId, CancellationToken cancellationToken = default);
    Task<InvestmentFund> CreateAsync(InvestmentFund fund, CancellationToken cancellationToken = default);
    Task<InvestmentFund> UpdateAsync(InvestmentFund fund, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
}
