using Microsoft.EntityFrameworkCore;
using Parking.WebAPI.Data;
using Parking.WebAPI.DTOs;
using Parking.WebAPI.Models;

namespace Parking.WebAPI.Services
{
    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;

        public ReservationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReservationResponseDTO> CreateReservationAsync(ReservationRequestDTO request)
        {
            var slot = await _context.ParkingSlots.FindAsync(request.ParkingSlotId);
            if(slot == null) throw new KeyNotFoundException("Parking slot not found.");

            var user = await _context.Users.FindAsync(request.UserId);
            if(user == null) throw new KeyNotFoundException("User not found.");

            if(slot.Type != SlotType.Regular && !user.AuthorizedSlotTypes.Contains(slot.Type))
            {
                throw new InvalidOperationException($"User is not authorized to book this {slot.Type} parking slot.");
            }

            // 1. Ensure timestamps are treated as UTC
            var startTimeUtc = request.StartTime.ToUniversalTime();
            var endTimeUtc = request.EndTime.ToUniversalTime();

            // 2. Reject if StartTime is in the past
            if (startTimeUtc < DateTime.UtcNow)
            {
                throw new ArgumentException("Reservation start time cannot be in the past.");
            }

            // 3. Reject if duration is less than 30 minutes
            var duration = endTimeUtc - startTimeUtc;
            if (duration.TotalMinutes < 30)
            {
                throw new ArgumentException("Reservation duration must be at least 30 minutes.");
            }

            // 4. Reject if an overlapping reservation exists for the same ParkingSlotId
            bool hasOverlap = await _context.Reservations
                .AnyAsync(r => r.ParkingSlotId == request.ParkingSlotId &&
                               r.StartTime < endTimeUtc &&
                               r.EndTime > startTimeUtc);

            if (hasOverlap)
            {
                throw new InvalidOperationException("The parking slot is already booked for the requested time period.");
            }

            // 5. Map DTO to Model
            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                ParkingSlotId = request.ParkingSlotId,
                UserId = request.UserId,
                StartTime = startTimeUtc,
                EndTime = endTimeUtc
            };

            // 6. Save to Database
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            // 7. Return Response DTO
            return new ReservationResponseDTO
            {
                Id = reservation.Id,
                ParkingSlotId = reservation.ParkingSlotId,
                UserId = reservation.UserId,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime
            };
        }

        public async Task CancelReservationAsync(Guid reservationId, Guid requestingUserId)
        {
            // Retrieve the reservation from the database
            var reservation = await _context.Reservations.FindAsync(reservationId);

            if (reservation == null)
            {
                throw new KeyNotFoundException($"Reservation with ID {reservationId} was not found.");
            }

            // Security check - ensure ownership
            if(reservation.UserId != requestingUserId)
            {
                throw new UnauthorizedAccessException("You are not authorized to cancel this reservation.");
            }

            // Check the StartTime against DateTime.UtcNow
            if (reservation.StartTime <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("Cannot cancel a reservation that has already started or finished.");
            }

            // Remove from database and save
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }
    }
}
