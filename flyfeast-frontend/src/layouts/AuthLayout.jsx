import { Outlet } from "react-router-dom";
import Toaster from "../components/feedback/Toaster"; // adjust path as needed

export default function AuthLayout() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50">
      {/* Render Login/Register */}
      <Outlet />

      {/* Toast notifications */}
      <Toaster />
    </div>
  );
}
