import { NavLink, Outlet } from "react-router-dom";
import {
  BarChart3,
  Plane,
  MapPin,
  Route,
  Calendar,
  Users,
  ClipboardList,
  CreditCard,
  RotateCcw,
  Settings,
} from "lucide-react";
import Toaster from "../components/feedback/Toaster";

const navItems = [
  { to: "/admin", label: "Dashboard", icon: <BarChart3 size={18} /> },
  { to: "/admin/Aircraft", label: "Aircraft", icon: <Plane size={18} /> },
  { to: "/admin/Airports", label: "Airports", icon: <MapPin size={18} /> },
  { to: "/admin/routes", label: "Routes", icon: <Route size={18} /> },
  { to: "/admin/schedules", label: "Schedules", icon: <Calendar size={18} /> },
  { to: "/admin/users", label: "Users", icon: <Users size={18} /> },
  { to: "/admin/bookings", label: "Bookings", icon: <ClipboardList size={18} /> },
  { to: "/admin/payments", label: "Payments", icon: <CreditCard size={18} /> },
  { to: "/admin/refunds", label: "Refunds", icon: <RotateCcw size={18} /> },
  { to: "/admin/settings", label: "Settings", icon: <Settings size={18} /> },
];

export default function AdminLayout() {
  return (
    <div className="flex min-h-screen bg-gray-50">
      {/* Sidebar */}
      <aside className="w-64 bg-white shadow-lg flex flex-col">
        <div className="p-6 text-2xl font-bold text-indigo-600">
          FlyFeast Admin
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
                        ? "bg-indigo-100 text-indigo-700"
                        : "text-gray-600 hover:bg-gray-100 hover:text-gray-900"
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
        <div className="p-4 text-xs text-gray-400">© {new Date().getFullYear()} FlyFeast</div>
      </aside>

      {/* Main Content */}
      <div className="flex-1 flex flex-col">
        {/* Topbar */}
        <header className="h-16 bg-white shadow flex items-center justify-between px-6">
          <h1 className="text-lg font-semibold">Admin Console</h1>
          <div className="flex items-center gap-3">
            <span className="text-sm text-gray-600">Admin User</span>
            <img
              src="https://ui-avatars.com/api/?name=Admin&background=6366F1&color=fff"
              alt="avatar"
              className="h-8 w-8 rounded-full"
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
