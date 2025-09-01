using InvestmentBackend.Domain.Entities;
using InvestmentBackend.Domain.UnitOfWork;

namespace InvestmentBackend.Domain.Services;

public interface IWalletTransactionService
{
    Task<WalletTransactionResult> ProcessTransactionAsync(string clientId, string currencyId, decimal amount, string? description = null, string? investmentFundId = null);
    Task<decimal> GetWalletBalanceAsync(string clientId, string currencyId);
    Task<List<Wallet>> GetClientWalletsAsync(string clientId);
    Task<Wallet> GetOrCreateWalletAsync(string clientId, string currencyId);
    Task<bool> ValidateInvestmentFundRequirementsAsync(decimal amount, string currencyId, string investmentFundId);
}

public class WalletTransactionResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public Transaction? Transaction { get; set; }
    public Wallet? UpdatedWallet { get; set; }
    
    public static WalletTransactionResult Success(Transaction transaction, Wallet wallet)
        => new() { IsSuccess = true, Transaction = transaction, UpdatedWallet = wallet };
    
    public static WalletTransactionResult Failure(string errorMessage)
        => new() { IsSuccess = false, ErrorMessage = errorMessage };
}

public class WalletTransactionService : IWalletTransactionService
{
    private readonly IUnitOfWork _unitOfWork;

    public WalletTransactionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<WalletTransactionResult> ProcessTransactionAsync(
        string clientId, 
        string currencyId, 
        decimal amount, 
        string? description = null,
        string? investmentFundId = null)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var client = await _unitOfWork.Clients.GetByIdAsync(clientId);
            if (client == null)
            {
                return WalletTransactionResult.Failure($"Client with ID {clientId} does not exist");
            }

            var currency = await _unitOfWork.Currencies.GetByIdAsync(currencyId);
            if (currency == null)
            {
                return WalletTransactionResult.Failure($"Currency with ID {currencyId} does not exist");
            }

            var wallet = await GetOrCreateWalletAsync(clientId, currencyId);

            if (amount <= 0)
            {
                return WalletTransactionResult.Failure("Transaction amount must be greater than zero");
            }

            var transaction = new Transaction(
                clientId: clientId,
                currencyId: currencyId,
                amount: amount,
                description: description ?? string.Empty,
                walletId: wallet.Id,
                investmentFundId: investmentFundId
            );

            if (!string.IsNullOrEmpty(investmentFundId))
            {
                if (!wallet.CanDebit(amount))
                {
                    return WalletTransactionResult.Failure($"Insufficient funds. Current balance: {wallet.Balance}, Required: {amount}");
                }
                
                var isValidInvestment = await ValidateInvestmentFundRequirementsAsync(amount, currencyId, investmentFundId);
                if (!isValidInvestment)
                {
                    return WalletTransactionResult.Failure("Investment fund requirements not met");
                }
                
                wallet.Debit(amount); // Restar el monto de la inversión
            }

            if (wallet.CurrencyId != transaction.CurrencyId)
            {
                return WalletTransactionResult.Failure($"Currency mismatch. Wallet currency: {wallet.CurrencyId}, Transaction currency: {transaction.CurrencyId}");
            }

            await _unitOfWork.Wallets.UpdateAsync(wallet);
            await _unitOfWork.Transactions.CreateAsync(transaction);

            transaction.MarkAsCompleted();
            await _unitOfWork.Transactions.UpdateAsync(transaction);

            await _unitOfWork.CommitTransactionAsync();
            await _unitOfWork.SaveChangesAsync();

            return WalletTransactionResult.Success(transaction, wallet);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return WalletTransactionResult.Failure($"Transaction failed: {ex.Message}");
        }
    }

    public async Task<decimal> GetWalletBalanceAsync(string clientId, string currencyId)
    {
        var wallet = await _unitOfWork.Wallets.GetByClientAndCurrencyAsync(clientId, currencyId);
        return wallet?.Balance ?? 0m;
    }

    public async Task<List<Wallet>> GetClientWalletsAsync(string clientId)
    {
        return await _unitOfWork.Wallets.GetByClientIdAsync(clientId);
    }

    public async Task<Wallet> GetOrCreateWalletAsync(string clientId, string currencyId)
    {
        // Buscar wallet existente
        var existingWallet = await _unitOfWork.Wallets.GetByClientAndCurrencyAsync(clientId, currencyId);
        if (existingWallet != null)
            return existingWallet;

        // Crear nuevo wallet
        var newWallet = new Wallet(clientId, currencyId, 0);
        await _unitOfWork.Wallets.CreateAsync(newWallet);
        return newWallet;
    }

    public async Task<bool> ValidateInvestmentFundRequirementsAsync(decimal amount, string currencyId, string investmentFundId)
    {
        try
        {
            // 1. Verificar que el fondo existe y está activo
            var fund = await _unitOfWork.InvestmentFunds.GetByIdAsync(investmentFundId);
            if (fund == null)
            {
                throw new ArgumentException($"Investment fund with ID {investmentFundId} does not exist");
            }

            if (!fund.IsActive)
            {
                throw new InvalidOperationException($"Investment fund '{fund.Name}' is not active");
            }

            // 2. Verificar que la moneda del fondo coincide con la de la transacción
            if (fund.CurrencyId != currencyId)
            {
                throw new InvalidOperationException($"Currency mismatch. Fund currency: {fund.CurrencyId}, Transaction currency: {currencyId}");
            }

            // 3. Verificar que el monto cumple con la inversión mínima del fondo
            if (amount < fund.MinInvestment)
            {
                throw new InvalidOperationException($"Investment amount ({amount}) is below the minimum required for fund '{fund.Name}' (Minimum: {fund.MinInvestment})");
            }

            return true;
        }
        catch (Exception)
        {
            // Log the exception details if needed
            return false;
        }
    }
}
