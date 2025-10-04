import { Outlet, Link } from "react-router-dom";
import Toaster from "../components/feedback/Toaster";

export default function AppLayout() {
  return (
    <div className="min-h-screen flex flex-col">
      {/* Header with nav */}
      <header className="bg-blue-600 text-white p-4 flex justify-between items-center">
        <h1 className="text-xl font-bold">FlyFeast ✈️</h1>
        <nav className="space-x-4">
          <Link to="/" className="hover:underline">
            Home
          </Link>
          <Link to="/login" className="hover:underline">
            Login
          </Link>
          <Link to="/register" className="hover:underline">
            Register
          </Link>
        </nav>
      </header>

      {/* Page content */}
      <main className="flex-1 p-6 flex justify-center items-center">
        <Outlet />
      </main>

      {/* Footer */}
      <footer className="bg-gray-100 text-center py-2 text-sm text-gray-600">
        © {new Date().getFullYear()} FlyFeast. All rights reserved.
      </footer>

      {/* Toast notifications */}
      <Toaster />
    </div>
  );
}
