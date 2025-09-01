using MediatR;
using InvestmentBackend.Domain.Entities;

namespace InvestmentBackend.Application.Transactions.Commands.ProcessTransaction;

public class ProcessTransactionCommand : IRequest<ProcessTransactionResult>
{
    public string ClientId { get; set; } = string.Empty;
    public string CurrencyId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class ProcessTransactionResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public decimal NewBalance { get; set; }
}
