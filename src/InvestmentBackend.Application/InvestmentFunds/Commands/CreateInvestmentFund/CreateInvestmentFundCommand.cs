using MediatR;
using InvestmentBackend.Domain.Entities;

namespace InvestmentBackend.Application.InvestmentFunds.Commands.CreateInvestmentFund;

public record CreateInvestmentFundCommand(
    string Name,
    string Category,
    string CurrencyId,
    decimal MinInvestment
) : IRequest<InvestmentFund>;
