using MediatR;

namespace InvestmentBackend.Application.Currencies.Queries.GetCurrency;

public record GetCurrencyQuery(string Id) : IRequest<CurrencyDto?>;

public record CurrencyDto(
    string Id,
    string Name,
    string CurrencyCode
);
