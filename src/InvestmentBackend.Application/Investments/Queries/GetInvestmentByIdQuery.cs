using MediatR;

namespace InvestmentBackend.Application.Investments.Queries;

public record GetInvestmentByIdQuery(string Id) : IRequest<InvestmentDto?>;

public record InvestmentDto(
    string Id,
    string IdClient,
    decimal Amount,
    int CurrencyId,
    string? InvestmentFundId,
    DateTime InvestmentDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool IsActive,
    decimal CurrentValue,
    decimal ReturnPercentage,
    decimal ReturnAmount
);
