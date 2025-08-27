using MediatR;
using InvestmentBackend.Application.InvestmentFunds.Queries.GetInvestmentFund;

namespace InvestmentBackend.Application.InvestmentFunds.Queries.GetInvestmentFunds;

public record GetInvestmentFundsQuery : IRequest<IEnumerable<InvestmentFundDto>>;
