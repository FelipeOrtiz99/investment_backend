using MediatR;

namespace InvestmentBackend.Application.Transactions.Queries.GetTransaction;

public record GetTransactionQuery(string Id) : IRequest<TransactionDto?>;

public record TransactionDto(
    string Id,
    DateTimeOffset Date,
    string IdClient,
    string CurrencyId,
    decimal Amount,
    bool Status
);
