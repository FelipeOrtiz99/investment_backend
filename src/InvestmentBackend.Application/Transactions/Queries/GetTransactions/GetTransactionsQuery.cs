using MediatR;
using InvestmentBackend.Application.Transactions.Queries.GetTransaction;

namespace InvestmentBackend.Application.Transactions.Queries.GetTransactions;

public record GetTransactionsQuery : IRequest<IEnumerable<TransactionDto>>;
