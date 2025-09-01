using MediatR;
using Microsoft.Extensions.Logging;
using InvestmentBackend.Domain.Entities;
using InvestmentBackend.Domain.Repositories;
using InvestmentBackend.Domain.Services;

namespace InvestmentBackend.Application.Transactions.Commands.UnsubscribeFromFund;

public class UnsubscribeFromFundCommandHandler : IRequestHandler<UnsubscribeFromFundCommand, Transaction>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IWalletService _walletService;
    private readonly IEmailService _emailService;
    private readonly IClientRepository _clientRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IInvestmentFundRepository _investmentFundRepository;
    private readonly ILogger<UnsubscribeFromFundCommandHandler> _logger;

    public UnsubscribeFromFundCommandHandler(
        ITransactionRepository transactionRepository,
        IWalletRepository walletRepository,
        IWalletService walletService,
        IEmailService emailService,
        IClientRepository clientRepository,
        ICurrencyRepository currencyRepository,
        IInvestmentFundRepository investmentFundRepository,
        ILogger<UnsubscribeFromFundCommandHandler> logger)
    {
        _transactionRepository = transactionRepository;
        _walletRepository = walletRepository;
        _walletService = walletService;
        _emailService = emailService;
        _clientRepository = clientRepository;
        _currencyRepository = currencyRepository;
        _investmentFundRepository = investmentFundRepository;
        _logger = logger;
    }

    public async Task<Transaction> Handle(UnsubscribeFromFundCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(request.TransactionId, cancellationToken);
        if (transaction == null)
        {
            throw new InvalidOperationException($"Transaction with ID {request.TransactionId} not found");
        }

        if (string.IsNullOrEmpty(transaction.InvestmentFundId))
        {
            throw new InvalidOperationException("This transaction is not associated with an investment fund");
        }

        if (!transaction.Status)
        {
            throw new InvalidOperationException("Transaction is already inactive");
        }

        try
        {
            transaction.Unsubscribe(); // Status = false

            if (!string.IsNullOrEmpty(transaction.WalletId))
            {
                var wallet = await _walletRepository.GetByIdAsync(transaction.WalletId);
                if (wallet != null)
                {
                    await _walletService.AddBalanceAsync(wallet.Id, transaction.Amount, cancellationToken);
                    
                    _logger.LogInformation("Balance updated for wallet {WalletId}. Added amount: {Amount}", 
                        wallet.Id, transaction.Amount);
                }
                else
                {
                    _logger.LogWarning("Wallet {WalletId} not found for transaction {TransactionId}", 
                        transaction.WalletId, transaction.Id);
                }
            }

            var updatedTransaction = await _transactionRepository.UpdateAsync(transaction, cancellationToken);

            // _ = Task.Run(async () =>
            // {
            //     try
            //     {
            //         await SendUnsubscribeNotificationAsync(updatedTransaction);
            //     }
            //     catch (Exception ex)
            //     {
            //         _logger.LogError(ex, "Failed to send unsubscribe notification for transaction {TransactionId}", 
            //             updatedTransaction.Id);
            //     }
            // }, cancellationToken);

            _logger.LogInformation("Successfully unsubscribed from fund for transaction {TransactionId}", 
                updatedTransaction.Id);

            return updatedTransaction;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unsubscribing from fund for transaction {TransactionId}", 
                request.TransactionId);
            throw;
        }
    }

    private async Task SendUnsubscribeNotificationAsync(Transaction transaction)
    {
        try
        {
            // Obtener información del cliente
            var client = await _clientRepository.GetByIdAsync(transaction.IdClient);
            if (client == null || string.IsNullOrEmpty(client.Email))
            {
                _logger.LogWarning("Cannot send email: Client {ClientId} not found or email is empty", 
                    transaction.IdClient);
                return;
            }

            // Obtener información de la moneda
            var currency = await _currencyRepository.GetByIdAsync(transaction.CurrencyId);
            var currencyCode = currency?.CurrencyCode ?? "USD";

            // Obtener información del fondo de inversión
            var investmentFund = await _investmentFundRepository.GetByIdAsync(transaction.InvestmentFundId!);
            var fundName = investmentFund?.Name ?? "Unknown Fund";

            // Preparar datos del email
            var transactionData = new TransactionEmailData
            {
                TransactionId = transaction.Id,
                TransactionType = "Fund Unsubscription",
                Amount = transaction.Amount,
                Currency = currencyCode,
                Date = transaction.Date.DateTime,
                Description = $"Unsubscribed from {fundName}"
            };

            await _emailService.SendTransactionNotificationAsync(client.Email, client.Name ?? "Cliente", transactionData);

            _logger.LogInformation("Unsubscribe notification sent successfully for transaction {TransactionId} to {Email}", 
                transaction.Id, client.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending unsubscribe notification for transaction {TransactionId}", 
                transaction.Id);
            throw;
        }
    }
}
