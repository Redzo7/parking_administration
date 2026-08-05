using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking.WebAPI.Data;
using Parking.WebAPI.DTOs;

namespace Parking.WebAPI.Controllers
{
    [ApiController]
    [Route("api/parkingslots")]
    public class ParkingSlotController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ParkingSlotController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/parkingslots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParkingSlotResponseDTO>>> GetAllParkingSlots()
        {
            var slots = await _context.ParkingSlots
                .AsNoTracking()
                .Include(p => p.Reservations)
                .Select(p => new ParkingSlotResponseDTO
                {
                    Id = p.Id,
                    Designation = p.Designation,
                    Type = p.Type,
                    Reservations = p.Reservations.Select(r => new ReservationResponseDTO
                    {
                        Id = r.Id,
                        ParkingSlotId = r.ParkingSlotId,
                        UserId = r.UserId,
                        StartTime = r.StartTime,
                        EndTime = r.EndTime
                    }).ToList()
                })
                .ToListAsync();

            return Ok(slots);
        }

        // GET: api/parkingslots/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ParkingSlotResponseDTO>> GetParkingSlotById(Guid id)
        {
            var slot = await _context.ParkingSlots
                .AsNoTracking()
                .Include(p => p.Reservations)
                .Where(p => p.Id == id)
                .Select(p => new ParkingSlotResponseDTO
                {
                    Id = p.Id,
                    Designation = p.Designation,
                    Type = p.Type,
                    Reservations = p.Reservations.Select(r => new ReservationResponseDTO
                    {
                        Id = r.Id,
                        ParkingSlotId = r.ParkingSlotId,
                        UserId = r.UserId,
                        StartTime = r.StartTime,
                        EndTime = r.EndTime
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (slot == null)
            {
                return NotFound(new { message = $"Parking slot with ID {id} was not found." });
            }

            return Ok(slot);
        }
    }
}
