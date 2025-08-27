using Amazon.DynamoDBv2.DataModel;

namespace InvestmentBackend.Domain.Entities;

[DynamoDBTable("Currencies")]
public class Currency
{
    [DynamoDBHashKey("id")]
    public string Id { get; set; }

    [DynamoDBProperty("name")]
    public string Name { get; set; } = string.Empty;

    [DynamoDBProperty("code")]
    public string CurrencyCode { get; set; } = string.Empty;


}   