using Amazon.DynamoDBv2.DataModel;

namespace InvestmentBackend.Domain.Entities;

[DynamoDBTable("Transactions")]
public class Transaction
{
    [DynamoDBHashKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [DynamoDBProperty]
    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;

    [DynamoDBProperty]
    public string IdClient { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string CurrencyId { get; set; } = string.Empty;

    [DynamoDBProperty]
    public decimal Amount { get; set; }

    [DynamoDBProperty]
    public bool Status { get; set; }


}
