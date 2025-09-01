using MediatR;
using InvestmentBackend.Domain.Entities;
using InvestmentBackend.Domain.Repositories;
using InvestmentBackend.Domain.Services;

namespace InvestmentBackend.Application.Transactions.Commands.ProcessTransaction;

public class ProcessTransactionHandler : IRequestHandler<ProcessTransactionCommand, ProcessTransactionResult>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IWalletService _walletService;

    public ProcessTransactionHandler(
        ITransactionRepository transactionRepository,
        IWalletService walletService)
    {
        _transactionRepository = transactionRepository;
        _walletService = walletService;
    }

    public async Task<ProcessTransactionResult> Handle(ProcessTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Crear la transacción
            var transaction = new Transaction(
                request.ClientId,
                request.CurrencyId,
                request.Amount,
                request.Description);

            // Procesar la transacción con el wallet
            var processed = await _walletService.ProcessTransactionAsync(transaction);

            if (!processed)
            {
                return new ProcessTransactionResult
                {
                    Success = false,
                    Message = "Insufficient funds or processing error",
                    TransactionId = transaction.Id
                };
            }

            // Guardar la transacción
            await _transactionRepository.CreateAsync(transaction);

            // Obtener el nuevo balance
            var newBalance = await _walletService.GetWalletBalanceAsync(request.ClientId, request.CurrencyId);

            return new ProcessTransactionResult
            {
                Success = true,
                TransactionId = transaction.Id,
                Message = "Transaction processed successfully",
                NewBalance = newBalance
            };
        }
        catch (ArgumentException ex)
        {
            return new ProcessTransactionResult
            {
                Success = false,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return new ProcessTransactionResult
            {
                Success = false,
                Message = $"Error processing transaction: {ex.Message}"
            };
        }
    }
}
