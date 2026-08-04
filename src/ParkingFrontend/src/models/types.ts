export interface UserResponseDTO {
  id: string;
  name: string;
}

export interface ReservationResponseDTO {
  id: string;
  parkingSlotId: string;
  userId: string;
  startTime: string;
  endTime: string;
}

export interface ParkingSlotResponseDTO {
  id: string;
  designation: string;
  reservations: ReservationResponseDTO[];
}