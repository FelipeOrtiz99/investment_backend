using MediatR;
using InvestmentBackend.Domain.Repositories;

namespace InvestmentBackend.Application.InvestmentFunds.Queries.GetInvestmentFund;

public class GetInvestmentFundQueryHandler : IRequestHandler<GetInvestmentFundQuery, InvestmentFundDto?>
{
    private readonly IInvestmentFundRepository _investmentFundRepository;

    public GetInvestmentFundQueryHandler(IInvestmentFundRepository investmentFundRepository)
    {
        _investmentFundRepository = investmentFundRepository;
    }

    public async Task<InvestmentFundDto?> Handle(GetInvestmentFundQuery request, CancellationToken cancellationToken)
    {
        var fund = await _investmentFundRepository.GetByIdAsync(request.Id, cancellationToken);
        if (fund == null)
            return null;

        return new InvestmentFundDto(
            fund.Id,
            fund.Name,
            fund.Category,
            fund.CurrencyId,
            fund.MinInvestment,
            fund.IsActive
        );
    }
}
