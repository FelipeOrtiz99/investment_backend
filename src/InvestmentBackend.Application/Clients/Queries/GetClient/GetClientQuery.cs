using MediatR;

namespace InvestmentBackend.Application.Clients.Queries.GetClient;

public record GetClientQuery(string Id) : IRequest<ClientDto?>;

public record ClientDto(
    string Id,
    string Name,
    bool State
);
