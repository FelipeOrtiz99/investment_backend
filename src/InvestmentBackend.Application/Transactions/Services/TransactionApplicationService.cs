using InvestmentBackend.Domain.Entities;
using InvestmentBackend.Domain.Services;

namespace InvestmentBackend.Application.Transactions.Services;

public class TransactionApplicationService : ITransactionApplicationService
{
    private readonly IWalletTransactionService _walletTransactionService;

    public TransactionApplicationService(IWalletTransactionService walletTransactionService)
    {
        _walletTransactionService = walletTransactionService;
    }

    public async Task<TransactionResult> ProcessTransactionAsync(
        string clientId, 
        string currencyId, 
        decimal amount, 
        string? description = null, 
        string? investmentFundId = null)
    {
        try
        {
            var result = await _walletTransactionService.ProcessTransactionAsync(
                clientId, 
                currencyId, 
                amount, 
                description, 
                investmentFundId);

            if (result.IsSuccess && result.Transaction != null)
            {
                return TransactionResult.Success(result.Transaction);
            }

            return TransactionResult.Failure(result.ErrorMessage ?? "Transaction failed");
        }
        catch (Exception ex)
        {
            return TransactionResult.Failure($"An error occurred: {ex.Message}");
        }
    }
}
