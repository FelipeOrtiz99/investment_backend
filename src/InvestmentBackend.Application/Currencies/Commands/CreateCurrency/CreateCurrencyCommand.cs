using MediatR;
using InvestmentBackend.Domain.Entities;

namespace InvestmentBackend.Application.Currencies.Commands.CreateCurrency;

public record CreateCurrencyCommand(string Name, string CurrencyCode) : IRequest<Currency>;
