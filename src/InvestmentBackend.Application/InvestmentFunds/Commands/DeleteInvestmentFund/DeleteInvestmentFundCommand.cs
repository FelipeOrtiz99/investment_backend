using MediatR;

namespace InvestmentBackend.Application.InvestmentFunds.Commands.DeleteInvestmentFund;

public record DeleteInvestmentFundCommand(string Id) : IRequest<bool>;
