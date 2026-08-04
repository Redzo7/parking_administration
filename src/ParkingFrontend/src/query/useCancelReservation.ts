import { useMutation, useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import { axiosInstance } from '../api/axios';

interface ApiError {
  message: string;
}

export const useCancelReservation = () => {
  const queryClient = useQueryClient();

  return useMutation<void, AxiosError<ApiError>, string>({
    mutationFn: async (reservationId: string) => {
      await axiosInstance.delete(`/api/reservations/${reservationId}`);
    },
    onSuccess: () => {
      // Refresh the dashboard data automatically
      queryClient.invalidateQueries({ queryKey: ['parkingSlots'] });
    },
  });
};