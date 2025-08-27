using MediatR;
using InvestmentBackend.Domain.Repositories;

namespace InvestmentBackend.Application.Currencies.Queries.GetCurrency;

public class GetCurrencyQueryHandler : IRequestHandler<GetCurrencyQuery, CurrencyDto?>
{
    private readonly ICurrencyRepository _currencyRepository;

    public GetCurrencyQueryHandler(ICurrencyRepository currencyRepository)
    {
        _currencyRepository = currencyRepository;
    }

    public async Task<CurrencyDto?> Handle(GetCurrencyQuery request, CancellationToken cancellationToken)
    {
        var currency = await _currencyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (currency == null)
            return null;

        return new CurrencyDto(
            currency.Id,
            currency.Name,
            currency.CurrencyCode
        );
    }
}
