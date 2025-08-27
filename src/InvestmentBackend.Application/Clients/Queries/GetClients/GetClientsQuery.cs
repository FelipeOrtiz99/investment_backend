using MediatR;
using InvestmentBackend.Application.Clients.Queries.GetClient;

namespace InvestmentBackend.Application.Clients.Queries.GetClients;

public record GetClientsQuery : IRequest<IEnumerable<ClientDto>>;
