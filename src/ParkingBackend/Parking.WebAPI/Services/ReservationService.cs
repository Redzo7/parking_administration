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

            // Ensure the User and ParkingSlot actually exist
            bool slotExists = await _context.ParkingSlots.AnyAsync(p => p.Id == request.ParkingSlotId);
            if (!slotExists) throw new ArgumentException("Invalid ParkingSlotId.");

            bool userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
            if (!userExists) throw new ArgumentException("Invalid UserId.");

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
    }
}
