using MediatR;

namespace InvestmentBackend.Application.Transactions.Commands.DeleteTransaction;

public record DeleteTransactionCommand(string Id) : IRequest<bool>;
