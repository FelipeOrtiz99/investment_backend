using Amazon.DynamoDBv2.DataModel;

namespace InvestmentBackend.Domain.Entities;

[DynamoDBTable("InvestmentsFund")]
public class InvestmentFund
{
    [DynamoDBHashKey("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [DynamoDBProperty("name")]
    public string Name { get; set; } = string.Empty;

    [DynamoDBProperty("category")]
    public string Category { get; set; } = string.Empty;

    [DynamoDBProperty("currency_id")]
    public string CurrencyId { get; set; } = string.Empty;

    [DynamoDBProperty("min_investment")]
    public decimal MinInvestment { get; set; }

    [DynamoDBProperty("state")]
    public bool IsActive { get; set; } = true;

}
