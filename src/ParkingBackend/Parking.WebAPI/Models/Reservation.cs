using System;

namespace Parking.WebAPI.Models
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Foreign Key for User
        public Guid UserId { get; set; }
        public User? User { get; set; }

        // Foreign Key for ParkingSlot
        public Guid ParkingSlotId { get; set; }
        public ParkingSlot? ParkingSlot { get; set; }
    }
}