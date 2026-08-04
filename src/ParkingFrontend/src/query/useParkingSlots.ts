import { useQuery } from '@tanstack/react-query';
import { axiosInstance } from '../api/axios';
import type { ParkingSlotResponseDTO } from '../models/types';

export const useParkingSlots = () => {
  return useQuery<ParkingSlotResponseDTO[], Error>({
    queryKey: ['parkingSlots'],
    queryFn: async () => {
      const response = await axiosInstance.get<ParkingSlotResponseDTO[]>('/api/parkingslots');
      return response.data;
    },
  });
};