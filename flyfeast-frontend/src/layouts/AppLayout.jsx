import { Outlet, Link, useLocation } from "react-router-dom";
import Toaster from "../components/feedback/Toaster";
import { useAuth } from "../context/AuthContext";

export default function AppLayout() {
  const { user, logout } = useAuth();

  return (
    <div className="flex flex-col min-h-screen bg-gray-50">
      {/* Header / Navbar */}
      <header className="sticky top-0 z-50 bg-white shadow-md">
        <div className="max-w-7xl mx-auto px-6 py-4 flex justify-between items-center">
          {/* Logo */}
          <Link
            to="/"
            className="text-2xl font-bold text-blue-600 hover:text-blue-700 transition"
          >
            FlyFeast
          </Link>

          {/* Navigation */}
          <nav className="space-x-6 flex items-center text-sm font-medium">
            <Link
              to="/"
              className="text-gray-700 hover:text-blue-600 transition"
            >
              Home
            </Link>

            {user ? (
              <>
                {/* Show Dashboard link only for admins */}
                {user.role === "Admin" && (
                  <Link
                    to="/admin"
                    className="text-gray-700 hover:text-blue-600 transition"
                  >
                    Dashboard
                  </Link>
                )}

                <Link
                  to="/user/profile"
                  className="text-gray-700 hover:text-blue-600 transition"
                >
                  Profile
                </Link>

                <span className="hidden sm:inline text-gray-800 font-semibold">
                  Welcome, {user.fullName}
                </span>

                <button
                  onClick={logout}
                  className="ml-4 bg-red-500 text-white px-4 py-2 rounded-lg hover:bg-red-600 transition"
                >
                  Logout
                </button>
              </>
            ) : (
              <>
                <Link
                  to="/login"
                  state={{ background: window.location.pathname }}
                  className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
                >
                  Login
                </Link>
                <Link
                  to="/register"
                  state={{ background: window.location.pathname }}
                  className="px-4 py-2 border border-blue-600 text-blue-600 rounded-lg hover:bg-blue-600 hover:text-white transition"
                >
                  Register
                </Link>
              </>
            )}
          </nav>
        </div>
      </header>

      {/* Page content */}
      <main className="flex-1 w-full">
        <Outlet />
      </main>

      {/* Footer */}
      <footer className="bg-blue-600 text-white py-6 mt-auto">
        <div className="max-w-7xl mx-auto px-6 flex flex-col md:flex-row justify-between items-center">
          <p className="text-sm">
            © {new Date().getFullYear()} FlyFeast. All rights reserved.
          </p>
          <nav className="space-x-4 mt-2 md:mt-0 text-sm">
            <Link to="/" className="hover:underline">
              Home
            </Link>
            <Link to="/about" className="hover:underline">
              About
            </Link>
            <Link to="/contact" className="hover:underline">
              Contact
            </Link>
          </nav>
        </div>
      </footer>

      {/* Toast notifications */}
      <Toaster />
    </div>
  );
}
