using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Parking.WebAPI.DTOs;
using Parking.WebAPI.Services;

namespace Parking.WebAPI.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        // POST: api/reservations
        [HttpPost]
        public async Task<ActionResult<ReservationResponseDTO>> CreateReservation([FromBody] ReservationRequestDTO request)
        {
            try
            {
                var response = await _reservationService.CreateReservationAsync(request);

                return Created(string.Empty, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/reservations/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelReservation(Guid id, [FromHeader(Name = "X-User-Id")] Guid requestingUserId)
        {
            try
            {
                if(requestingUserId == Guid.Empty)
                {
                    return BadRequest(new { message = "The X-User-Id header is missing." });
                }
                await _reservationService.CancelReservationAsync(id, requestingUserId);
                return NoContent(); // Returns 204
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while canceling the reservation." });
            }
        }
    }
}