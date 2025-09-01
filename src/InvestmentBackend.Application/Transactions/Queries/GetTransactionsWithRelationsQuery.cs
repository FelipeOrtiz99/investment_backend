using MediatR;

namespace InvestmentBackend.Application.Transactions.Queries;

public class GetTransactionsWithRelationsQuery : IRequest<List<TransactionWithRelationsDto>>
{
}
