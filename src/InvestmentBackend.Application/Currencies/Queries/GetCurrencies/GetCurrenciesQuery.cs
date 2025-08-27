using MediatR;
using InvestmentBackend.Application.Currencies.Queries.GetCurrency;

namespace InvestmentBackend.Application.Currencies.Queries.GetCurrencies;

public record GetCurrenciesQuery : IRequest<IEnumerable<CurrencyDto>>;
