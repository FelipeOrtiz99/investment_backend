using Amazon.DynamoDBv2.DataModel;

namespace InvestmentBackend.Domain.Entities;


[DynamoDBTable("Transactions")]
public class Transaction
{
    [DynamoDBHashKey("id")]
    public string Id { get; set; }

    [DynamoDBProperty("date")]
    public string? DateString { get; set; }

    [DynamoDBIgnore]
    public DateTimeOffset Date 
    { 
        get => string.IsNullOrEmpty(DateString) ? DateTimeOffset.UtcNow : DateTimeOffset.Parse(DateString);
        set => DateString = value.ToString("O");
    }

    [DynamoDBProperty("id_client")]
    public string IdClient { get; set; } = string.Empty;

    [DynamoDBProperty("id_currency")]
    public string CurrencyId { get; set; } = string.Empty;

    [DynamoDBProperty("amount")]
    public decimal Amount { get; set; }

    [DynamoDBProperty("status")]
    public bool Status { get; set; }


    [DynamoDBProperty("wallet_id")]
    public string? WalletId { get; set; }

    [DynamoDBProperty("description")]
    public string Description { get; set; } = string.Empty;

    [DynamoDBProperty("investmentFund_id")]
    public string? InvestmentFundId { get; set; }

    // Navigation Properties (no se almacenan en DynamoDB)
    [DynamoDBIgnore]
    public Client? Client { get; set; }

    [DynamoDBIgnore]
    public Currency? Currency { get; set; }

    [DynamoDBIgnore]
    public Wallet? Wallet { get; set; }

    [DynamoDBIgnore]
    public InvestmentFund? InvestmentFund { get; set; }

    // Constructor para DynamoDB
    public Transaction() { }

    // Constructor para lógica de negocio
    public Transaction(string clientId, string currencyId, decimal amount, string description = "", string? walletId = null, string? investmentFundId = null)
    {
        Id = Guid.NewGuid().ToString();
        Date = DateTimeOffset.UtcNow;
        IdClient = clientId ?? throw new ArgumentNullException(nameof(clientId));
        CurrencyId = currencyId ?? throw new ArgumentNullException(nameof(currencyId));
        Amount = amount > 0 ? amount : throw new ArgumentException("Amount must be positive", nameof(amount));
        Description = description;
        WalletId = walletId;
        InvestmentFundId = investmentFundId;
        Status = true; // Pendiente por defecto
    }

    // Métodos de dominio
    public void MarkAsCompleted()
    {
        Status = true;
    }

    public void Unsubscribe()
    {
        Status = false;
    }

}
