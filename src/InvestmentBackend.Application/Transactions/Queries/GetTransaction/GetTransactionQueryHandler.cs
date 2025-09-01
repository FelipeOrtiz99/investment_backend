using MediatR;
using InvestmentBackend.Domain.Repositories;

namespace InvestmentBackend.Application.Transactions.Queries.GetTransaction;

public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, TransactionDto?>
{
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<TransactionDto?> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (transaction == null)
            return null;

        return new TransactionDto(
            transaction.Id,
            transaction.Date,
            transaction.IdClient,
            transaction.CurrencyId,
            transaction.Amount,
            transaction.Status,
            transaction.WalletId,
            transaction.Description,
            transaction.InvestmentFundId
        );
    }
}
