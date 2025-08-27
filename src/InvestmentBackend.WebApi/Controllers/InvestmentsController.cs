using MediatR;
using Microsoft.AspNetCore.Mvc;
using InvestmentBackend.Application.Investments.Commands;
using InvestmentBackend.Application.Investments.Queries;

namespace InvestmentBackend.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvestmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvestmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all investments
    /// </summary>
    /// <returns>List of investments</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvestmentDto>>> GetAllInvestments(CancellationToken cancellationToken)
    {
        var query = new GetAllInvestmentsQuery();
        var investments = await _mediator.Send(query, cancellationToken);
        return Ok(investments);
    }

    /// <summary>
    /// Gets an investment by ID
    /// </summary>
    /// <param name="id">Investment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Investment details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<InvestmentDto>> GetInvestmentById(string id, CancellationToken cancellationToken)
    {
        var query = new GetInvestmentByIdQuery(id);
        var investment = await _mediator.Send(query, cancellationToken);
        
        if (investment == null)
            return NotFound($"Investment with ID {id} not found.");
        
        return Ok(investment);
    }

    // /// <summary>
    // /// Creates a new investment
    // /// </summary>
    // /// <param name="request">Investment creation request</param>
    // /// <param name="cancellationToken">Cancellation token</param>
    // /// <returns>Created investment</returns>
    // [HttpPost]
    // public async Task<ActionResult<CreateInvestmentResult>> CreateInvestment(
    //     [FromBody] CreateInvestmentRequest request, 
    //     CancellationToken cancellationToken)
    // {
    //     var command = new CreateInvestmentCommand(
    //         request.Name,
    //         request.Description,
    //         request.Amount,
    //         request.InvestmentType,
    //         request.ExpectedReturn,
    //         request.Currency ?? "USD",
    //         request.StartDate,
    //         request.EndDate,
    //         request.RiskLevel ?? "Medium"
    //     );

    //     var result = await _mediator.Send(command, cancellationToken);
    //     return CreatedAtAction(nameof(GetInvestmentById), new { id = result.Id }, result);
    // }

    // /// <summary>
    // /// Updates an existing investment
    // /// </summary>
    // /// <param name="id">Investment ID</param>
    // /// <param name="request">Investment update request</param>
    // /// <param name="cancellationToken">Cancellation token</param>
    // /// <returns>Updated investment</returns>
    // [HttpPut("{id}")]
    // public async Task<ActionResult<UpdateInvestmentResult>> UpdateInvestment(
    //     string id,
    //     [FromBody] UpdateInvestmentRequest request,
    //     CancellationToken cancellationToken)
    // {
    //     try
    //     {
    //         var command = new UpdateInvestmentCommand(
    //             id,
    //             request.Name,
    //             request.Description,
    //             request.Amount,
    //             request.InvestmentType,
    //             request.ExpectedReturn,
    //             request.Currency ?? "USD",
    //             request.StartDate,
    //             request.EndDate,
    //             request.RiskLevel ?? "Medium"
    //         );

    //         var result = await _mediator.Send(command, cancellationToken);
    //         return Ok(result);
    //     }
    //     catch (InvalidOperationException ex)
    //     {
    //         return NotFound(ex.Message);
    //     }
    // }

    /// <summary>
    /// Deletes an investment
    /// </summary>
    /// <param name="id">Investment ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteInvestment(string id, CancellationToken cancellationToken)
    {
        var command = new DeleteInvestmentCommand(id);
        var success = await _mediator.Send(command, cancellationToken);
        
        if (!success)
            return NotFound($"Investment with ID {id} not found.");
        
        return NoContent();
    }
}

public record CreateInvestmentRequest(
    string Name,
    string Description,
    decimal Amount,
    string InvestmentType,
    decimal ExpectedReturn,
    string? Currency,
    DateTime StartDate,
    DateTime? EndDate,
    string? RiskLevel
);

public record UpdateInvestmentRequest(
    string Name,
    string Description,
    decimal Amount,
    string InvestmentType,
    decimal ExpectedReturn,
    string? Currency,
    DateTime StartDate,
    DateTime? EndDate,
    string? RiskLevel
);
