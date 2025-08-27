// using MediatR;
// using Microsoft.AspNetCore.Mvc;
// using InvestmentBackend.Application.Clients.Commands.CreateClient;
// using InvestmentBackend.Application.Clients.Commands.UpdateClient;
// using InvestmentBackend.Application.Clients.Commands.DeleteClient;
// using InvestmentBackend.Application.Clients.Queries.GetClient;
// using InvestmentBackend.Application.Clients.Queries.GetClients;

// namespace InvestmentBackend.WebApi.Controllers;

// [ApiController]
// [Route("api/[controller]")]
// public class ClientsController : ControllerBase
// {
//     private readonly IMediator _mediator;

//     public ClientsController(IMediator mediator)
//     {
//         _mediator = mediator;
//     }

//     /// <summary>
//     /// Get all clients
//     /// </summary>
//     [HttpGet]
//     public async Task<IActionResult> GetClients()
//     {
//         var query = new GetClientsQuery();
//         var result = await _mediator.Send(query);
//         return Ok(result);
//     }

//     /// <summary>
//     /// Get client by ID
//     /// </summary>
//     [HttpGet("{id}")]
//     public async Task<IActionResult> GetClient(string id)
//     {
//         var query = new GetClientQuery(id);
//         var result = await _mediator.Send(query);
        
//         if (result == null)
//             return NotFound();
            
//         return Ok(result);
//     }

//     /// <summary>
//     /// Create a new client
//     /// </summary>
//     [HttpPost]
//     public async Task<IActionResult> CreateClient([FromBody] CreateClientCommand command)
//     {
//         var result = await _mediator.Send(command);
//         return CreatedAtAction(nameof(GetClient), new { id = result.Id }, result);
//     }

//     /// <summary>
//     /// Update an existing client
//     /// </summary>
//     [HttpPut("{id}")]
//     public async Task<IActionResult> UpdateClient(string id, [FromBody] UpdateClientCommand command)
//     {
//         if (id != command.Id)
//             return BadRequest("ID mismatch");

//         var result = await _mediator.Send(command);
        
//         if (result == null)
//             return NotFound();
            
//         return Ok(result);
//     }

//     /// <summary>
//     /// Delete a client
//     /// </summary>
//     [HttpDelete("{id}")]
//     public async Task<IActionResult> DeleteClient(string id)
//     {
//         var command = new DeleteClientCommand(id);
//         var success = await _mediator.Send(command);
        
//         if (!success)
//             return NotFound();
            
//         return NoContent();
//     }
// }