import { api } from "./api";

interface LoginRequest {
  email: string;
  password: string;
}

interface AuthResponse {
  email: string;
  token: string;
}

const TOKEN_KEY = "token";

export const authService = {
  async login(data: LoginRequest): Promise<void> {
    const response = await api.post<AuthResponse>("/auth/login", data);
    localStorage.setItem(TOKEN_KEY, response.data.token);
  },

  logout() {
    localStorage.removeItem(TOKEN_KEY);
  },

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  },

  isAuthenticated(): boolean {
    return !!localStorage.getItem(TOKEN_KEY);
  },
};
