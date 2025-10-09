import { useEffect, useState } from "react";
import {
  getAircraftCount,
  getScheduleCount,
  getBookingCount,
  getRevenueTotal,
  getRecentBookings,
  getBookingsPerMonth,
  getTopRoutes,
} from "../../services/adminService";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  PieChart,
  Pie,
  Cell,
  ResponsiveContainer,
} from "recharts";
import { formatCurrency } from "../../utils/formatters";

const COLORS = ["#6366F1", "#10B981", "#F59E0B", "#EF4444"];

export default function Dashboard() {
  const [stats, setStats] = useState([]);
  const [bookingsData, setBookingsData] = useState([]);
  const [routeDistribution, setRouteDistribution] = useState([]);
  const [recentBookings, setRecentBookings] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function loadData() {
      try {
        const [aircraft, schedules, bookings, revenue, recent, perMonth, routes] =
          await Promise.all([
            getAircraftCount(),
            getScheduleCount(),
            getBookingCount(),
            getRevenueTotal(),
            getRecentBookings(),
            getBookingsPerMonth(),
            getTopRoutes(),
          ]);

        setStats([
          { label: "Aircraft", value: aircraft },
          { label: "Schedules", value: schedules },
          { label: "Bookings", value: bookings },
          { label: "Revenue", value: formatCurrency(revenue) },
        ]);
        setBookingsData(perMonth);
        setRouteDistribution(routes);
        setRecentBookings(recent);
      } catch (err) {
        console.error("Failed to load dashboard data", err);
      } finally {
        setLoading(false);
      }
    }

    loadData();
  }, []);

  if (loading) return <p>Loading dashboard...</p>;

  return (
    <div className="space-y-8">
      {/* Stats */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
        {stats.map((s) => (
          <div
            key={s.label}
            className="bg-white rounded-xl shadow p-6 text-center"
          >
            <div className="text-2xl font-bold text-indigo-600">{s.value}</div>
            <div className="text-gray-500">{s.label}</div>
          </div>
        ))}
      </div>

      {/* Charts */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div className="bg-white rounded-xl shadow p-6">
          <h2 className="text-lg font-semibold mb-4">Bookings Per Month</h2>
          <ResponsiveContainer width="100%" height={250}>
            <BarChart data={bookingsData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="month" />
              <YAxis />
              <Tooltip />
              <Bar dataKey="bookings" fill="#ff009dff" radius={[6, 6, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>
        <div className="bg-white rounded-xl shadow p-6">
          <h2 className="text-lg font-semibold mb-4">Top Routes</h2>
          <ResponsiveContainer width="100%" height={250}>
            <PieChart>
              <Pie
                data={routeDistribution}
                dataKey="value"
                nameKey="name"
                outerRadius={80}
                label
              >
                {routeDistribution.map((_, index) => (
                  <Cell
                    key={`cell-${index}`}
                    fill={COLORS[index % COLORS.length]}
                  />
                ))}
              </Pie>
              <Tooltip />
            </PieChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* Recent Bookings */}
      <div className="bg-white rounded-xl shadow p-6">
        <h2 className="text-lg font-semibold mb-4">Recent Bookings</h2>
        <div className="overflow-x-auto">
          <table className="w-full text-sm text-left text-gray-600">
            <thead>
              <tr className="bg-gray-100 text-gray-700">
                <th className="px-4 py-2">Booking ID</th>
                <th className="px-4 py-2">Passenger</th>
                <th className="px-4 py-2">Route</th>
                <th className="px-4 py-2">Status</th>
              </tr>
            </thead>
            <tbody>
              {recentBookings.map((b) => (
                <tr key={b.bookingId} className="border-b">
                  <td className="px-4 py-2">{b.bookingRef}</td>
                  <td className="px-4 py-2">{b.user?.fullName}</td>
                  <td className="px-4 py-2">
                    {b.schedule?.route
                      ? `${b.schedule.route.originAirport?.code} → ${b.schedule.route.destinationAirport?.code}`
                      : "-"}
                  </td>
                  <td className="px-4 py-2">
                    <span
                      className={`px-2 py-1 rounded-full text-xs font-medium ${
                        b.status === "Confirmed"
                          ? "bg-green-100 text-green-700"
                          : b.status === "Pending"
                          ? "bg-yellow-100 text-yellow-700"
                          : "bg-red-100 text-red-700"
                      }`}
                    >
                      {b.status}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
