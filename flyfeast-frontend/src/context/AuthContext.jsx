import { createContext, useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";

const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem(import.meta.env.VITE_TOKEN_STORAGE_KEY);
    const role = localStorage.getItem("userRole");
    const fullName = localStorage.getItem("userName");

    if (token && role && fullName) {
      setUser({ token, role, fullName });
    }
    setLoading(false);
  }, []);

  const login = (data) => {
    const { token, role, fullName } = data;
    localStorage.setItem(import.meta.env.VITE_TOKEN_STORAGE_KEY, token);
    localStorage.setItem("userRole", role);
    localStorage.setItem("userName", fullName);
    setUser({ token, role, fullName });
  };

  const logout = () => {
    localStorage.removeItem(import.meta.env.VITE_TOKEN_STORAGE_KEY);
    localStorage.removeItem("userRole");
    localStorage.removeItem("userName");
    setUser(null);
    toast.info("Logged out successfully.");
    window.location.href = "/login";
  };

  return (
    <AuthContext.Provider value={{ user, login, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}
