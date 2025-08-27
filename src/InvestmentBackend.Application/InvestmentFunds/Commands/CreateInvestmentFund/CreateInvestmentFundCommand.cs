using MediatR;
using InvestmentBackend.Domain.Entities;

namespace InvestmentBackend.Application.InvestmentFunds.Commands.CreateInvestmentFund;

public record CreateInvestmentFundCommand(
    string Name,
    string Category,
    int CurrencyId,
    decimal MinInvestment
) : IRequest<InvestmentFund>;
