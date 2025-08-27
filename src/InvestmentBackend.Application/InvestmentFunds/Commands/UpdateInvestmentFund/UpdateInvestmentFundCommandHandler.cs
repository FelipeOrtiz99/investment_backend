using MediatR;
using InvestmentBackend.Domain.Entities;
using InvestmentBackend.Domain.Repositories;

namespace InvestmentBackend.Application.InvestmentFunds.Commands.UpdateInvestmentFund;

public class UpdateInvestmentFundCommandHandler : IRequestHandler<UpdateInvestmentFundCommand, InvestmentFund?>
{
    private readonly IInvestmentFundRepository _investmentFundRepository;

    public UpdateInvestmentFundCommandHandler(IInvestmentFundRepository investmentFundRepository)
    {
        _investmentFundRepository = investmentFundRepository;
    }

    public async Task<InvestmentFund?> Handle(UpdateInvestmentFundCommand request, CancellationToken cancellationToken)
    {
        var fund = await _investmentFundRepository.GetByIdAsync(request.Id, cancellationToken);
        if (fund == null)
            return null;

        fund.Name = request.Name;
        fund.Category = request.Category;
        fund.CurrencyId = request.CurrencyId;
        fund.MinInvestment = request.MinInvestment;
        fund.IsActive = request.IsActive;
        
        return await _investmentFundRepository.UpdateAsync(fund, cancellationToken);
    }
}
