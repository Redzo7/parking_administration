using System;
using System.Collections.Generic;

namespace Parking.WebAPI.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<SlotType> AuthorizedSlotTypes { get; set; } = new List<SlotType> { SlotType.Regular };

        // Navigation property
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}