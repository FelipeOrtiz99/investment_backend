using Amazon.DynamoDBv2.DataModel;

namespace InvestmentBackend.Domain.Entities;

[DynamoDBTable("Currencies")]
public class Currency
{
    [DynamoDBHashKey("id")]
    public string Id { get; set; } = string.Empty;

    [DynamoDBProperty("name")]
    public string Name { get; set; } = string.Empty;

    [DynamoDBProperty("code")]
    public string CurrencyCode { get; set; } = string.Empty;

    // Constructor sin parámetros para DynamoDB
    public Currency() 
    {
        Id = Guid.NewGuid().ToString();
    }

    // Constructor con parámetros para lógica de negocio
    public Currency(string name, string currencyCode)
    {
        Id = Guid.NewGuid().ToString();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        CurrencyCode = currencyCode ?? throw new ArgumentNullException(nameof(currencyCode));
    }

    // Métodos de dominio
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        
        Name = name;
    }

    public void UpdateCode(string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new ArgumentException("Currency code cannot be empty", nameof(currencyCode));
        
        CurrencyCode = currencyCode;
    }
}   