import { useQuery } from '@tanstack/react-query';
import { axiosInstance } from '../api/axios';
import type { ReservationResponseDTO } from '../models/types';

export const useUserReservations = (userId: string) => {
  return useQuery<ReservationResponseDTO[], Error>({
    queryKey: ['userReservations', userId],
    queryFn: async () => {
      const response = await axiosInstance.get<ReservationResponseDTO[]>(`/api/users/${userId}/reservations`);
      return response.data;
    },
    // Don't run the query if there is no user selected
    enabled: !!userId,
  });
};