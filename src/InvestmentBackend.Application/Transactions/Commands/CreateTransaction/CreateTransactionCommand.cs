using MediatR;
using InvestmentBackend.Domain.Entities;

namespace InvestmentBackend.Application.Transactions.Commands.CreateTransaction;

public record CreateTransactionCommand(
    string IdClient,
    string CurrencyId,
    decimal Amount
) : IRequest<Transaction>;
