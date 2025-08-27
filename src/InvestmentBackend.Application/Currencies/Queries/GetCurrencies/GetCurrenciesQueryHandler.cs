using MediatR;
using InvestmentBackend.Domain.Repositories;
using InvestmentBackend.Application.Currencies.Queries.GetCurrency;

namespace InvestmentBackend.Application.Currencies.Queries.GetCurrencies;

public class GetCurrenciesQueryHandler : IRequestHandler<GetCurrenciesQuery, IEnumerable<CurrencyDto>>
{
    private readonly ICurrencyRepository _currencyRepository;

    public GetCurrenciesQueryHandler(ICurrencyRepository currencyRepository)
    {
        _currencyRepository = currencyRepository;
    }

    public async Task<IEnumerable<CurrencyDto>> Handle(GetCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var currencies = await _currencyRepository.GetAllAsync(cancellationToken);

        return currencies.Select(currency => new CurrencyDto(
            currency.Id,
            currency.Name,
            currency.CurrencyCode
        ));
    }
}
