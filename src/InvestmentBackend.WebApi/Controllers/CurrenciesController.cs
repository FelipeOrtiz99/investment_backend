using MediatR;
using Microsoft.AspNetCore.Mvc;
using InvestmentBackend.Application.Currencies.Queries.GetCurrency;
using InvestmentBackend.Application.Currencies.Queries.GetCurrencies;
using InvestmentBackend.Application.Currencies.Commands.CreateCurrency;
using InvestmentBackend.Application.Currencies.Commands.UpdateCurrency;
using InvestmentBackend.Application.Currencies.Commands.DeleteCurrency;

namespace InvestmentBackend.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrenciesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CurrenciesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all currencies
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCurrencies()
    {
        var query = new GetCurrenciesQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get currency by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCurrency(string id)
    {
        var query = new GetCurrencyQuery(id);
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// Create a new currency
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateCurrency([FromBody] CreateCurrencyCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCurrency), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing currency
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCurrency(string id, [FromBody] UpdateCurrencyCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(command);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// Delete a currency
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCurrency(string id)
    {
        var command = new DeleteCurrencyCommand(id);
        var success = await _mediator.Send(command);
        
        if (!success)
            return NotFound();
            
        return NoContent();
    }
}
