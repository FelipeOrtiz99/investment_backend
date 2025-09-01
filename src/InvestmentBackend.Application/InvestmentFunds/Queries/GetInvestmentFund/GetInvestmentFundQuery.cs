using MediatR;

namespace InvestmentBackend.Application.InvestmentFunds.Queries.GetInvestmentFund;

public record GetInvestmentFundQuery(string Id) : IRequest<InvestmentFundDto?>;

public record InvestmentFundDto(
    string Id,
    string Name,
    string Category,
    string CurrencyId,
    decimal MinInvestment,
    bool IsActive
);
