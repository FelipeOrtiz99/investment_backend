using MediatR;
using InvestmentBackend.Domain.Repositories;

namespace InvestmentBackend.Application.Transactions.Queries;

public class GetTransactionWithRelationsHandler : IRequestHandler<GetTransactionWithRelationsQuery, TransactionWithRelationsDto?>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ICurrencyRepository _currencyRepository;

    public GetTransactionWithRelationsHandler(
        ITransactionRepository transactionRepository,
        IClientRepository clientRepository,
        ICurrencyRepository currencyRepository)
    {
        _transactionRepository = transactionRepository;
        _clientRepository = clientRepository;
        _currencyRepository = currencyRepository;
    }

    public async Task<TransactionWithRelationsDto?> Handle(GetTransactionWithRelationsQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(request.Id);
        if (transaction == null)
            return null;

        var dto = new TransactionWithRelationsDto
        {
            Id = transaction.Id,
            Date = transaction.Date,
            IdClient = transaction.IdClient,
            CurrencyId = transaction.CurrencyId,
            Amount = transaction.Amount,
            Status = transaction.Status
        };

        // Cargar relaciones de forma paralela para mejor performance
        var clientTask = _clientRepository.GetByIdAsync(transaction.IdClient);
        var currencyTask = _currencyRepository.GetByIdAsync(transaction.CurrencyId);

        await Task.WhenAll(clientTask, currencyTask);

        var client = await clientTask;
        var currency = await currencyTask;

        if (client != null)
        {
            dto.Client = new ClientDto
            {
                Id = client.Id ?? string.Empty,
                Name = client.Name ?? string.Empty,
                State = client.State
            };
        }

        if (currency != null)
        {
            dto.Currency = new CurrencyDto
            {
                Id = currency.Id ?? string.Empty,
                Name = currency.Name ?? string.Empty,
                CurrencyCode = currency.CurrencyCode ?? string.Empty
            };
        }

        return dto;
    }
}
