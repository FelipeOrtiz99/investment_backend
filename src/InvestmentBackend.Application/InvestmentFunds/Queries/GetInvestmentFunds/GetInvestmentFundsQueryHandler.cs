using MediatR;
using InvestmentBackend.Domain.Repositories;
using InvestmentBackend.Application.InvestmentFunds.Queries.GetInvestmentFund;

namespace InvestmentBackend.Application.InvestmentFunds.Queries.GetInvestmentFunds;

public class GetInvestmentFundsQueryHandler : IRequestHandler<GetInvestmentFundsQuery, IEnumerable<InvestmentFundDto>>
{
    private readonly IInvestmentFundRepository _investmentFundRepository;

    public GetInvestmentFundsQueryHandler(IInvestmentFundRepository investmentFundRepository)
    {
        _investmentFundRepository = investmentFundRepository;
    }

    public async Task<IEnumerable<InvestmentFundDto>> Handle(GetInvestmentFundsQuery request, CancellationToken cancellationToken)
    {
        var funds = await _investmentFundRepository.GetAllAsync(cancellationToken);

        return funds.Select(fund => new InvestmentFundDto(
            fund.Id,
            fund.Name,
            fund.Category,
            fund.CurrencyId,
            fund.MinInvestment,
            fund.IsActive
        ));
    }
}
