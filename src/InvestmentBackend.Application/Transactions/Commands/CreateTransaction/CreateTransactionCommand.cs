using MediatR;
using InvestmentBackend.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace InvestmentBackend.Application.Transactions.Commands.CreateTransaction;

public record CreateTransactionCommand(
    [Required] string IdClient,
    [Required] string CurrencyId,
    [Required] decimal Amount,
    string? InvestmentFundId = null,
    string? Description = null
) : IRequest<Transaction>;
