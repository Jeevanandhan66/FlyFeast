import { useEffect, useState } from "react";
import api from "../../services/apiClient";
import { useAuth } from "../../context/AuthContext";
import { Link } from "react-router-dom";
import { toast } from "react-toastify";

export default function Bookings() {
  const { user } = useAuth();
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);

  const fetchBookings = async () => {
    try {
      const res = await api.get(`/Booking/byuser/${user.userId}`);
      setBookings(res.data);
    } catch (err) {
      toast.error("Failed to load bookings.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (user?.userId) fetchBookings();
  }, [user]);

  const handleCancel = async (id) => {
    if (!window.confirm("Are you sure you want to cancel this booking?"))
      return;
    try {
      await api.put(`/Booking/${id}/cancel`, {
        bookingId: id,
        cancelledById: user.userId,
        reason: "User requested cancellation",
        cancelledAt: new Date().toISOString(),
      });

      toast.success("Booking cancelled successfully.");
      fetchBookings();
    } catch (err) {
      toast.error("Failed to cancel booking.");
    }
  };

  if (loading) return <p className="text-center mt-10">Loading bookings...</p>;

  if (bookings.length === 0)
    return <p className="text-center mt-10">No bookings yet.</p>;

  return (
    <div className="min-h-screen bg-gray-50 py-10 px-6">
      <div className="max-w-6xl mx-auto bg-white shadow-xl rounded-2xl p-8">
        <h1 className="text-3xl font-bold text-blue-600 mb-6">My Bookings</h1>

        <div className="overflow-x-auto">
          <table className="w-full border-collapse rounded-lg overflow-hidden">
            <thead>
              <tr className="bg-blue-100 text-left">
                <th className="p-3 font-semibold text-gray-700">Booking Ref</th>
                <th className="p-3 font-semibold text-gray-700">Status</th>
                <th className="p-3 font-semibold text-gray-700">Total</th>
                <th className="p-3 font-semibold text-gray-700">Date</th>
                <th className="p-3 font-semibold text-gray-700">Actions</th>
              </tr>
            </thead>
            <tbody>
              {bookings.map((b, idx) => (
                <tr
                  key={b.bookingId}
                  className={`${
                    idx % 2 === 0 ? "bg-gray-50" : "bg-white"
                  } border-t hover:bg-blue-50 transition`}
                >
                  <td className="p-3 font-medium text-gray-800">
                    {b.bookingRef}
                  </td>
                  <td
                    className={`p-3 font-semibold ${
                      b.status === "Confirmed"
                        ? "text-green-600"
                        : b.status === "Cancelled"
                        ? "text-red-600"
                        : "text-gray-600"
                    }`}
                  >
                    {b.status}
                  </td>
                  <td className="p-3 text-gray-800">₹{b.totalAmount}</td>
                  <td className="p-3 text-gray-600">
                    {new Date(b.createdAt || b.CreatedAt).toLocaleString()}
                  </td>
                  <td className="p-3 space-x-2">
                    <Link
                      to={`/user/bookings/${b.bookingId}`}
                      className="inline-block px-3 py-1 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 transition"
                    >
                      View
                    </Link>

                    {b.status === "Confirmed" && (
                      <button
                        onClick={() => handleCancel(b.bookingId)}
                        className="inline-block px-3 py-1 text-sm font-medium text-white bg-red-500 rounded-lg hover:bg-red-600 transition"
                      >
                        Cancel
                      </button>
                    )}
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
