using MediatR;
using InvestmentBackend.Domain.Entities;
using InvestmentBackend.Domain.Repositories;

namespace InvestmentBackend.Application.InvestmentFunds.Commands.CreateInvestmentFund;

public class CreateInvestmentFundCommandHandler : IRequestHandler<CreateInvestmentFundCommand, InvestmentFund>
{
    private readonly IInvestmentFundRepository _investmentFundRepository;

    public CreateInvestmentFundCommandHandler(IInvestmentFundRepository investmentFundRepository)
    {
        _investmentFundRepository = investmentFundRepository;
    }

    public async Task<InvestmentFund> Handle(CreateInvestmentFundCommand request, CancellationToken cancellationToken)
    {
        var fund = new InvestmentFund
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Category = request.Category,
            CurrencyId = request.CurrencyId,
            MinInvestment = request.MinInvestment,
            IsActive = true
        };
        
        return await _investmentFundRepository.CreateAsync(fund, cancellationToken);
    }
}
