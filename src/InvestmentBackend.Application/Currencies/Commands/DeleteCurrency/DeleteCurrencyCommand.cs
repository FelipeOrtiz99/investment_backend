using MediatR;

namespace InvestmentBackend.Application.Currencies.Commands.DeleteCurrency;

public record DeleteCurrencyCommand(string Id) : IRequest<bool>;
