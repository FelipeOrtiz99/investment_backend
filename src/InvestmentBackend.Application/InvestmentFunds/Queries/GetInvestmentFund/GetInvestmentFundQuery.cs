using MediatR;

namespace InvestmentBackend.Application.InvestmentFunds.Queries.GetInvestmentFund;

public record GetInvestmentFundQuery(string Id) : IRequest<InvestmentFundDto?>;

public record InvestmentFundDto(
    string Id,
    string Name,
    string Category,
    int CurrencyId,
    decimal MinInvestment,
    bool IsActive
);
