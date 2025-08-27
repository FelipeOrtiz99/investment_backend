using MediatR;
using InvestmentBackend.Domain.Entities;

namespace InvestmentBackend.Application.Clients.Commands.UpdateClient;

public record UpdateClientCommand(string Id, string Name) : IRequest<Client?>;
