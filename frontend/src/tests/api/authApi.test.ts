import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { authApi } from '../../api/authApi';
import { HttpError } from '../../api/client';

const MOCK_LOGIN_RESPONSE = {
  username: 'johndoe',
  email: 'john@example.com',
  token: 'mock-jwt-token',
};

function mockFetch(status: number, body: unknown, ok = status >= 200 && status < 300) {
  return vi.fn().mockResolvedValueOnce({
    ok,
    status,
    json: async () => body,
  } as Response);
}

describe('authApi', () => {
  beforeEach(() => {
    vi.stubGlobal('fetch', vi.fn());
    localStorage.clear();
  });

  afterEach(() => {
    vi.unstubAllGlobals();
  });

  describe('login', () => {
    it('calls POST /Auth/login with the provided credentials', async () => {
      vi.stubGlobal('fetch', mockFetch(200, MOCK_LOGIN_RESPONSE));

      await authApi.login({ email: 'john@example.com', password: 'secret' });

      const [url, options] = (fetch as ReturnType<typeof vi.fn>).mock.calls[0] as [
        string,
        RequestInit,
      ];
      expect(url).toContain('/Auth/login');
      expect(options.method).toBe('POST');
      expect(JSON.parse(options.body as string)).toEqual({
        email: 'john@example.com',
        password: 'secret',
      });
    });

    it('returns the parsed login response on success', async () => {
      vi.stubGlobal('fetch', mockFetch(200, MOCK_LOGIN_RESPONSE));

      const result = await authApi.login({ email: 'john@example.com', password: 'secret' });

      expect(result).toEqual(MOCK_LOGIN_RESPONSE);
    });

    it('throws HttpError with status 401 on invalid credentials', async () => {
      vi.stubGlobal('fetch', mockFetch(401, { Error: 'Invalid email or password' }, false));

      await expect(
        authApi.login({ email: 'wrong@example.com', password: 'bad' }),
      ).rejects.toThrow(HttpError);
    });

    it('includes the status code in the thrown HttpError', async () => {
      vi.stubGlobal('fetch', mockFetch(401, { Error: 'Invalid email or password' }, false));

      try {
        await authApi.login({ email: 'wrong@example.com', password: 'bad' });
      } catch (err) {
        expect(err).toBeInstanceOf(HttpError);
        expect((err as HttpError).status).toBe(401);
      }
    });

    it('includes the response body in the thrown HttpError', async () => {
      const errorBody = { Error: 'Invalid email or password.' };
      vi.stubGlobal('fetch', mockFetch(401, errorBody, false));

      try {
        await authApi.login({ email: 'wrong@example.com', password: 'bad' });
      } catch (err) {
        expect((err as HttpError).body).toEqual(errorBody);
      }
    });

    it('attaches the Authorization header when a token is in storage', async () => {
      localStorage.setItem('mt_token', 'stored-token');
      vi.stubGlobal('fetch', mockFetch(200, MOCK_LOGIN_RESPONSE));

      await authApi.login({ email: 'john@example.com', password: 'secret' });

      const [, options] = (fetch as ReturnType<typeof vi.fn>).mock.calls[0] as [
        string,
        RequestInit,
      ];
      expect((options.headers as Record<string, string>)['Authorization']).toBe(
        'Bearer stored-token',
      );
    });
  });
});
