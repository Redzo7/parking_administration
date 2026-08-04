import { useQuery } from '@tanstack/react-query';
import { axiosInstance } from '../api/axios';
import type { UserResponseDTO } from '../models/types';

export const useUsers = () => {
  return useQuery<UserResponseDTO[], Error>({
    queryKey: ['users'],
    queryFn: async () => {
      const response = await axiosInstance.get<UserResponseDTO[]>('/api/users');
      return response.data;
    },
  });
};