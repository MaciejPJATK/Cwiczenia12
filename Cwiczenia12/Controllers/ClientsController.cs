using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cwiczenia12.Models;

namespace Cwiczenia12.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly Cwiczenia12DbContext _context;

    public ClientsController(Cwiczenia12DbContext context)
    {
        _context = context;
    }

    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient)
    {
        var client = await _context.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == idClient);

        if (client == null)
            return NotFound(new { message = "Client not found" });

        if (client.ClientTrips.Any())
            return BadRequest(new { message = "Client cannot be deleted because they are assigned to a trip." });

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Client deleted successfully" });
    }
}