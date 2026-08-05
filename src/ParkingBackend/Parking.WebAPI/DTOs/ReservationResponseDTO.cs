namespace Parking.WebAPI.DTOs
{
    public class ReservationResponseDTO
    {
        public Guid Id { get; set; }
        public Guid ParkingSlotId { get; set; }
        public string ParkingSlotDesignation { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
