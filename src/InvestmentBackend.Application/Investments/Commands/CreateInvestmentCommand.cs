using MediatR;

namespace InvestmentBackend.Application.Investments.Commands;

public record CreateInvestmentCommand(
    string IdClient,
    decimal Amount,
    int CurrencyId,
    string? InvestmentFundId = null
) : IRequest<CreateInvestmentResult>;

public record CreateInvestmentResult(
    string Id,
    string IdClient,
    decimal Amount,
    int CurrencyId,
    DateTime InvestmentDate
);
