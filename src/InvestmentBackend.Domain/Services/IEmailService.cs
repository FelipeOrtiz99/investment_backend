namespace InvestmentBackend.Domain.Services;

public interface IEmailService
{
    Task SendTransactionNotificationAsync(string clientEmail, string clientName, TransactionEmailData transactionData);
    Task SendInvestmentConfirmationAsync(string clientEmail, string clientName, InvestmentEmailData investmentData);
}

public class TransactionEmailData
{
    public string TransactionId { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class InvestmentEmailData : TransactionEmailData
{
    public string InvestmentFundName { get; set; } = string.Empty;
    public decimal MinimumInvestment { get; set; }
}
