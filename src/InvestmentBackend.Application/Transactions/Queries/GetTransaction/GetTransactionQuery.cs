using MediatR;

namespace InvestmentBackend.Application.Transactions.Queries.GetTransaction;

public record GetTransactionQuery(string Id) : IRequest<TransactionDto?>;

public record TransactionDto(
    string Id,
    DateTimeOffset Date,
    string IdClient,
    string CurrencyId,
    decimal Amount,
    bool Status,
    string? WalletId,
    string Description,
    string? InvestmentFundId
);

public record TransactionWithNamesDto(
    string Id,
    DateTimeOffset Date,
    string IdClient,
    string ClientName,
    string CurrencyId,
    string CurrencyName,
    string CurrencyCode,
    decimal Amount,
    bool Status,
    string? WalletId,
    string Description,
    string? InvestmentFundId,
    string? InvestmentFundName
);
