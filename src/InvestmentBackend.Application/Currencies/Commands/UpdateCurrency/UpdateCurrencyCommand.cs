using MediatR;
using InvestmentBackend.Domain.Entities;

namespace InvestmentBackend.Application.Currencies.Commands.UpdateCurrency;

public record UpdateCurrencyCommand(string Id, string Name, string CurrencyCode) : IRequest<Currency?>;
