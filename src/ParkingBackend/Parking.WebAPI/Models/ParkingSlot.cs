using System;
using System.Collections.Generic;

namespace Parking.WebAPI.Models
{
    public class ParkingSlot
    {
        public Guid Id { get; set; }
        public string Designation { get; set; } = string.Empty;
        public SlotType Type { get; set; } = SlotType.Regular;

        // Navigation property
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}