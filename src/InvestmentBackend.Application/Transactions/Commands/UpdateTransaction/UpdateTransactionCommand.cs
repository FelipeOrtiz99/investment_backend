using MediatR;
using InvestmentBackend.Domain.Entities;

namespace InvestmentBackend.Application.Transactions.Commands.UpdateTransaction;

public record UpdateTransactionCommand(
    string Id,
    string IdClient,
    string CurrencyId,
    decimal Amount,
    bool Status
) : IRequest<Transaction?>;
