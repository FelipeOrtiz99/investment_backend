using InvestmentBackend.Domain.Entities;

namespace InvestmentBackend.Application.Transactions.Services;

public interface ITransactionApplicationService
{
    Task<TransactionResult> ProcessTransactionAsync(
        string clientId, 
        string currencyId, 
        decimal amount, 
        string? description = null, 
        string? investmentFundId = null);
}

public class TransactionResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public Transaction? Transaction { get; set; }
    
    public static TransactionResult Success(Transaction transaction)
    {
        return new TransactionResult 
        { 
            IsSuccess = true, 
            Transaction = transaction 
        };
    }
    
    public static TransactionResult Failure(string errorMessage)
    {
        return new TransactionResult 
        { 
            IsSuccess = false, 
            ErrorMessage = errorMessage 
        };
    }
}
