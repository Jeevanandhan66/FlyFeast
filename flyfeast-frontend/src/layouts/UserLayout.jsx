import { NavLink, Outlet, useNavigate } from "react-router-dom";
import { User, BookOpen, LogOut, Home } from "lucide-react";
import { useAuth } from "../context/AuthContext";
import Toaster from "../components/feedback/Toaster";

const navItems = [
  { to: "/user/profile", label: "My Profile", icon: <User size={18} /> },
  { to: "/user/bookings", label: "My Bookings", icon: <BookOpen size={18} /> },
];

export default function UserLayout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  return (
    <div className="flex min-h-screen bg-gray-100">
      {/* Sidebar */}
      <aside className="w-64 bg-indigo-600 text-white shadow-lg flex flex-col">
        <div className="p-6 text-2xl font-bold cursor-pointer" onClick={() => navigate("/")}>
          FlyFeast
        </div>
        <div className="px-6 pb-4 text-sm text-indigo-200 border-b border-indigo-500">
          Welcome, {user?.fullName || "User"}
        </div>
        <nav className="flex-1 px-4">
          <ul className="space-y-2">
            {navItems.map((item) => (
              <li key={item.to}>
                <NavLink
                  to={item.to}
                  className={({ isActive }) =>
                    `flex items-center gap-2 rounded-lg px-3 py-2 text-sm font-medium transition ${
                      isActive
                        ? "bg-white text-indigo-700"
                        : "text-indigo-100 hover:bg-indigo-500 hover:text-white"
                    }`
                  }
                >
                  {item.icon}
                  {item.label}
                </NavLink>
              </li>
            ))}
          </ul>
        </nav>
        <div className="p-4 border-t border-indigo-500">
          <button
            onClick={logout}
            className="flex items-center justify-center gap-2 w-full bg-red-500 text-white py-2 rounded-lg hover:bg-red-600 transition"
          >
            <LogOut size={16} />
            Logout
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <div className="flex-1 flex flex-col">
        {/* Topbar */}
        <header className="h-16 bg-white shadow flex items-center justify-between px-6">
          <h1 className="text-lg font-semibold text-indigo-700">User Console</h1>
          <div className="flex items-center gap-4">
            {/* Home Button */}
            <button
              onClick={() => navigate("/")}
              className="flex items-center gap-1 rounded-lg bg-indigo-600 px-3 py-2 text-sm font-medium text-white hover:bg-indigo-700 transition"
            >
              <Home size={16} />
              Home
            </button>
            <span className="text-sm text-gray-600">{user?.fullName || "User"}</span>
            <img
              src={`https://ui-avatars.com/api/?name=${encodeURIComponent(
                user?.fullName || "User"
              )}&background=6366F1&color=fff`}
              alt="avatar"
              className="h-8 w-8 rounded-full border border-indigo-300"
            />
          </div>
        </header>

        {/* Routed Content */}
        <main className="flex-1 p-6 bg-gray-50">
          <Outlet />
        </main>
      </div>

      {/* Toast notifications */}
      <Toaster />
    </div>
  );
}
