import { createContext, useContext, useEffect, useState } from "react";
import { toast } from "react-toastify";

const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  // Restore user on page refresh
  useEffect(() => {
    const token = localStorage.getItem(import.meta.env.VITE_TOKEN_STORAGE_KEY);
    const role = localStorage.getItem("userRole");
    const fullName = localStorage.getItem("userName");
    const email = localStorage.getItem("userEmail");
    const userId = localStorage.getItem("userId");

    if (token && role && fullName) {
      setUser({ token, role, fullName, email, userId });
    }
    setLoading(false);
  }, []);

  // Save user on login
  const login = (data) => {
    const { token, role, fullName, email, userId } = data;
    localStorage.setItem(import.meta.env.VITE_TOKEN_STORAGE_KEY, token);
    localStorage.setItem("userRole", role);
    localStorage.setItem("userName", fullName);
    localStorage.setItem("userEmail", email);
    localStorage.setItem("userId", userId);
    setUser({ token, role, fullName, email, userId });
  };

  // Clear user on logout
  const logout = () => {
    localStorage.removeItem(import.meta.env.VITE_TOKEN_STORAGE_KEY);
    localStorage.removeItem("userRole");
    localStorage.removeItem("userName");
    localStorage.removeItem("userEmail");
    localStorage.removeItem("userId");
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
