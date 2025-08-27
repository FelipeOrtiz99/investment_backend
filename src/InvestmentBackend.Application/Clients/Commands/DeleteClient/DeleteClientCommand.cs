using MediatR;

namespace InvestmentBackend.Application.Clients.Commands.DeleteClient;

public record DeleteClientCommand(string Id) : IRequest<bool>;
