using InvestmentBackend.Domain.Entities;
using InvestmentBackend.Domain.Repositories;

namespace InvestmentBackend.Domain.Services;

public interface IWalletService
{
    Task<Wallet> GetOrCreateWalletAsync(string clientId, string currencyId);
    Task<bool> ProcessTransactionAsync(Transaction transaction);
    Task<decimal> GetWalletBalanceAsync(string clientId, string currencyId);
    Task<List<Wallet>> GetClientWalletsAsync(string clientId);
    Task AddBalanceAsync(string walletId, decimal amount, CancellationToken cancellationToken = default);
}

public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ICurrencyRepository _currencyRepository;

    public WalletService(
        IWalletRepository walletRepository,
        IClientRepository clientRepository,
        ICurrencyRepository currencyRepository)
    {
        _walletRepository = walletRepository;
        _clientRepository = clientRepository;
        _currencyRepository = currencyRepository;
    }

    public async Task<Wallet> GetOrCreateWalletAsync(string clientId, string currencyId)
    {
        // Verificar que el cliente existe
        var client = await _clientRepository.GetByIdAsync(clientId);
        if (client == null)
            throw new ArgumentException($"Client with ID {clientId} does not exist");

        // Verificar que la moneda existe
        var currency = await _currencyRepository.GetByIdAsync(currencyId);
        if (currency == null)
            throw new ArgumentException($"Currency with ID {currencyId} does not exist");

        // Buscar wallet existente
        var existingWallet = await _walletRepository.GetByClientAndCurrencyAsync(clientId, currencyId);
        if (existingWallet != null)
            return existingWallet;

        // Crear nuevo wallet
        var newWallet = new Wallet(clientId, currencyId, 0);
        await _walletRepository.CreateAsync(newWallet);
        return newWallet;
    }

    public async Task<bool> ProcessTransactionAsync(Transaction transaction)
    {
        try
        {
            // Obtener o crear el wallet
            var wallet = await GetOrCreateWalletAsync(transaction.IdClient, transaction.CurrencyId);
            
            // Actualizar el WalletId en la transacción
            transaction.WalletId = wallet.Id;

            // Actualizar el wallet en la base de datos
            await _walletRepository.UpdateAsync(wallet);
            
            // Marcar la transacción como completada
            transaction.MarkAsCompleted();
            
            return true;
        }
        catch (Exception)
        {
            // En caso de error, marcar como pendiente
            transaction.Unsubscribe();
            return false;
        }
    }

    public async Task<decimal> GetWalletBalanceAsync(string clientId, string currencyId)
    {
        var wallet = await _walletRepository.GetByClientAndCurrencyAsync(clientId, currencyId);
        return wallet?.Balance ?? 0m;
    }

    public async Task<List<Wallet>> GetClientWalletsAsync(string clientId)
    {
        return await _walletRepository.GetByClientIdAsync(clientId);
    }

    public async Task AddBalanceAsync(string walletId, decimal amount, CancellationToken cancellationToken = default)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));

        var wallet = await _walletRepository.GetByIdAsync(walletId);
        if (wallet == null)
            throw new InvalidOperationException($"Wallet with ID {walletId} not found");

        // Agregar el monto al balance usando el método Credit
        wallet.Credit(amount);

        // Actualizar el wallet en la base de datos
        await _walletRepository.UpdateAsync(wallet);
    }
}
