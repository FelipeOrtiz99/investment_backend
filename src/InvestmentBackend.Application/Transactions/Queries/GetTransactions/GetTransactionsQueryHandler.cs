using MediatR;
using InvestmentBackend.Domain.Repositories;
using InvestmentBackend.Application.Transactions.Queries.GetTransaction;

namespace InvestmentBackend.Application.Transactions.Queries.GetTransactions;

public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, IEnumerable<TransactionDto>>
{
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionsQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<IEnumerable<TransactionDto>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetAllAsync(cancellationToken);

        return transactions.Select(transaction => new TransactionDto(
            transaction.Id,
            transaction.Date,
            transaction.IdClient,
            transaction.CurrencyId,
            transaction.Amount,
            transaction.Status
        ));
    }
}
