using MediatR;
using InvestmentBackend.Domain.Repositories;

namespace InvestmentBackend.Application.Currencies.Commands.DeleteCurrency;

public class DeleteCurrencyCommandHandler : IRequestHandler<DeleteCurrencyCommand, bool>
{
    private readonly ICurrencyRepository _currencyRepository;

    public DeleteCurrencyCommandHandler(ICurrencyRepository currencyRepository)
    {
        _currencyRepository = currencyRepository;
    }

    public async Task<bool> Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
    {
        var currency = await _currencyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (currency == null)
            return false;

        await _currencyRepository.DeleteAsync(request.Id, cancellationToken);
        return true;
    }
}
