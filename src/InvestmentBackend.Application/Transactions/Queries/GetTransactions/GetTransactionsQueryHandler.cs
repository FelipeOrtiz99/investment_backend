using MediatR;
using InvestmentBackend.Domain.Repositories;
using InvestmentBackend.Application.Transactions.Queries.GetTransaction;

namespace InvestmentBackend.Application.Transactions.Queries.GetTransactions;

public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, IEnumerable<TransactionWithNamesDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IInvestmentFundRepository _investmentFundRepository;

    public GetTransactionsQueryHandler(
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

    public async Task<IEnumerable<TransactionWithNamesDto>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetAllAsync(cancellationToken);
        var result = new List<TransactionWithNamesDto>();

        foreach (var transaction in transactions)
        {
            // Obtener información del cliente
            var client = await _clientRepository.GetByIdAsync(transaction.IdClient, cancellationToken);
            
            // Obtener información de la moneda
            var currency = await _currencyRepository.GetByIdAsync(transaction.CurrencyId, cancellationToken);
            
            // Obtener información del fondo de inversión (si existe)
            var investmentFund = !string.IsNullOrEmpty(transaction.InvestmentFundId) 
                ? await _investmentFundRepository.GetByIdAsync(transaction.InvestmentFundId, cancellationToken)
                : null;

            var transactionDto = new TransactionWithNamesDto(
                Id: transaction.Id,
                Date: transaction.Date,
                IdClient: transaction.IdClient,
                ClientName: client?.Name ?? "Unknown Client",
                CurrencyId: transaction.CurrencyId,
                CurrencyName: currency?.Name ?? "Unknown Currency",
                CurrencyCode: currency?.CurrencyCode ?? "UNK",
                Amount: transaction.Amount,
                Status: transaction.Status,
                WalletId: transaction.WalletId,
                Description: transaction.Description,
                InvestmentFundId: transaction.InvestmentFundId,
                InvestmentFundName: investmentFund?.Name
            );

            result.Add(transactionDto);
        }

        return result;
    }
}
