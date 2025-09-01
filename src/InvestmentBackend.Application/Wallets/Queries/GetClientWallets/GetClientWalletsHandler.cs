using MediatR;
using InvestmentBackend.Domain.Services;
using InvestmentBackend.Domain.Repositories;

namespace InvestmentBackend.Application.Wallets.Queries.GetClientWallets;

public class GetClientWalletsHandler : IRequestHandler<GetClientWalletsQuery, List<WalletDto>>
{
    private readonly IWalletService _walletService;
    private readonly IClientRepository _clientRepository;
    private readonly ICurrencyRepository _currencyRepository;

    public GetClientWalletsHandler(
        IWalletService walletService,
        IClientRepository clientRepository,
        ICurrencyRepository currencyRepository)
    {
        _walletService = walletService;
        _clientRepository = clientRepository;
        _currencyRepository = currencyRepository;
    }

    public async Task<List<WalletDto>> Handle(GetClientWalletsQuery request, CancellationToken cancellationToken)
    {
        var wallets = await _walletService.GetClientWalletsAsync(request.ClientId);
        var result = new List<WalletDto>();

        // Obtener información adicional
        var client = await _clientRepository.GetByIdAsync(request.ClientId);
        
        foreach (var wallet in wallets)
        {
            var currency = await _currencyRepository.GetByIdAsync(wallet.CurrencyId);
            
            var dto = new WalletDto
            {
                Id = wallet.Id,
                ClientId = wallet.ClientId,
                CurrencyId = wallet.CurrencyId,
                Balance = wallet.Balance,
                CreatedAt = wallet.CreatedAt.GetValueOrDefault(DateTimeOffset.UtcNow),
                UpdatedAt = wallet.UpdatedAt.GetValueOrDefault(DateTimeOffset.UtcNow),
                ClientName = client?.Name,
                CurrencyName = currency?.Name,
                CurrencyCode = currency?.CurrencyCode
            };
            
            result.Add(dto);
        }

        return result;
    }
}
