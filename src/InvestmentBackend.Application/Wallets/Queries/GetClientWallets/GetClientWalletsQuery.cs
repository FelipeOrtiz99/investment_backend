using MediatR;

namespace InvestmentBackend.Application.Wallets.Queries.GetClientWallets;

public class GetClientWalletsQuery : IRequest<List<WalletDto>>
{
    public string ClientId { get; set; } = string.Empty;

    public GetClientWalletsQuery(string clientId)
    {
        ClientId = clientId;
    }
}

public class WalletDto
{
    public string Id { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string CurrencyId { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    
    // Información de entidades relacionadas
    public string? ClientName { get; set; }
    public string? CurrencyName { get; set; }
    public string? CurrencyCode { get; set; }
}
