using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AeroTurn.API.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AeroTurn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TurnaroundController : ControllerBase
    {
        private readonly AeroTurnDbContext _context;

        public TurnaroundController(AeroTurnDbContext context)
        {
            _context = context;
        }

        // 1. Python AI (ya Manager/GH) naye events bheje ga
        [HttpPost("log-event")]
        [Authorize(Roles = "TerminalManager,GHStaff")]
        public async Task<IActionResult> LogEvent([FromBody] TurnaroundEvent turnaroundEvent)
        {
            if (turnaroundEvent == null) return BadRequest("No data received.");

            _context.TurnaroundEvents.Add(turnaroundEvent);
            await _context.SaveChangesAsync();

            return Ok(new { message = "AI Event Logged Successfully!", eventId = turnaroundEvent.Id });
        }

        // 2. Events List Dekhna (Security Check Ke Sath)
        [HttpGet("events/{flightId}")]
        [Authorize(Roles = "TerminalManager,GHStaff,AirlineOfficer")]
        public async Task<IActionResult> GetEventsForFlight(int flightId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userAirline = User.FindFirst("AirlineContext")?.Value;

            // Pehle database se flight nikalo taake uski airline pata chale
            var flight = await _context.Flights.FindAsync(flightId);
            if (flight == null)
            {
                return NotFound("Flight not found.");
            }

            // A-CDM SECURITY CROSS-CHECK: Agar Airline Officer hai, toh flight uski apni airline ki honi chahiye
            if (userRole == "AirlineOfficer" && flight.Airline != userAirline)
            {
                return StatusCode(403, "Access Denied! Aap doosri airline ka data nahi dekh sakte.");
            }

            var events = await _context.TurnaroundEvents
                                       .Where(e => e.FlightId == flightId)
                                       .ToListAsync();
            return Ok(events);
        }
    }
}