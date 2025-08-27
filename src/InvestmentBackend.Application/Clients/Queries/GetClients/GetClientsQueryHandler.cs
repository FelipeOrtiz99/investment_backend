using MediatR;
using InvestmentBackend.Domain.Repositories;
using InvestmentBackend.Application.Clients.Queries.GetClient;

namespace InvestmentBackend.Application.Clients.Queries.GetClients;

public class GetClientsQueryHandler : IRequestHandler<GetClientsQuery, IEnumerable<ClientDto>>
{
    private readonly IClientRepository _clientRepository;

    public GetClientsQueryHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<IEnumerable<ClientDto>> Handle(GetClientsQuery request, CancellationToken cancellationToken)
    {
        var clients = await _clientRepository.GetAllAsync(cancellationToken);

        return clients.Select(client => new ClientDto(
            client.Id,
            client.Name,
            client.State
        ));
    }
}
