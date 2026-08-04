using System.ComponentModel.DataAnnotations;

namespace Parking.WebAPI.DTOs
{
    public class ReservationRequestDTO
    {
        [Required]
        public Guid ParkingSlotId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
    }
}
