import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function ProtectedRoute({ role }) {
  const { user, loading } = useAuth();

  if (loading) {
    return <div className="flex justify-center items-center h-screen">Loading...</div>;
  }

  if (!user) {
    // Not logged in → go to login
    return <Navigate to="/login" replace />;
  }

  if (role && user.role !== role) {
    // Logged in but not correct role → show unauthorized
    return <Navigate to="/unauthorized" replace />;
  }

  return <Outlet />; // ✅ render child route
}
