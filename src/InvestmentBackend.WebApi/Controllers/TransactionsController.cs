using MediatR;
using Microsoft.AspNetCore.Mvc;
using InvestmentBackend.Application.Transactions.Queries.GetTransaction;
using InvestmentBackend.Application.Transactions.Queries.GetTransactions;
using InvestmentBackend.Application.Transactions.Queries;
using InvestmentBackend.Application.Transactions.Queries.GetActiveTransactions;
using InvestmentBackend.Application.Transactions.Commands.CreateTransaction;
using InvestmentBackend.Application.Transactions.Commands.UpdateTransaction;
using InvestmentBackend.Application.Transactions.Commands.DeleteTransaction;
using InvestmentBackend.Application.Transactions.Commands.UnsubscribeFromFund;
using InvestmentBackend.Domain.Entities;

namespace InvestmentBackend.WebApi.Controllers;

public class CreateInvestmentTransactionRequest
{
    public string ClientId { get; set; } = string.Empty;
    public string CurrencyId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string InvestmentFundId { get; set; } = string.Empty;
    public string? Description { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all transactions
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetTransactions()
    {
        var query = new GetTransactionsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get all transactions with related entities (Client and Currency)
    /// </summary>
    [HttpGet("with-relations")]
    public async Task<IActionResult> GetTransactionsWithRelations()
    {
        var query = new GetTransactionsWithRelationsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get all active transactions (Status = true) with related entities
    /// </summary>
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveTransactions()
    {
        var query = new GetActiveTransactionsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get transaction by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransaction(string id)
    {
        var query = new GetTransactionQuery(id);
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// Get transaction by ID with related entities (Client and Currency)
    /// </summary>
    [HttpGet("{id}/with-relations")]
    public async Task<IActionResult> GetTransactionWithRelations(string id)
    {
        var query = new GetTransactionWithRelationsQuery(id);
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// Create a new transaction
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTransaction), new { id = result.Id }, result);
    }

    /// <summary>
    /// Create a new investment transaction
    /// </summary>
    [HttpPost("investment")]
    public async Task<IActionResult> CreateInvestmentTransaction([FromBody] CreateInvestmentTransactionRequest request)
    {
        var command = new CreateTransactionCommand(
            IdClient: request.ClientId,
            CurrencyId: request.CurrencyId,
            Amount: request.Amount,
            Description: request.Description ?? $"Investment in fund {request.InvestmentFundId}",
            InvestmentFundId: request.InvestmentFundId
        );

        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTransaction), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing transaction
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(string id, [FromBody] UpdateTransactionCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(command);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// Unsubscribe from investment fund and update client balance
    /// </summary>
    [HttpPut("{id}/unsubscribe")]
    public async Task<IActionResult> UnsubscribeFromFund(string id)
    {
        var command = new UnsubscribeFromFundCommand(id);

        try
        {
            var result = await _mediator.Send(command);
            return Ok(new 
            { 
                message = "Successfully unsubscribed from investment fund",
                transaction = result,
                balanceUpdated = true
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "An error occurred while unsubscribing from the fund" });
        }
    }

    /// <summary>
    /// Delete a transaction
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(string id)
    {
        var command = new DeleteTransactionCommand(id);
        var success = await _mediator.Send(command);
        
        if (!success)
            return NotFound();
            
        return NoContent();
    }
}
