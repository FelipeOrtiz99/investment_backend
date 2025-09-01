using MediatR;
using InvestmentBackend.Domain.Repositories;

namespace InvestmentBackend.Application.Transactions.Queries;

public class GetTransactionsWithRelationsHandler : IRequestHandler<GetTransactionsWithRelationsQuery, List<TransactionWithRelationsDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ICurrencyRepository _currencyRepository;

    public GetTransactionsWithRelationsHandler(
        ITransactionRepository transactionRepository,
        IClientRepository clientRepository,
        ICurrencyRepository currencyRepository)
    {
        _transactionRepository = transactionRepository;
        _clientRepository = clientRepository;
        _currencyRepository = currencyRepository;
    }

    public async Task<List<TransactionWithRelationsDto>> Handle(GetTransactionsWithRelationsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetAllAsync();
        var result = new List<TransactionWithRelationsDto>();

        // Obtener todos los IDs únicos para optimizar las consultas
        var clientIds = transactions.Select(t => t.IdClient).Distinct().ToList();
        var currencyIds = transactions.Select(t => t.CurrencyId).Distinct().ToList();

        // Cargar todos los clientes y monedas de una vez
        var clientTasks = clientIds.Select(id => _clientRepository.GetByIdAsync(id));
        var currencyTasks = currencyIds.Select(id => _currencyRepository.GetByIdAsync(id));

        var clients = await Task.WhenAll(clientTasks);
        var currencies = await Task.WhenAll(currencyTasks);

        // Crear diccionarios para búsqueda rápida
        var clientDict = clients.Where(c => c != null).ToDictionary(c => c!.Id!, c => c);
        var currencyDict = currencies.Where(c => c != null).ToDictionary(c => c!.Id!, c => c);

        foreach (var transaction in transactions)
        {
            var dto = new TransactionWithRelationsDto
            {
                Id = transaction.Id,
                Date = transaction.Date,
                IdClient = transaction.IdClient,
                CurrencyId = transaction.CurrencyId,
                Amount = transaction.Amount,
                Status = transaction.Status
            };

            // Agregar relaciones si existen
            if (clientDict.TryGetValue(transaction.IdClient, out var client))
            {
                dto.Client = new ClientDto
                {
                    Id = client.Id ?? string.Empty,
                    Name = client.Name ?? string.Empty,
                    State = client.State
                };
            }

            if (currencyDict.TryGetValue(transaction.CurrencyId, out var currency))
            {
                dto.Currency = new CurrencyDto
                {
                    Id = currency.Id ?? string.Empty,
                    Name = currency.Name ?? string.Empty,
                    CurrencyCode = currency.CurrencyCode ?? string.Empty
                };
            }

            result.Add(dto);
        }

        return result;
    }
}
