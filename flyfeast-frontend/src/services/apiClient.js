import axios from "axios";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
});

// Attach token on each request
api.interceptors.request.use((config) => {
  const token = localStorage.getItem(import.meta.env.VITE_TOKEN_STORAGE_KEY);
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Handle errors globally
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // token expired → force logout (we’ll improve later)
      localStorage.removeItem(import.meta.env.VITE_TOKEN_STORAGE_KEY);
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

export default api;
