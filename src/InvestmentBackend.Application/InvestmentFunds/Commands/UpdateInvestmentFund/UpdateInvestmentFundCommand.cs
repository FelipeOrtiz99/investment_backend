using MediatR;
using InvestmentBackend.Domain.Entities;

namespace InvestmentBackend.Application.InvestmentFunds.Commands.UpdateInvestmentFund;

public record UpdateInvestmentFundCommand(
    string Id,
    string Name,
    string Category,
    int CurrencyId,
    decimal MinInvestment,
    bool IsActive
) : IRequest<InvestmentFund?>;
