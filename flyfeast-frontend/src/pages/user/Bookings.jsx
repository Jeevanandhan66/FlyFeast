import { useEffect, useState } from "react";
import api from "../../services/apiClient";
import { useAuth } from "../../context/AuthContext";
import { Link } from "react-router-dom";
import { toast } from "react-toastify";

export default function Bookings() {
  const { user } = useAuth();
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchBookings() {
      try {
        const res = await api.get(`/Booking/byuser/${user.userId}`);
        setBookings(res.data);
      } catch (err) {
        toast.error("Failed to load bookings.");
      } finally {
        setLoading(false);
      }
    }
    if (user?.userId) fetchBookings();
  }, [user]);

  if (loading) return <p className="text-center mt-10">Loading bookings...</p>;

  if (bookings.length === 0)
    return <p className="text-center mt-10">No bookings yet.</p>;

  return (
    <div className="min-h-screen bg-gray-50 py-10 px-6">
      <div className="max-w-5xl mx-auto bg-white shadow-lg rounded-xl p-6">
        <h1 className="text-2xl font-bold text-blue-600 mb-6">My Bookings</h1>
        <table className="w-full border">
          <thead>
            <tr className="bg-gray-100 text-left">
              <th className="p-2">Booking Ref</th>
              <th className="p-2">Status</th>
              <th className="p-2">Total</th>
              <th className="p-2">Date</th>
              <th className="p-2">Action</th>
            </tr>
          </thead>
          <tbody>
            {bookings.map((b) => (
              <tr key={b.bookingId} className="border-t">
                <td className="p-2">{b.bookingRef}</td>
                <td className="p-2">{b.status}</td>
                <td className="p-2">₹{b.totalAmount}</td>
                <td className="p-2">
                  {new Date(b.createdAt || b.CreatedAt).toLocaleString()}
                </td>
                <td className="p-2">
                  <Link
                    to={`/user/bookings/${b.bookingId}`}
                    className="text-blue-600 hover:underline"
                  >
                    View
                  </Link>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
