using Parking.WebAPI.DTOs;

namespace Parking.WebAPI.Services
{
    public interface IReservationService
    {
        Task<ReservationResponseDTO> CreateReservationAsync(ReservationRequestDTO request);
    }
}
