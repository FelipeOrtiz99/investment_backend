using MediatR;

namespace InvestmentBackend.Application.Investments.Commands;

public record UpdateInvestmentCommand(
    string Id,
    decimal Amount,
    int CurrencyId,
    decimal CurrentValue,
    string? InvestmentFundId = null
) : IRequest<UpdateInvestmentResult>;

public record UpdateInvestmentResult(
    string Id,
    string IdClient,
    decimal Amount,
    decimal CurrentValue,
    DateTime UpdatedAt
);
