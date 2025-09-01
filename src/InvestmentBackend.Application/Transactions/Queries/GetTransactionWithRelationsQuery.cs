using MediatR;

namespace InvestmentBackend.Application.Transactions.Queries;

public class GetTransactionWithRelationsQuery : IRequest<TransactionWithRelationsDto?>
{
    public string Id { get; set; } = string.Empty;

    public GetTransactionWithRelationsQuery(string id)
    {
        Id = id;
    }
}
