import { authService } from "./authService";
import axios from "axios";

const baseURL = (window as any).env?.REACT_APP_API_URL ?? "";

export const api = axios.create({
  baseURL,
});

api.interceptors.request.use(
  (config) => {
    const token = authService.getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      authService.logout();
      window.location.href = "/login";
    }

    return Promise.reject(error);
  }
);
