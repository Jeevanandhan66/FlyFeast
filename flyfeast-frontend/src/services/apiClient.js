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
// Handle errors globally
api.interceptors.response.use(
  (response) => response,
  (error) => {
    // If it's a 401 but not from /Auth/login → force logout
    if (
      error.response?.status === 401 &&
      !error.config?.url.includes("/Auth/login")
    ) {
      localStorage.removeItem(import.meta.env.VITE_TOKEN_STORAGE_KEY);
      window.location.href = "/login";
    }

    return Promise.reject(error);
  }
);



export default api;
