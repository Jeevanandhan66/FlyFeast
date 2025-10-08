import { useEffect, useState, useRef } from "react";
import { useParams, useNavigate } from "react-router-dom";
import api from "../../services/apiClient";
import { toast } from "react-toastify";
import { useReactToPrint } from "react-to-print";
import { formatIST } from "../../utils/formatters";
import { QRCodeCanvas } from "qrcode.react";


export default function BookingDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [booking, setBooking] = useState(null);
  const [loading, setLoading] = useState(true);

  const contentRef = useRef();

  const handlePrint = useReactToPrint({
    contentRef,
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

  const origin = booking.schedule?.route?.originAirport;
  const destination = booking.schedule?.route?.destinationAirport;

  return (
    <div className="min-h-screen bg-gray-100 py-10 px-4 flex justify-center">
      <div className="w-full max-w-4xl space-y-6">
        {/* Ticket Card */}
        <div
          ref={contentRef}
          className="bg-white shadow-xl rounded-lg border border-gray-300 overflow-hidden"
        >
          {/* Header */}
          <div className="bg-blue-700 text-white px-6 py-4 flex justify-between items-center">
            <h1 className="text-2xl font-bold">FlyFeast Airlines</h1>
            <div className="text-right">
              <p className="text-sm">Booking Ref</p>
              <p className="text-2xl font-bold tracking-widest">
                {booking.bookingRef}
              </p>
            </div>
          </div>

          {/* Main Ticket */}
          <div className="grid grid-cols-3 gap-6 p-6">
            {/* From - To */}
            <div className="col-span-2 flex justify-between items-center">
              <div>
                <p className="text-sm text-gray-500">From</p>
                <p className="text-4xl font-bold text-blue-700">
                  {origin?.code}
                </p>
                <p className="text-gray-600">{origin?.city}</p>
              </div>
              <div className="text-center text-3xl font-bold text-gray-400">→</div>
              <div>
                <p className="text-sm text-gray-500">To</p>
                <p className="text-4xl font-bold text-blue-700">
                  {destination?.code}
                </p>
                <p className="text-gray-600">{destination?.city}</p>
              </div>
            </div>

            {/* QR Code */}
            <div className="flex justify-center items-center">
              <QRCodeCanvas value={booking.bookingRef} size={100} />
            </div>
          </div>

          {/* Flight Details */}
          <div className="px-6 pb-4 grid grid-cols-2 md:grid-cols-4 gap-4 text-sm border-t border-gray-200">
            <div>
              <p className="text-gray-500">Departure</p>
              <p className="font-semibold">
                {new Date(booking.schedule?.departureTime).toLocaleString(
                  "en-IN",
                  { dateStyle: "medium", timeStyle: "short" }
                )}
              </p>
            </div>
            <div>
              <p className="text-gray-500">Arrival</p>
              <p className="font-semibold">
                {new Date(booking.schedule?.arrivalTime).toLocaleString(
                  "en-IN",
                  { dateStyle: "medium", timeStyle: "short" }
                )}
              </p>
            </div>
            <div>
              <p className="text-gray-500">Duration</p>
              <p className="font-semibold">
                {booking.schedule?.durationFormatted}
              </p>
            </div>
            <div>
              <p className="text-gray-500">Aircraft</p>
              <p className="font-semibold">
                {booking.schedule?.route?.aircraft?.aircraftCode}
              </p>
            </div>
          </div>

          {/* Passenger & Seats */}
          <div className="px-6 py-4 border-t border-gray-200 grid grid-cols-2 gap-6">
            <div>
              <h2 className="text-lg font-semibold mb-2">Passenger(s)</h2>
              {[...new Map(
                booking.bookingItems.map((i) => [i.passenger?.passengerId, i.passenger])
              ).values()].map((p) => (
                <div key={p.passengerId} className="mb-2">
                  <p className="font-medium">{p.fullName}</p>
                  <p className="text-sm text-gray-600">
                    {p.passportNumber} · {p.nationality}
                  </p>
                  <p className="text-sm text-gray-600">{p.email}</p>
                </div>
              ))}
            </div>

            <div>
              <h2 className="text-lg font-semibold mb-2">Seats</h2>
              <ul className="space-y-1">
                {booking.bookingItems?.map((item) => (
                  <li key={item.bookingItemId} className="text-sm">
                    {item.seat?.seatNumber} ({item.seat?.class}) — ₹
                    {item.priceAtBooking}
                  </li>
                ))}
              </ul>
            </div>
          </div>

          {/* Footer */}
          <div className="bg-blue-700 text-white px-6 py-2 text-center text-sm">
            Thank you for flying with FlyFeast Airlines. Have a pleasant journey!
          </div>
        </div>

        {/* Download Button */}
        <div className="text-center">
          <button
            onClick={handlePrint}
            className="px-6 py-3 bg-blue-700 text-white rounded-lg hover:bg-blue-800 transition"
          >
            Download Ticket
          </button>
        </div>
      </div>
    </div>
  );
}
