import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";

export default function BookingDetail() {
  const { id } = useParams(); // booking id
  const navigate = useNavigate();
  const [booking, setBooking] = useState(null);
  const [loading, setLoading] = useState(true);

  // Dummy API call (later integrate with backend BookingController)
  useEffect(() => {
    setLoading(true);

    const dummyBooking = {
      id,
      airline: "IndiGo",
      flightNumber: "6E 203",
      origin: "Chennai",
      destination: "Delhi",
      departureTime: "2025-10-10T08:30",
      arrivalTime: "2025-10-10T11:15",
      passenger: "John Doe",
      seat: "12A",
      nationality: "India",
      price: 4999,
      status: "Confirmed",
      cardLast4: "3456",
    };

    setTimeout(() => {
      setBooking(dummyBooking);
      setLoading(false);
    }, 500);
  }, [id]);

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen text-gray-600">
        Loading booking details...
      </div>
    );
  }

  if (!booking) {
    return (
      <div className="flex items-center justify-center min-h-screen text-gray-600">
        Booking not found.
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-100 py-10 px-6">
      <div className="max-w-3xl mx-auto bg-white shadow-xl rounded-2xl p-8">
        {/* Header */}
        <div className="text-center border-b pb-6">
          <h1 className="text-3xl font-bold text-blue-600">Booking Details</h1>
          <p className="text-gray-600 mt-2">
            Booking Ref: <span className="text-blue-600">{booking.id}</span>
          </p>
          <p
            className={`mt-2 font-semibold ${
              booking.status === "Confirmed"
                ? "text-green-600"
                : "text-red-600"
            }`}
          >
            Status: {booking.status}
          </p>
        </div>

        {/* Flight Info */}
        <div className="mt-6 border-b pb-6">
          <h2 className="text-xl font-semibold text-gray-800 mb-3">
            Flight Information
          </h2>
          <p className="text-gray-700">
            <span className="font-bold">{booking.airline}</span> (
            {booking.flightNumber})
          </p>
          <p className="text-gray-600">
            {booking.origin} → {booking.destination}
          </p>
          <p className="text-gray-600">
            Departure: {new Date(booking.departureTime).toLocaleString()}
          </p>
          <p className="text-gray-600">
            Arrival: {new Date(booking.arrivalTime).toLocaleString()}
          </p>
        </div>

        {/* Passenger Info */}
        <div className="mt-6 border-b pb-6">
          <h2 className="text-xl font-semibold text-gray-800 mb-3">
            Passenger Information
          </h2>
          <p className="text-gray-700">
            Name: <span className="font-medium">{booking.passenger}</span>
          </p>
          <p className="text-gray-700">Seat: {booking.seat}</p>
          <p className="text-gray-700">Nationality: {booking.nationality}</p>
        </div>

        {/* Payment Info */}
        <div className="mt-6 border-b pb-6">
          <h2 className="text-xl font-semibold text-gray-800 mb-3">
            Payment Information
          </h2>
          <p className="text-gray-700">
            Paid with Card ending in{" "}
            <span className="font-medium">{booking.cardLast4}</span>
          </p>
          <p className="text-lg font-bold text-gray-800 mt-2">
            Fare Paid: ₹{booking.price.toLocaleString()}
          </p>
        </div>

        {/* Actions */}
        <div className="mt-8 flex justify-between">
          <button
            onClick={() => window.print()}
            className="px-6 py-3 bg-gray-200 rounded-lg hover:bg-gray-300 transition"
          >
            Download Ticket
          </button>
          <button
            onClick={() => navigate("/user/bookings")}
            className="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
          >
            Back to My Bookings
          </button>
        </div>
      </div>
    </div>
  );
}
