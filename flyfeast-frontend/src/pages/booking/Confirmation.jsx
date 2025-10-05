import { useLocation, useNavigate } from "react-router-dom";

export default function Confirmation() {
  const location = useLocation();
  const navigate = useNavigate();

  // booking info from Payment.jsx
  const { flight, seats, passenger, payment } = location.state || {};

  if (!flight || !seats || !passenger) {
    navigate("/");
  }

  // Dummy booking reference
  const bookingRef = "FF" + Math.floor(100000 + Math.random() * 900000);

  const handleMyBookings = () => {
    navigate("/user/bookings");
  };

  return (
    <div className="min-h-screen bg-gray-100 py-10 px-6">
      <div className="max-w-3xl mx-auto bg-white shadow-xl rounded-2xl p-8">
        {/* Header */}
        <div className="text-center border-b pb-6">
          <h1 className="text-3xl font-bold text-blue-600">FlyFeast E-Ticket</h1>
          <p className="text-gray-600 mt-2">Booking Confirmation</p>
          <p className="text-lg font-semibold text-gray-800 mt-2">
            Booking Ref: <span className="text-blue-600">{bookingRef}</span>
          </p>
        </div>

        {/* Flight Details */}
        <div className="mt-6 border-b pb-6">
          <h2 className="text-xl font-semibold text-gray-800 mb-3">
            Flight Details
          </h2>
          <p className="text-gray-700">
            <span className="font-bold">{flight.airline}</span> ({flight.flightNumber})
          </p>
          <p className="text-gray-600">
            {flight.origin} → {flight.destination}
          </p>
          <p className="text-gray-600">
            Departure: {flight.departureTime} | Arrival: {flight.arrivalTime}
          </p>
          <p className="text-gray-600">Duration: {flight.duration}</p>
        </div>

        {/* Passenger Info */}
        <div className="mt-6 border-b pb-6">
          <h2 className="text-xl font-semibold text-gray-800 mb-3">
            Passenger Details
          </h2>
          <p className="text-gray-700">
            Name: <span className="font-medium">{passenger.fullName}</span>
          </p>
          <p className="text-gray-700">Gender: {passenger.gender}</p>
          <p className="text-gray-700">Passport: {passenger.passportNumber}</p>
          <p className="text-gray-700">Nationality: {passenger.nationality}</p>
          <p className="text-gray-700 mt-2">
            Seats: <span className="font-medium">{seats.join(", ")}</span>
          </p>
        </div>

        {/* Payment Info */}
        <div className="mt-6 border-b pb-6">
          <h2 className="text-xl font-semibold text-gray-800 mb-3">
            Payment Summary
          </h2>
          <p className="text-gray-700">
            Paid with Card ending in{" "}
            <span className="font-medium">
              {payment?.cardNumber.slice(-4) || "XXXX"}
            </span>
          </p>
          <p className="text-lg font-bold text-gray-800 mt-2">
            Total Paid: ₹{(flight.price * seats.length).toLocaleString()}
          </p>
          <p className="text-green-600 font-semibold mt-1">Status: Confirmed</p>
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
            onClick={handleMyBookings}
            className="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
          >
            Go to My Bookings
          </button>
        </div>
      </div>
    </div>
  );
}
