using MediatR;

namespace InvestmentBackend.Application.Investments.Commands;

public record DeleteInvestmentCommand(string Id) : IRequest<bool>;
