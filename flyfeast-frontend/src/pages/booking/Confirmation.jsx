import { useLocation, useNavigate } from "react-router-dom";

export default function Confirmation() {
  const location = useLocation();
  const navigate = useNavigate();
  const { booking, schedule, seats, passenger, payment } = location.state || {};

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
    <div className="min-h-screen bg-gray-100 py-10 px-4 flex justify-center">
      <div className="bg-white shadow-2xl rounded-2xl w-full max-w-4xl overflow-hidden">
        {/* Header */}
        <div className="bg-green-600 text-white p-6 text-center">
          <h1 className="text-3xl font-bold">🎉 Booking Confirmed !</h1>
          <p className="mt-2 text-lg">
            Reference: <span className="font-mono">{booking.bookingRef}</span>
          </p>
        </div>

        {/* Body */}
        <div className="p-8 space-y-6">
          {/* Booking Summary */}
          <div className="grid md:grid-cols-2 gap-6">
            <div>
              <h2 className="text-lg font-semibold text-gray-800 mb-2">
                Booking Summary
              </h2>
              <p>
                Status:{" "}
                <span className="font-medium text-green-600">
                  {booking.status}
                </span>
              </p>
              <p>
                Total Paid:{" "}
                <span className="font-medium">₹{booking.totalAmount}</span>
              </p>
              <p>
                Booked At:{" "}
                {booking.createdAt
                  ? new Date(booking.createdAt).toLocaleString()
                  : "N/A"}
              </p>
            </div>

            {payment && (
              <div>
                <h2 className="text-lg font-semibold text-gray-800 mb-2">
                  Payment Details
                </h2>
                <p>Provider Ref: {payment.providerRef}</p>
                <p>Provider: {payment.provider}</p>
                <p>Status: {payment.status}</p>
                <p>
                  Paid At:{" "}
                  {payment.createdAt
                    ? new Date(payment.createdAt).toLocaleString()
                    : "N/A"}
                </p>
              </div>
            )}
          </div>

          {/* Flight Info */}
          {schedule && (
            <div className="border-t pt-4">
              <h2 className="text-lg font-semibold text-gray-800 mb-2">
                Flight Details
              </h2>
              <p className="font-medium text-gray-700">
                {schedule.route.originAirport.city} (
                {schedule.route.originAirport.code}) →{" "}
                {schedule.route.destinationAirport.city} (
                {schedule.route.destinationAirport.code})
              </p>
              <p>
                Departure: {new Date(schedule.departureTime).toLocaleString()}
              </p>
              <p>Arrival: {new Date(schedule.arrivalTime).toLocaleString()}</p>
              <p>
                Aircraft: {schedule.route.aircraft.aircraftName} (
                {schedule.route.aircraft.aircraftCode})
              </p>
            </div>
          )}

          {/* Passenger Info */}
          {passenger && (
            <div className="border-t pt-4">
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
            <div className="border-t pt-4">
              <h2 className="text-lg font-semibold text-gray-800 mb-2">
                Seats
              </h2>
              <p>{seats.map((s) => s.seatNumber).join(", ")}</p>
            </div>
          )}
        </div>

        {/* Footer CTA */}
        <div className="bg-gray-50 p-6 text-center">
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
