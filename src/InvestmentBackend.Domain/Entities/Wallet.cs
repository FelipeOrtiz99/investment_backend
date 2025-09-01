using Amazon.DynamoDBv2.DataModel;

namespace InvestmentBackend.Domain.Entities;

[DynamoDBTable("Wallets")]
public class Wallet
{
    [DynamoDBHashKey("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [DynamoDBProperty("client_id")]
    public string ClientId { get; set; } = string.Empty;

    [DynamoDBProperty("currency_id")]
    public string CurrencyId { get; set; } = string.Empty;

    [DynamoDBProperty("balance")]
    public string BalanceString { get; set; } = "0";

    [DynamoDBIgnore]
    public decimal Balance 
    { 
        get => decimal.TryParse(BalanceString, out var result) ? result : 0m;
        set => BalanceString = value.ToString("F2"); // 2 decimales
    }

    [DynamoDBProperty("create_date")]
    public string? CreatedAtString { get; set; }

    [DynamoDBIgnore]
    public DateTimeOffset? CreatedAt 
    { 
        get => string.IsNullOrEmpty(CreatedAtString) ? null : DateTimeOffset.Parse(CreatedAtString);
        set => CreatedAtString = value?.ToString("O"); // ISO 8601 format
    }

    [DynamoDBProperty("update_date")]
    public string? UpdatedAtString { get; set; }

    [DynamoDBIgnore]
    public DateTimeOffset? UpdatedAt 
    { 
        get => string.IsNullOrEmpty(UpdatedAtString) ? null : DateTimeOffset.Parse(UpdatedAtString);
        set => UpdatedAtString = value?.ToString("O"); // ISO 8601 format
    }

    // Navigation Properties
    [DynamoDBIgnore]
    public Client? Client { get; set; }

    [DynamoDBIgnore]
    public Currency? Currency { get; set; }

    // Constructor para DynamoDB
    public Wallet() 
    {
        Id = Guid.NewGuid().ToString();
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    // Constructor para lógica de negocio
    public Wallet(string clientId, string currencyId, decimal initialBalance = 0)
    {
        Id = Guid.NewGuid().ToString();
        ClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
        CurrencyId = currencyId ?? throw new ArgumentNullException(nameof(currencyId));
        Balance = initialBalance;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    // Métodos de dominio para operaciones de wallet
    public bool CanDebit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));
        
        return Balance >= amount;
    }

    public void Debit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));
        
        if (!CanDebit(amount))
            throw new InvalidOperationException($"Insufficient funds. Current balance: {Balance}, Required: {amount}");
        
        Balance -= amount;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Credit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));
        
        Balance += amount;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateBalance(decimal newBalance)
    {
        if (newBalance < 0)
            throw new ArgumentException("Balance cannot be negative", nameof(newBalance));
        
        Balance = newBalance;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
