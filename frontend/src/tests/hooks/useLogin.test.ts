import { renderHook, act, waitFor } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import type { ReactNode } from 'react';
import { MemoryRouter } from 'react-router-dom';
import { createElement } from 'react';
import { useLogin } from '../../hooks/useLogin';
import { authApi } from '../../api/authApi';
import { storage } from '../../utils/storage';
import { HttpError } from '../../api/client';
import { ROUTES } from '../../utils/constants';

// Mock dependencies
vi.mock('../../api/authApi');
vi.mock('../../utils/storage');

const mockNavigate = vi.fn();
vi.mock('react-router-dom', async (importOriginal) => {
  const mod = await importOriginal<typeof import('react-router-dom')>();
  return { ...mod, useNavigate: () => mockNavigate };
});

const VALID_CREDENTIALS = { email: 'john@example.com', password: 'secret' };
const MOCK_RESPONSE = { username: 'johndoe', email: 'john@example.com', token: 'tok' };

function wrapper({ children }: { children: ReactNode }) {
  return createElement(MemoryRouter, null, children);
}

describe('useLogin', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('starts with no error and not loading', () => {
    // Arrange / Act
    const { result } = renderHook(() => useLogin(), { wrapper });

    // Assert
    expect(result.current.error).toBeNull();
    expect(result.current.isLoading).toBe(false);
  });

  it('sets isLoading to true while the request is in flight', async () => {
    // Arrange
    let resolve!: (value: typeof MOCK_RESPONSE) => void;
    vi.mocked(authApi.login).mockReturnValueOnce(new Promise((r) => (resolve = r)));

    const { result } = renderHook(() => useLogin(), { wrapper });

    // Act
    act(() => {
      void result.current.submit(VALID_CREDENTIALS);
    });

    // Assert — loading should be true before promise resolves
    expect(result.current.isLoading).toBe(true);

    // Cleanup
    resolve(MOCK_RESPONSE);
    await waitFor(() => expect(result.current.isLoading).toBe(false));
  });

  it('stores the token and user on successful login', async () => {
    // Arrange
    vi.mocked(authApi.login).mockResolvedValueOnce(MOCK_RESPONSE);

    const { result } = renderHook(() => useLogin(), { wrapper });

    // Act
    await act(() => result.current.submit(VALID_CREDENTIALS));

    // Assert
    expect(storage.setToken).toHaveBeenCalledWith('tok');
    expect(storage.setUser).toHaveBeenCalledWith({
      username: 'johndoe',
      email: 'john@example.com',
    });
  });

  it('navigates to the feed on successful login', async () => {
    // Arrange
    vi.mocked(authApi.login).mockResolvedValueOnce(MOCK_RESPONSE);

    const { result } = renderHook(() => useLogin(), { wrapper });

    // Act
    await act(() => result.current.submit(VALID_CREDENTIALS));

    // Assert
    expect(mockNavigate).toHaveBeenCalledWith(ROUTES.FEED);
  });

  it('sets a user-friendly error message on 401', async () => {
    // Arrange
    vi.mocked(authApi.login).mockRejectedValueOnce(
      new HttpError(401, { Error: 'Invalid email or password.' }),
    );

    const { result } = renderHook(() => useLogin(), { wrapper });

    // Act
    await act(() => result.current.submit(VALID_CREDENTIALS));

    // Assert
    expect(result.current.error).toBe('Invalid email or password.');
    expect(result.current.isLoading).toBe(false);
    expect(mockNavigate).not.toHaveBeenCalled();
  });

  it('sets a fallback error message when the server body has no Error field', async () => {
    // Arrange
    vi.mocked(authApi.login).mockRejectedValueOnce(new HttpError(500, null));

    const { result } = renderHook(() => useLogin(), { wrapper });

    // Act
    await act(() => result.current.submit(VALID_CREDENTIALS));

    // Assert
    expect(result.current.error).toBeTruthy();
    expect(result.current.isLoading).toBe(false);
  });

  it('sets a network error message when fetch throws a non-HttpError', async () => {
    // Arrange
    vi.mocked(authApi.login).mockRejectedValueOnce(new TypeError('Failed to fetch'));

    const { result } = renderHook(() => useLogin(), { wrapper });

    // Act
    await act(() => result.current.submit(VALID_CREDENTIALS));

    // Assert
    expect(result.current.error).toMatch(/network|connect/i);
  });

  it('clears any previous error on a new submission attempt', async () => {
    // Arrange — first call fails, second succeeds
    vi.mocked(authApi.login)
      .mockRejectedValueOnce(new HttpError(401, { Error: 'Bad creds' }))
      .mockResolvedValueOnce(MOCK_RESPONSE);

    const { result } = renderHook(() => useLogin(), { wrapper });

    // Act — first submit (fails)
    await act(() => result.current.submit(VALID_CREDENTIALS));
    expect(result.current.error).not.toBeNull();

    // Act — second submit (succeeds)
    await act(() => result.current.submit(VALID_CREDENTIALS));

    // Assert
    expect(result.current.error).toBeNull();
  });
});
