using MediatR;
using InvestmentBackend.Domain.Entities;

namespace InvestmentBackend.Application.Clients.Commands.CreateClient;

public record CreateClientCommand(string Id, string Name, bool State) : IRequest<Client>;
