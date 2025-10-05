import { useLocation, useNavigate } from "react-router-dom";

export default function Confirmation() {
  const location = useLocation();
  const navigate = useNavigate();
  const { booking, schedule, seats, passenger } = location.state || {};

  if (!booking) {
    return (
      <div className="min-h-screen flex flex-col items-center justify-center">
        <h1 className="text-2xl font-bold text-red-600">
          No booking data found
        </h1>
        <button
          onClick={() => navigate("/")}
          className="mt-4 px-4 py-2 bg-blue-600 text-white rounded-lg"
        >
          Go Home
        </button>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-10 px-6 flex justify-center">
      <div className="bg-white shadow-xl rounded-2xl p-8 w-full max-w-3xl">
        <h1 className="text-3xl font-bold text-green-600 mb-6 text-center">
          🎉 Booking Confirmed!
        </h1>

        {/* Booking Info */}
        <div className="mb-6">
          <p className="text-gray-700 font-semibold">
            Booking Reference:{" "}
            <span className="text-blue-600">{booking.bookingRef}</span>
          </p>
          <p className="text-gray-700">
            Status:{" "}
            <span className="font-medium text-green-600">
              {booking.status}
            </span>
          </p>
          <p className="text-gray-700">
            Total Paid:{" "}
            <span className="font-medium">₹{booking.totalAmount}</span>
          </p>
          <p className="text-gray-700">
            Booked At:{" "}
            {booking.createdAt
              ? new Date(booking.createdAt).toLocaleString()
              : "N/A"}
          </p>
        </div>

        {/* Flight Info */}
        {schedule && (
          <div className="mb-6 border-t pt-4">
            <h2 className="text-lg font-semibold text-gray-800 mb-2">
              Flight Details
            </h2>
            <p>
              {schedule.route.originAirport.city} (
              {schedule.route.originAirport.code}) →{" "}
              {schedule.route.destinationAirport.city} (
              {schedule.route.destinationAirport.code})
            </p>
            <p>
              Departure: {new Date(schedule.departureTime).toLocaleString()}
            </p>
            <p>
              Arrival: {new Date(schedule.arrivalTime).toLocaleString()}
            </p>
            <p>
              Aircraft: {schedule.route.aircraft.aircraftName} (
              {schedule.route.aircraft.aircraftCode})
            </p>
          </div>
        )}

        {/* Passenger Info */}
        {passenger && (
          <div className="mb-6 border-t pt-4">
            <h2 className="text-lg font-semibold text-gray-800 mb-2">
              Passenger Information
            </h2>
            <p>{passenger.fullName}</p>
            <p>{passenger.email}</p>
            <p>
              {passenger.passportNumber} — {passenger.nationality}
            </p>
          </div>
        )}

        {/* Seats */}
        {seats && seats.length > 0 && (
          <div className="mb-6 border-t pt-4">
            <h2 className="text-lg font-semibold text-gray-800 mb-2">Seats</h2>
            <p>{seats.map((s) => s.seatNumber).join(", ")}</p>
          </div>
        )}

        {/* CTA */}
        <div className="text-center">
          <button
            onClick={() => navigate("/user/bookings")}
            className="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
          >
            View My Bookings
          </button>
        </div>
      </div>
    </div>
  );
}
