using MediatR;
using Microsoft.AspNetCore.Mvc;
using InvestmentBackend.Application.Wallets.Queries.GetClientWallets;
using InvestmentBackend.Application.Transactions.Commands.ProcessTransaction;
using InvestmentBackend.Domain.Entities;

namespace InvestmentBackend.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WalletsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all wallets for a specific client
    /// </summary>
    [HttpGet("client/{clientId}")]
    public async Task<IActionResult> GetClientWallets(string clientId)
    {
        var query = new GetClientWalletsQuery(clientId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Process a wallet transaction (deposit, withdrawal, investment, return)
    /// </summary>
    [HttpPost("transactions")]
    public async Task<IActionResult> ProcessTransaction([FromBody] ProcessTransactionRequest request)
    {
        var command = new ProcessTransactionCommand
        {
            ClientId = request.ClientId,
            CurrencyId = request.CurrencyId,
            Amount = request.Amount,
            Description = request.Description
        };

        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(new { message = result.Message, transactionId = result.TransactionId });
        }

        return Ok(result);
    }
}

public class ProcessTransactionRequest
{
    public string ClientId { get; set; } = string.Empty;
    public string CurrencyId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}
