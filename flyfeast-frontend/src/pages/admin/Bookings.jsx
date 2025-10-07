import { useEffect, useState, useMemo } from "react";
import { useToast } from "../../hooks/useToast";
import {
  getBookings,
  cancelBooking,
  deleteBooking,
  updateBookingStatus,
} from "../../services/adminService";
import { formatCurrency } from "../../utils/formatters";

const STATUS_OPTIONS = ["Pending", "Confirmed", "Cancelled", "Refunded"];

export default function Bookings() {
  const toast = useToast();

  const [list, setList] = useState([]);
  const [loading, setLoading] = useState(true);
  const [err, setErr] = useState("");
  const [filter, setFilter] = useState("");
  const [updatingId, setUpdatingId] = useState(null);

  const filtered = useMemo(() => {
    if (!filter.trim()) return list;
    const f = filter.toLowerCase();
    return list.filter((b) =>
      `${b.bookingRef} ${b.user?.fullName} ${b.schedule?.route?.originAirport?.code} ${b.schedule?.route?.destinationAirport?.code}`
        .toLowerCase()
        .includes(f)
    );
  }, [list, filter]);

  async function load() {
    try {
      setLoading(true);
      setErr("");
      const data = await getBookings();
      setList(Array.isArray(data) ? data : []);
    } catch (e) {
      setErr(
        e?.response?.data?.error || e?.message || "Failed to load bookings."
      );
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    load();
  }, []);

  async function onCancel(id) {
    if (!window.confirm("Cancel this booking?")) return;
    try {
      await cancelBooking(id, "Cancelled by admin");
      toast.success("Booking cancelled successfully!");
      await load();
    } catch (e) {
      const msg =
        e?.response?.data?.error || e?.message || "Failed to cancel booking.";
      setErr(msg);
      toast.error(msg);
    }
  }

  async function onDelete(id) {
    if (!window.confirm("Delete this booking permanently?")) return;
    try {
      await deleteBooking(id);
      toast.success("Booking deleted successfully!");
      await load();
    } catch (e) {
      const msg =
        e?.response?.data?.error || e?.message || "Failed to delete booking.";
      setErr(msg);
      toast.error(msg);
    }
  }

  async function onStatusChange(id, newStatus) {
    setUpdatingId(id);
    try {
      await updateBookingStatus(id, { status: newStatus });
      toast.success(`Booking marked as ${newStatus}`);
      await load();
    } catch (e) {
      const msg =
        e?.response?.data?.error || e?.message || "Failed to update booking.";
      setErr(msg);
      toast.error(msg);
    } finally {
      setUpdatingId(null);
    }
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-xl font-semibold">Bookings</h1>
          <p className="text-sm text-gray-500">
            Manage and review customer bookings.
          </p>
        </div>
        <input
          type="text"
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
          placeholder="Search bookings…"
          className="w-64 border rounded-lg px-3 py-2 text-sm"
        />
      </div>

      {/* Error */}
      {err && <div className="p-3 bg-red-100 text-red-700 rounded">{err}</div>}

      {/* Table */}
      <div className="bg-white rounded-xl shadow">
        {loading ? (
          <div className="p-8 text-center text-gray-500">Loading bookings…</div>
        ) : filtered.length === 0 ? (
          <div className="p-8 text-center text-gray-500">
            No bookings found.
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-gray-100 text-gray-700">
              <tr>
                <th className="px-4 py-2">ID</th>
                <th className="px-4 py-2">Ref</th>
                <th className="px-4 py-2">User</th>
                <th className="px-4 py-2">Route</th>
                <th className="px-4 py-2">Departure</th>
                <th className="px-4 py-2">Amount</th>
                <th className="px-4 py-2">Status</th>
                <th className="px-4 py-2 text-right">Actions</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map((b) => (
                <tr key={b.bookingId} className="border-b">
                  <td className="px-4 py-2">{b.bookingId}</td>
                  <td className="px-4 py-2">{b.bookingRef}</td>
                  <td className="px-4 py-2">{b.user?.fullName}</td>
                  <td className="px-4 py-2">
                    {b.schedule?.route?.originAirport?.code} →{" "}
                    {b.schedule?.route?.destinationAirport?.code}
                  </td>

                  <td className="px-4 py-2">
                    {new Date(b.schedule?.departureTime).toLocaleString()}
                  </td>
                  <td className="px-4 py-2">
                    {formatCurrency(b.totalAmount, "INR")}
                  </td>
                  <td className="px-4 py-2">
                    <select
                      value={b.status}
                      onChange={(e) =>
                        onStatusChange(b.bookingId, e.target.value)
                      }
                      disabled={updatingId === b.bookingId}
                      className="border rounded px-2 py-1 text-sm"
                    >
                      {STATUS_OPTIONS.map((s) => (
                        <option key={s} value={s}>
                          {s}
                        </option>
                      ))}
                    </select>
                  </td>
                  <td className="px-4 py-2 text-right">
                    <button
                      onClick={() => onCancel(b.bookingId)}
                      className="mr-2 border px-3 py-1 rounded hover:bg-gray-50"
                    >
                      Cancel
                    </button>
                    <button
                      onClick={() => onDelete(b.bookingId)}
                      className="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600"
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
}
