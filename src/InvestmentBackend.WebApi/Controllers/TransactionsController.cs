using MediatR;
using Microsoft.AspNetCore.Mvc;
using InvestmentBackend.Application.Transactions.Queries.GetTransaction;
using InvestmentBackend.Application.Transactions.Queries.GetTransactions;
using InvestmentBackend.Application.Transactions.Commands.CreateTransaction;
using InvestmentBackend.Application.Transactions.Commands.UpdateTransaction;
using InvestmentBackend.Application.Transactions.Commands.DeleteTransaction;

namespace InvestmentBackend.WebApi.Controllers;

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
    /// Create a new transaction
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTransaction), new { id = result.Id }, result);
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
