namespace InvestmentBackend.Application.Transactions.Queries;

public class TransactionWithRelationsDto
{
    public string Id { get; set; } = string.Empty;
    public DateTimeOffset Date { get; set; }
    public string IdClient { get; set; } = string.Empty;
    public string CurrencyId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool Status { get; set; }

    // Propiedades relacionadas
    public ClientDto? Client { get; set; }
    public CurrencyDto? Currency { get; set; }
}

public class ClientDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool State { get; set; }
}

public class CurrencyDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
}
