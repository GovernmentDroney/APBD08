using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientsService _clientsService;

    public ClientsController(IClientsService clientsService)
    {
        _clientsService = clientsService;
    }
   
    
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClient(int id)
    {
        if( !await _clientsService.DoesClientExist(id)){
         return NotFound();
        }
        var clients = await _clientsService.GetClient(id);
        return Ok(clients);
    }
    
    
    [HttpPost]
    public async Task<IActionResult> PostClient(ClientDTO client)
    {
        int result = await _clientsService.PostClient(client);
        return StatusCode(201, result);
    }
[HttpPut("{id}/trips/{tripId}")]
    public async Task<IActionResult> PutClient(int id, int tripId)
    {
        if( !await _clientsService.DoesClientExist(id)){
            return NotFound();
        }

        if (!await _clientsService.DoesClientExist(tripId))
        {
            return NotFound();
        }

        if (!await _clientsService.IsTripFull(id))
        {
            return StatusCode(503);
        }
        await _clientsService.PutClient( id, tripId);
        return Accepted();
    }

    public async Task<IActionResult> DeleteClient(int id, int tripId)
    {
        if( !await _clientsService.DoesClientExist(id)){
            return NotFound();
        }

        if (!await _clientsService.DoesClientExist(tripId))
        {
            return NotFound();
        }
        await _clientsService.DeleteClient(id,tripId);
        return Ok();
    }
}