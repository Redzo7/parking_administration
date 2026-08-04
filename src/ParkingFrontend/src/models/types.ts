export const SlotType = {
    Regular: 0,
    VIP: 1,
    Electric: 2,
    Accessible: 3
} as const;

export type SlotType = (typeof SlotType)[keyof typeof SlotType]

export interface UserResponseDTO {
  id: string;
  name: string;
  authorizedSlotTypes: SlotType[];
}

export interface ReservationResponseDTO {
  id: string;
  parkingSlotId: string;
  parkingSlotDesignation?: string;
  userId: string;
  startTime: string;
  endTime: string;
}

export interface ParkingSlotResponseDTO {
  id: string;
  designation: string;
  type: SlotType;
  reservations: ReservationResponseDTO[];
}