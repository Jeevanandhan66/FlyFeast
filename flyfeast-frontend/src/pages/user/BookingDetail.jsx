import { useEffect, useState, useRef } from "react";
import { useParams, useNavigate } from "react-router-dom";
import api from "../../services/apiClient";
import { toast } from "react-toastify";
import { useReactToPrint } from "react-to-print";

export default function BookingDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [booking, setBooking] = useState(null);
  const [loading, setLoading] = useState(true);

  // ✅ React v3 uses `contentRef` instead of `content`
  const contentRef = useRef();

  const handlePrint = useReactToPrint({
    contentRef, // new API
    documentTitle: booking ? `Ticket-${booking.bookingRef}` : "Ticket",
  });

  useEffect(() => {
    async function fetchBooking() {
      try {
        const res = await api.get(`/Booking/${id}`);
        setBooking(res.data);
      } catch (err) {
        toast.error("Failed to load booking details.");
        navigate("/user/bookings");
      } finally {
        setLoading(false);
      }
    }
    fetchBooking();
  }, [id, navigate]);

  if (loading) return <p className="text-center mt-10">Loading booking...</p>;
  if (!booking) return <p className="text-center mt-10">Booking not found.</p>;

  return (
    <div className="min-h-screen bg-gray-100 py-10 px-4 flex justify-center">
      <div className="w-full max-w-4xl space-y-6">
        {/* Ticket Card */}
        <div
          ref={contentRef}
          className="bg-white shadow-2xl rounded-2xl overflow-hidden border-2 border-blue-600"
        >
          {/* Header */}
          <div className="bg-blue-600 text-white px-6 py-4 flex justify-between items-center">
            <h1 className="text-xl font-bold">FlyFeast ✈️</h1>
            <span className="text-sm">e-Ticket Confirmation</span>
          </div>

          {/* Ticket Content */}
          <div className="p-6 grid grid-cols-2 gap-6 text-gray-800">
            {/* Left Column */}
            <div className="space-y-4">
              <p className="text-sm text-gray-500">Booking Reference</p>
              <p className="text-lg font-bold text-blue-600">
                {booking.bookingRef}
              </p>

              <p className="text-sm text-gray-500">Status</p>
              <p className="font-medium">{booking.status}</p>

              <p className="text-sm text-gray-500">Total Paid</p>
              <p className="font-semibold text-green-600">
                ₹{booking.totalAmount}
              </p>

              <p className="text-sm text-gray-500">Booked At</p>
              <p>
                {booking.createdAt
                  ? new Date(booking.createdAt).toLocaleString("en-IN", {
                      dateStyle: "medium",
                      timeStyle: "short",
                    })
                  : "N/A"}
              </p>
            </div>

            {/* Right Column - Flight Info */}
            <div className="space-y-4">
              <p className="text-sm text-gray-500">Flight</p>
              <p className="font-bold">
                {booking.schedule?.scheduleId} / {booking.schedule?.status}
              </p>

              <p className="text-sm text-gray-500">Departure</p>
              <p className="font-medium">
                {new Date(
                  booking.schedule?.departureTime
                ).toLocaleString("en-IN", {
                  dateStyle: "medium",
                  timeStyle: "short",
                })}
              </p>

              <p className="text-sm text-gray-500">Arrival</p>
              <p className="font-medium">
                {new Date(booking.schedule?.arrivalTime).toLocaleString(
                  "en-IN",
                  {
                    dateStyle: "medium",
                    timeStyle: "short",
                  }
                )}
              </p>

              <p className="text-sm text-gray-500">Duration</p>
              <p>{booking.schedule?.durationFormatted}</p>
            </div>
          </div>

          {/* Passenger + Seats */}
          <div className="bg-gray-50 px-6 py-4 grid grid-cols-2 gap-6 border-t border-dashed border-gray-300">
            {/* Passenger */}
            <div>
              <h2 className="text-lg font-semibold text-gray-700 mb-2">
                Passenger
              </h2>
              {booking.bookingItems?.map((item) => (
                <div key={item.bookingItemId}>
                  <p className="font-medium">
                    {item.passenger?.fullName || "Passenger"}
                  </p>
                  <p className="text-sm text-gray-600">
                    {item.passenger?.passportNumber} ·{" "}
                    {item.passenger?.nationality}
                  </p>
                  <p className="text-sm text-gray-600">
                    {item.passenger?.email || "—"}
                  </p>
                </div>
              ))}
            </div>

            {/* Seats */}
            <div>
              <h2 className="text-lg font-semibold text-gray-700 mb-2">Seats</h2>
              <ul className="list-disc list-inside">
                {booking.bookingItems?.map((item) => (
                  <li key={item.bookingItemId}>
                    Seat {item.seat?.seatNumber} ({item.seat?.class}) — ₹
                    {item.priceAtBooking}
                  </li>
                ))}
              </ul>
            </div>
          </div>

          {/* Footer */}
          <div className="bg-blue-600 text-white px-6 py-2 text-center text-sm">
            Thank you for choosing FlyFeast ✈️ — Have a pleasant journey!
          </div>
        </div>

        {/* Download Button */}
        <div className="text-center">
          <button
            onClick={handlePrint}
            className="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
          >
            Download Ticket
          </button>
        </div>
      </div>
    </div>
  );
}
