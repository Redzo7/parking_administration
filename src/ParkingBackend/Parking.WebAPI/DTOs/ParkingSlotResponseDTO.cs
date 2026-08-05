using Parking.WebAPI.Models;

namespace Parking.WebAPI.DTOs
{
    public class ParkingSlotResponseDTO
    {
        public Guid Id { get; set; }
        public string Designation { get; set; } = string.Empty;
        public SlotType Type { get; set; }

        public IEnumerable<ReservationResponseDTO> Reservations { get; set; } = new List<ReservationResponseDTO>();
    }
}
