import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { authApi } from '../api/authApi';
import { storage } from '../utils/storage';
import { ROUTES } from '../utils/constants';
import { HttpError } from '../api/client';
import type { LoginRequest } from '../types/auth';

interface UseLoginReturn {
  error: string | null;
  isLoading: boolean;
  submit: (data: LoginRequest) => Promise<void>;
}

function resolveErrorMessage(err: unknown): string {
  if (err instanceof HttpError) {
    const body = err.body as { Error?: string } | null;
    if (err.status === 401) {
      return body?.Error ?? 'Invalid credentials. Please check your username and password.';
    }
    return body?.Error ?? 'Something went wrong. Please try again.';
  }
  return 'Unable to connect. Please check your network and try again.';
}

export function useLogin(): UseLoginReturn {
  const navigate = useNavigate();
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const submit = async (data: LoginRequest): Promise<void> => {
    setError(null);
    setIsLoading(true);

    try {
      const response = await authApi.login(data);
      storage.setToken(response.token);
      storage.setUser({ username: response.username, email: response.email });
      navigate(ROUTES.FEED);
    } catch (err) {
      setError(resolveErrorMessage(err));
    } finally {
      setIsLoading(false);
    }
  };

  return { error, isLoading, submit };
}
