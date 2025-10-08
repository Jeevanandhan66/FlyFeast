import { Link, NavLink, Outlet, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function UserLayout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  return (
    <div className="min-h-screen flex bg-gray-100">
      {/* Sidebar */}
      <aside className="w-64 bg-white shadow-md hidden md:flex flex-col">
        <div className="p-6 border-b">
          <h1
            className="text-xl font-bold text-blue-600 cursor-pointer"
            onClick={() => navigate("/")}
          >
            FlyFeast
          </h1>
          <p className="text-sm text-gray-500 mt-1">
            Welcome, {user?.fullName || "Guest"}
          </p>
        </div>

        <nav className="flex-1 p-4 space-y-2">
          <NavLink
            to="/user/profile"
            className={({ isActive }) =>
              `block px-4 py-2 rounded-lg ${
                isActive
                  ? "bg-blue-600 text-white"
                  : "text-gray-700 hover:bg-blue-100"
              }`
            }
          >
            👤 Profile
          </NavLink>

          <NavLink
            to="/user/bookings"
            className={({ isActive }) =>
              `block px-4 py-2 rounded-lg ${
                isActive
                  ? "bg-blue-600 text-white"
                  : "text-gray-700 hover:bg-blue-100"
              }`
            }
          >
            📖 My Bookings
          </NavLink>
        </nav>

        <div className="p-4 border-t">
          <button
            onClick={logout}
            className="w-full bg-red-500 text-white py-2 rounded-lg hover:bg-red-600"
          >
            Logout
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <main className="flex-1 flex flex-col">
        {/* Mobile Header */}
        <header className="md:hidden bg-blue-600 text-white p-4 flex justify-between items-center">
          <h1
            className="text-lg font-bold cursor-pointer"
            onClick={() => navigate("/")}
          >
            FlyFeast ✈️
          </h1>
          <div className="space-x-2">
            <Link
              to="/user/profile"
              className="bg-white text-blue-600 px-3 py-1 rounded-lg"
            >
              Profile
            </Link>
            <Link
              to="/user/bookings"
              className="bg-white text-blue-600 px-3 py-1 rounded-lg"
            >
              Bookings
            </Link>
          </div>
        </header>

        {/* Content Area */}
        <div className="p-6 flex-1">
          <Outlet />
        </div>
      </main>
    </div>
  );
}
