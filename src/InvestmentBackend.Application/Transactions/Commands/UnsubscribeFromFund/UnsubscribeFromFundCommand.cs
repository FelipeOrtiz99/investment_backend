using MediatR;
using InvestmentBackend.Domain.Entities;

namespace InvestmentBackend.Application.Transactions.Commands.UnsubscribeFromFund;

public record UnsubscribeFromFundCommand(
    string TransactionId
) : IRequest<Transaction>;
