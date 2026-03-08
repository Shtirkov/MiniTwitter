import { apiClient } from './client';
import type { LoginRequest, LoginResponse, RegisterRequest, RegisterResponse } from '../types/auth';

export const authApi = {
  login(credentials: LoginRequest): Promise<LoginResponse> {
    return apiClient.post<LoginResponse>('/Auth/login', credentials);
  },

  register(data: RegisterRequest): Promise<RegisterResponse> {
    return apiClient.post<RegisterResponse>('/Auth/register', data);
  },

  logout(): Promise<void> {
    return apiClient.post<void>('/Auth/logout', {});
  },
};
