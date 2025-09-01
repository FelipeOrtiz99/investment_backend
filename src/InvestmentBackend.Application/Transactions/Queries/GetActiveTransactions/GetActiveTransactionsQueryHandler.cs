using MediatR;
using InvestmentBackend.Domain.Repositories;
using InvestmentBackend.Application.Transactions.Queries.GetTransaction;

namespace InvestmentBackend.Application.Transactions.Queries.GetActiveTransactions;

public class GetActiveTransactionsQueryHandler : IRequestHandler<GetActiveTransactionsQuery, List<TransactionWithNamesDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IInvestmentFundRepository _investmentFundRepository;

    public GetActiveTransactionsQueryHandler(
        ITransactionRepository transactionRepository,
        IClientRepository clientRepository,
        ICurrencyRepository currencyRepository,
        IInvestmentFundRepository investmentFundRepository)
    {
        _transactionRepository = transactionRepository;
        _clientRepository = clientRepository;
        _currencyRepository = currencyRepository;
        _investmentFundRepository = investmentFundRepository;
    }

    public async Task<List<TransactionWithNamesDto>> Handle(GetActiveTransactionsQuery request, CancellationToken cancellationToken)
    {
        // Obtener transacciones activas (Status = true)
        var activeTransactions = await _transactionRepository.GetByStatusAsync(true, cancellationToken);
        var result = new List<TransactionWithNamesDto>();

        foreach (var transaction in activeTransactions)
        {
            // Obtener información del cliente
            var client = await _clientRepository.GetByIdAsync(transaction.IdClient);
            var clientName = client?.Name ?? "Cliente no encontrado";

            // Obtener información de la moneda
            var currency = await _currencyRepository.GetByIdAsync(transaction.CurrencyId);
            var currencyName = currency?.Name ?? "Moneda no encontrada";
            var currencyCode = currency?.CurrencyCode ?? "N/A";

            // Obtener información del fondo de inversión si existe
            var investmentFundName = "N/A";
            if (!string.IsNullOrEmpty(transaction.InvestmentFundId))
            {
                var investmentFund = await _investmentFundRepository.GetByIdAsync(transaction.InvestmentFundId);
                investmentFundName = investmentFund?.Name ?? "Fondo no encontrado";
            }

            var transactionWithNames = new TransactionWithNamesDto(
                Id: transaction.Id,
                Date: transaction.Date,
                IdClient: transaction.IdClient,
                ClientName: clientName,
                CurrencyId: transaction.CurrencyId,
                CurrencyName: currencyName,
                CurrencyCode: currencyCode,
                Amount: transaction.Amount,
                Status: transaction.Status,
                WalletId: transaction.WalletId,
                Description: transaction.Description,
                InvestmentFundId: transaction.InvestmentFundId,
                InvestmentFundName: investmentFundName
            );

            result.Add(transactionWithNames);
        }

        return result;
    }
}
