using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking.WebAPI.Data;
using Parking.WebAPI.DTOs;

namespace Parking.WebAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDTO>>> GetUsers()
        {
            var users = await _context.Users
                .AsNoTracking()
                .Select(u => new UserResponseDTO
                {
                    Id = u.Id,
                    Name = u.Name,
                    AuthorizedSlotTypes = u.AuthorizedSlotTypes,
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/users/{userId}/reservations
        [HttpGet("{userId:guid}/reservations")]
        public async Task<ActionResult<IEnumerable<ReservationResponseDTO>>> GetUserReservations(Guid userId)
        {
            // First, verify the user exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                return NotFound(new { message = "User not found." });
            }

            // Fetch, map, and sort their reservations
            var reservations = await _context.Reservations
                .AsNoTracking()
                .Include(r => r.ParkingSlot) // Include to access the Designation
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.StartTime)
                .Select(r => new ReservationResponseDTO
                {
                    Id = r.Id,
                    ParkingSlotId = r.ParkingSlotId,
                    ParkingSlotDesignation = r.ParkingSlot!.Designation, // Map the designation for the UI
                    UserId = r.UserId,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime
                })
                .ToListAsync();

            return Ok(reservations);
        }
    }
}