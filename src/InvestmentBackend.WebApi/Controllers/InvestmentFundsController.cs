using MediatR;
using Microsoft.AspNetCore.Mvc;
using InvestmentBackend.Application.InvestmentFunds.Queries.GetInvestmentFund;
using InvestmentBackend.Application.InvestmentFunds.Queries.GetInvestmentFunds;
using InvestmentBackend.Application.InvestmentFunds.Commands.CreateInvestmentFund;
using InvestmentBackend.Application.InvestmentFunds.Commands.UpdateInvestmentFund;
using InvestmentBackend.Application.InvestmentFunds.Commands.DeleteInvestmentFund;

namespace InvestmentBackend.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvestmentFundsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvestmentFundsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all investment funds
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetInvestmentFunds()
    {
        var query = new GetInvestmentFundsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get investment fund by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInvestmentFund(string id)
    {
        var query = new GetInvestmentFundQuery(id);
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// Create a new investment fund
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateInvestmentFund([FromBody] CreateInvestmentFundCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetInvestmentFund), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update an existing investment fund
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInvestmentFund(string id, [FromBody] UpdateInvestmentFundCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(command);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    /// <summary>
    /// Delete an investment fund
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInvestmentFund(string id)
    {
        var command = new DeleteInvestmentFundCommand(id);
        var success = await _mediator.Send(command);
        
        if (!success)
            return NotFound();
            
        return NoContent();
    }
}
