import { useState, useEffect } from "react";
import { Link } from "react-router-dom";

export default function Bookings() {
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(true);

  // Dummy data for now (later fetch from API)
  useEffect(() => {
    setLoading(true);

    const dummyBookings = [
      {
        id: "FF123456",
        airline: "IndiGo",
        flightNumber: "6E 203",
        origin: "Chennai",
        destination: "Delhi",
        departureTime: "2025-10-10T08:30",
        arrivalTime: "2025-10-10T11:15",
        passenger: "John Doe",
        seat: "12A",
        status: "Confirmed",
        price: 4999,
      },
      {
        id: "FF789012",
        airline: "Air India",
        flightNumber: "AI 101",
        origin: "Mumbai",
        destination: "Bangalore",
        departureTime: "2025-11-05T14:00",
        arrivalTime: "2025-11-05T16:45",
        passenger: "John Doe",
        seat: "7C",
        status: "Cancelled",
        price: 5499,
      },
    ];

    setTimeout(() => {
      setBookings(dummyBookings);
      setLoading(false);
    }, 500);
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen text-gray-600">
        Loading your bookings...
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-10 px-6">
      <div className="max-w-5xl mx-auto">
        <h1 className="text-3xl font-bold text-blue-600 mb-6">My Bookings</h1>

        {bookings.length === 0 ? (
          <p className="text-gray-600">You have no bookings yet.</p>
        ) : (
          <div className="space-y-6">
            {bookings.map((booking) => (
              <div
                key={booking.id}
                className="bg-white shadow-lg rounded-xl p-6 flex flex-col md:flex-row justify-between items-center hover:shadow-xl transition"
              >
                <div>
                  <h2 className="text-lg font-semibold text-gray-800">
                    {booking.airline} ({booking.flightNumber})
                  </h2>
                  <p className="text-gray-600">
                    {booking.origin} → {booking.destination}
                  </p>
                  <p className="text-gray-600">
                    {new Date(booking.departureTime).toLocaleString()} -{" "}
                    {new Date(booking.arrivalTime).toLocaleString()}
                  </p>
                  <p className="text-gray-700">
                    Passenger: <span className="font-medium">{booking.passenger}</span>
                  </p>
                  <p className="text-gray-700">
                    Seat: <span className="font-medium">{booking.seat}</span>
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

                <div className="mt-4 md:mt-0 text-right">
                  <p className="text-lg font-bold text-gray-800">
                    ₹{booking.price.toLocaleString()}
                  </p>
                  {booking.status === "Confirmed" && (
                    <button
                      onClick={() =>
                        alert(`Cancel booking ${booking.id} (API later)`)
                      }
                      className="mt-2 px-4 py-2 bg-red-500 text-white rounded-lg hover:bg-red-600 transition"
                    >
                      Cancel
                    </button>
                  )}
                  <Link
                    to={`/user/bookings/${booking.id}`}
                    className="block mt-2 text-blue-600 hover:underline"
                  >
                    View Details
                  </Link>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
