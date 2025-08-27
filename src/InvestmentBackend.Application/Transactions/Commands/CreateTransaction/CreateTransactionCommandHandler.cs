using MediatR;
using InvestmentBackend.Domain.Entities;
using InvestmentBackend.Domain.Repositories;

namespace InvestmentBackend.Application.Transactions.Commands.CreateTransaction;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, Transaction>
{
    private readonly ITransactionRepository _transactionRepository;

    public CreateTransactionCommandHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<Transaction> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = new Transaction
        {
            Id = Guid.NewGuid().ToString(),
            Date = DateTimeOffset.UtcNow,
            IdClient = request.IdClient,
            CurrencyId = request.CurrencyId,
            Amount = request.Amount,
            Status = true // Default to active/successful
        };
        
        return await _transactionRepository.CreateAsync(transaction, cancellationToken);
    }
}
