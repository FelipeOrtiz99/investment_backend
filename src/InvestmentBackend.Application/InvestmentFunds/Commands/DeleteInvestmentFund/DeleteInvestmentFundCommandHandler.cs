using MediatR;
using InvestmentBackend.Domain.Repositories;

namespace InvestmentBackend.Application.InvestmentFunds.Commands.DeleteInvestmentFund;

public class DeleteInvestmentFundCommandHandler : IRequestHandler<DeleteInvestmentFundCommand, bool>
{
    private readonly IInvestmentFundRepository _investmentFundRepository;

    public DeleteInvestmentFundCommandHandler(IInvestmentFundRepository investmentFundRepository)
    {
        _investmentFundRepository = investmentFundRepository;
    }

    public async Task<bool> Handle(DeleteInvestmentFundCommand request, CancellationToken cancellationToken)
    {
        var fund = await _investmentFundRepository.GetByIdAsync(request.Id, cancellationToken);
        if (fund == null)
            return false;

        await _investmentFundRepository.DeleteAsync(request.Id, cancellationToken);
        return true;
    }
}
