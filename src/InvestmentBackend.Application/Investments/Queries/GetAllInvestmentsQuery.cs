using MediatR;

namespace InvestmentBackend.Application.Investments.Queries;

public record GetAllInvestmentsQuery : IRequest<IEnumerable<InvestmentDto>>;
