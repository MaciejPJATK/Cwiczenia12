using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cwiczenia12.Models;
using Cwiczenia12.DTOs;

namespace Cwiczenia12.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly Cwiczenia12DbContext _context;

    public TripsController(Cwiczenia12DbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var totalTrips = await _context.Trips.CountAsync();
        var totalPages = (int)Math.Ceiling(totalTrips / (double)pageSize);

        var trips = await _context.Trips
            .Include(t => t.ClientTrips).ThenInclude(ct => ct.Client)
            .Include(t => t.CountryTrips).ThenInclude(ct => ct.Country)
            .OrderByDescending(t => t.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new
            {
                t.Name,
                t.Description,
                t.DateFrom,
                t.DateTo,
                t.MaxPeople,
                Countries = t.CountryTrips.Select(ct => new { ct.Country.Name }),
                Clients = t.ClientTrips.Select(ct => new { ct.Client.FirstName, ct.Client.LastName })
            })
            .ToListAsync();

        return Ok(new
        {
            pageNum = page,
            pageSize = pageSize,
            allPages = totalPages,
            trips
        });
    }
    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AddClientToTrip(int idTrip, [FromBody] ClientTripRequest request)
    {
        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.IdTrip == idTrip);
        if (trip == null)
            return BadRequest(new { message = "Trip not found" });

        if (trip.DateFrom <= DateTime.Now)
            return BadRequest(new { message = "Cannot assign client to a past trip" });

        var existingClient = await _context.Clients.FirstOrDefaultAsync(c => c.Pesel == request.Pesel);
        if (existingClient != null)
        {
            bool alreadyAssigned = await _context.ClientTrips.AnyAsync(ct => ct.IdTrip == idTrip && ct.IdClient == existingClient.IdClient);
            if (alreadyAssigned)
                return BadRequest(new { message = "Client already assigned to this trip" });
        }

        var client = existingClient ?? new Client
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Telephone = request.Telephone,
            Pesel = request.Pesel
        };

        var clientTrip = new ClientTrip
        {
            Client = client,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = request.PaymentDate
        };

        _context.ClientTrips.Add(clientTrip);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Client assigned to trip" });
    }


}