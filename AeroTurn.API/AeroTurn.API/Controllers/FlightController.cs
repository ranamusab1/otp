using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AeroTurn.API.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AeroTurn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bina login koi access nahi
    public class FlightController : ControllerBase
    {
        private readonly AeroTurnDbContext _context;

        public FlightController(AeroTurnDbContext context)
        {
            _context = context;
        }

        // 1. Nayi Flight Schedule Karna (Sirf Manager aur us specific Airline ka Officer kar sakta hai)
        [HttpPost("add-flight")]
        [Authorize(Roles = "TerminalManager,AirlineOfficer")]
        public async Task<IActionResult> AddFlight([FromBody] Flight newFlight)
        {
            // User ka Role aur Airline check karna
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userAirline = User.FindFirst("AirlineContext")?.Value;

            // SECURITY: Agar Airline Officer hai, toh wo sirf apni hi airline ki flight daal sakta hai
            if (userRole == "AirlineOfficer" && newFlight.Airline != userAirline)
            {
                return StatusCode(403, "Access Denied! Aap sirf apni airline ki flights schedule kar sakte hain.");
            }

            // Default status
            newFlight.Status = "Scheduled";

            _context.Flights.Add(newFlight);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Flight scheduled successfully!", flightId = newFlight.Id });
        }

        // 2. Flights ki list dekhna (Dashboard ke liye)
        [HttpGet("all-flights")]
        [Authorize(Roles = "TerminalManager,GHStaff,AirlineOfficer")]
        public async Task<IActionResult> GetAllFlights()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userAirline = User.FindFirst("AirlineContext")?.Value;

            // Agar Manager ya GH Staff hai, toh saari flights dikhao
            if (userRole == "TerminalManager" || userRole == "GHStaff")
            {
                var allFlights = await _context.Flights.ToListAsync();
                return Ok(allFlights);
            }
            // Agar Airline Officer hai, toh sirf uski apni airline ki flights dikhao
            else if (userRole == "AirlineOfficer")
            {
                var airlineFlights = await _context.Flights
                                             .Where(f => f.Airline == userAirline)
                                             .ToListAsync();
                return Ok(airlineFlights);
            }

            return BadRequest("Role not identified.");
        }
    }
}