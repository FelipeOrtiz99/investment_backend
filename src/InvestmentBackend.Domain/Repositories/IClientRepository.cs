using InvestmentBackend.Domain.Entities;

namespace InvestmentBackend.Domain.Repositories;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Client>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Client>> GetActiveClientsAsync(CancellationToken cancellationToken = default);
    Task<Client> CreateAsync(Client client, CancellationToken cancellationToken = default);
    Task<Client> UpdateAsync(Client client, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
}
