import { useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import { axiosInstance } from '../api/axios';
import type { ReservationResponseDTO } from '../models/types';

export interface ReservationRequestDto {
  parkingSlotId: string;
  userId: string;
  startTime: string;
  endTime: string;
}

interface ApiError {
  message: string;
}

export const useCreateReservation = () => {
  const queryClient = useQueryClient();

  return useMutation<ReservationResponseDTO, AxiosError<ApiError>, ReservationRequestDto>({
    mutationFn: async (request) => {
      const response = await axiosInstance.post<ReservationResponseDTO>('/api/reservations', request);
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['parkingSlots'] });
    },
  });
};