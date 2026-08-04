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

                // Return 201 Created. 
                // We use nameof(CreateReservation) just as a placeholder for the Location header URI, 
                // but typically this points to a GET endpoint for the specific resource.
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
        public async Task<IActionResult> CancelReservation(Guid id)
        {
            try
            {
                await _reservationService.CancelReservationAsync(id);
                return NoContent(); // Returns 204
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Returns 400 Bad Request if the reservation cannot be canceled due to business rules (e.g., already started)
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}