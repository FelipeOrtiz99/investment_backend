using MediatR;
using InvestmentBackend.Domain.Entities;
using InvestmentBackend.Domain.Services;
using InvestmentBackend.Domain.Repositories;
using InvestmentBackend.Application.Transactions.Services;
using Microsoft.Extensions.Logging;

namespace InvestmentBackend.Application.Transactions.Commands.CreateTransaction;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, Transaction>
{
    private readonly ITransactionApplicationService _transactionService;
    private readonly IEmailService _emailService;
    private readonly IClientRepository _clientRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IInvestmentFundRepository _investmentFundRepository;
    private readonly ILogger<CreateTransactionCommandHandler> _logger;

    public CreateTransactionCommandHandler(
        ITransactionApplicationService transactionService,
        IEmailService emailService,
        IClientRepository clientRepository,
        ICurrencyRepository currencyRepository,
        IInvestmentFundRepository investmentFundRepository,
        ILogger<CreateTransactionCommandHandler> logger)
    {
        _transactionService = transactionService;
        _emailService = emailService;
        _clientRepository = clientRepository;
        _currencyRepository = currencyRepository;
        _investmentFundRepository = investmentFundRepository;
        _logger = logger;
    }

    public async Task<Transaction> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var result = await _transactionService.ProcessTransactionAsync(
            clientId: request.IdClient,
            currencyId: request.CurrencyId,
            amount: request.Amount,
            investmentFundId: request.InvestmentFundId,
            description: request.Description
        );

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException(result.ErrorMessage ?? "Transaction failed");
        }

        var transaction = result.Transaction ?? throw new InvalidOperationException("Transaction was not created");

        _ = Task.Run(async () =>
        {
            try
            {
                await SendEmailNotificationAsync(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email notification for transaction {TransactionId}", transaction.Id);
            }
        }, cancellationToken);

        return transaction;
    }

    private async Task SendEmailNotificationAsync(Transaction transaction)
    {
        try
        {
            // Get client details
            var client = await _clientRepository.GetByIdAsync(transaction.IdClient);
            if (client == null || string.IsNullOrEmpty(client.Email))
            {
                _logger.LogWarning("Cannot send email: Client {ClientId} not found or email is empty", transaction.IdClient);
                return;
            }

            // Get currency details
            var currency = await _currencyRepository.GetByIdAsync(transaction.CurrencyId);
            var currencyCode = currency?.CurrencyCode ?? "USD";

            // Prepare email data based on whether it's an investment transaction
            if (!string.IsNullOrEmpty(transaction.InvestmentFundId))
            {
                // Investment email
                var investmentFund = await _investmentFundRepository.GetByIdAsync(transaction.InvestmentFundId);
                
                var investmentData = new InvestmentEmailData
                {
                    TransactionId = transaction.Id,
                    InvestmentFundName = investmentFund?.Name ?? "Unknown Fund",
                    Amount = transaction.Amount,
                    Currency = currencyCode,
                    Date = transaction.Date.DateTime,
                    MinimumInvestment = investmentFund?.MinInvestment ?? 0
                };

                await _emailService.SendInvestmentConfirmationAsync(client.Email, client.Name ?? "Cliente", investmentData);
            }
            else
            {
                // Regular transaction email
                var transactionData = new TransactionEmailData
                {
                    TransactionId = transaction.Id,
                    TransactionType = string.IsNullOrEmpty(transaction.InvestmentFundId) ? "Payment" : "Investment",
                    Amount = transaction.Amount,
                    Currency = currencyCode,
                    Date = transaction.Date.DateTime,
                    Description = transaction.Description ?? ""
                };

                await _emailService.SendTransactionNotificationAsync(client.Email, client.Name ?? "Cliente", transactionData);
            }

            _logger.LogInformation("Email notification sent successfully for transaction {TransactionId} to {Email}", 
                transaction.Id, client.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email notification for transaction {TransactionId}", transaction.Id);
            throw;
        }
    }
}
