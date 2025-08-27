using MediatR;
using InvestmentBackend.Domain.Repositories;

namespace InvestmentBackend.Application.Clients.Queries.GetClient;

public class GetClientQueryHandler : IRequestHandler<GetClientQuery, ClientDto?>
{
    private readonly IClientRepository _clientRepository;

    public GetClientQueryHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<ClientDto?> Handle(GetClientQuery request, CancellationToken cancellationToken)
    {
        var client = await _clientRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (client == null)
            return null;

        return new ClientDto(
            client.Id,
            client.Name,
            client.State
        );
    }
}
