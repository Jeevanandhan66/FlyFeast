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

const stats = [
  { label: "Aircraft", value: 42 },       // from AircraftController
  { label: "Schedules", value: 180 },     // from ScheduleController
  { label: "Bookings", value: 245 },      // from BookingController
  { label: "Revenue", value: "$78,500" }, // from PaymentController
];

const bookingsData = [
  { month: "Jan", bookings: 120 },
  { month: "Feb", bookings: 98 },
  { month: "Mar", bookings: 180 },
  { month: "Apr", bookings: 150 },
  { month: "May", bookings: 200 },
];

const routeDistribution = [
  { name: "NYC-LAX", value: 400 },
  { name: "LHR-DXB", value: 300 },
  { name: "DEL-SIN", value: 200 },
  { name: "SYD-HKG", value: 100 },
];

const COLORS = ["#6366F1", "#10B981", "#F59E0B", "#EF4444"];

const recentBookings = [
  {
    id: "BK001",
    passenger: "John Doe",
    route: "NYC-LAX",
    status: "Confirmed",
  },
  {
    id: "BK002",
    passenger: "Jane Smith",
    route: "LHR-DXB",
    status: "Pending",
  },
  {
    id: "BK003",
    passenger: "Raj Patel",
    route: "DEL-SIN",
    status: "Cancelled",
  },
];

export default function Dashboard() {
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
              <Bar dataKey="bookings" fill="#6366F1" radius={[6, 6, 0, 0]} />
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
                <tr key={b.id} className="border-b">
                  <td className="px-4 py-2">{b.id}</td>
                  <td className="px-4 py-2">{b.passenger}</td>
                  <td className="px-4 py-2">{b.route}</td>
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
