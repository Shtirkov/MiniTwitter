export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  username: string;
  email: string;
  token: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface RegisterResponse {
  username: string;
  email: string;
  token: string;
}

export interface AuthUser {
  username: string;
  email: string;
}
