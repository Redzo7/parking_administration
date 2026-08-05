using Parking.WebAPI.Models;

namespace Parking.WebAPI.DTOs
{
    public class UserResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<SlotType> AuthorizedSlotTypes { get; set; } = new();
    }
}
