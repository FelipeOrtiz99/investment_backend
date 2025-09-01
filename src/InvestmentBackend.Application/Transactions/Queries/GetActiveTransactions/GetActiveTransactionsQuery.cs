using MediatR;
using InvestmentBackend.Application.Transactions.Queries.GetTransaction;

namespace InvestmentBackend.Application.Transactions.Queries.GetActiveTransactions;

public record GetActiveTransactionsQuery : IRequest<List<TransactionWithNamesDto>>;
