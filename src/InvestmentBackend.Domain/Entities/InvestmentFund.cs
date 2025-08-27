using Amazon.DynamoDBv2.DataModel;

namespace InvestmentBackend.Domain.Entities;

[DynamoDBTable("Investments")]
public class InvestmentFund
{
    [DynamoDBHashKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [DynamoDBProperty]
    public string Name { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string Category { get; set; } = string.Empty;

    [DynamoDBProperty]
    public int CurrencyId { get; set; }

    [DynamoDBProperty]
    public decimal MinInvestment { get; set; }

    [DynamoDBProperty]
    public bool IsActive { get; set; } = true;

}
