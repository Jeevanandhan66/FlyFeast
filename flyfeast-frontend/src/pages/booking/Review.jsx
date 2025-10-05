import { useLocation, useNavigate } from "react-router-dom";
import { useState } from "react";

export default function Review() {
  const location = useLocation();
  const navigate = useNavigate();

  // Flight + seats come from state passed in Booking.jsx
  const { flight, seats } = location.state || {};

  // If user comes directly without data, redirect to home
  if (!flight || !seats) {
    navigate("/");
  }

  const [passenger, setPassenger] = useState({
    fullName: "",
    gender: "",
    passportNumber: "",
    nationality: "",
  });

  const handleChange = (e) => {
    setPassenger({ ...passenger, [e.target.name]: e.target.value });
  };

  const handleProceed = () => {
    if (!passenger.fullName || !passenger.gender || !passenger.passportNumber) {
      alert("Please fill in all required fields.");
      return;
    }

    navigate("/booking/payment", {
      state: { flight, seats, passenger },
    });
  };

  return (
    <div className="min-h-screen bg-gray-50 py-10 px-6">
      <div className="max-w-4xl mx-auto space-y-10">
        {/* Flight Summary */}
        <div className="bg-white shadow-lg rounded-xl p-6">
          <h1 className="text-2xl font-bold text-blue-600 mb-4">
            Review Your Booking
          </h1>
          <p className="text-gray-700">
            {flight.airline} ({flight.flightNumber})
          </p>
          <p className="text-gray-600">
            {flight.origin} → {flight.destination}
          </p>
          <p className="text-gray-600">
            {flight.departureTime} - {flight.arrivalTime} ({flight.duration})
          </p>
          <p className="mt-2 text-gray-700">
            Seats Selected: <span className="font-semibold">{seats.join(", ")}</span>
          </p>
          <p className="text-lg font-bold text-gray-800 mt-2">
            Total Fare: ₹{(flight.price * seats.length).toLocaleString()}
          </p>
        </div>

        {/* Passenger Details */}
        <div className="bg-white shadow-lg rounded-xl p-6">
          <h2 className="text-xl font-semibold text-gray-800 mb-4">
            Passenger Details
          </h2>
          <form className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label className="block text-sm font-medium text-gray-700">
                Full Name *
              </label>
              <input
                type="text"
                name="fullName"
                value={passenger.fullName}
                onChange={handleChange}
                className="mt-1 w-full border rounded-lg p-2 focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">
                Gender *
              </label>
              <select
                name="gender"
                value={passenger.gender}
                onChange={handleChange}
                className="mt-1 w-full border rounded-lg p-2 focus:ring-2 focus:ring-blue-500"
              >
                <option value="">Select</option>
                <option value="Male">Male</option>
                <option value="Female">Female</option>
                <option value="Other">Other</option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">
                Passport Number *
              </label>
              <input
                type="text"
                name="passportNumber"
                value={passenger.passportNumber}
                onChange={handleChange}
                className="mt-1 w-full border rounded-lg p-2 focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">
                Nationality
              </label>
              <input
                type="text"
                name="nationality"
                value={passenger.nationality}
                onChange={handleChange}
                className="mt-1 w-full border rounded-lg p-2 focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </form>
        </div>

        {/* Proceed Button */}
        <div className="flex justify-end">
          <button
            onClick={handleProceed}
            className="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
          >
            Proceed to Payment
          </button>
        </div>
      </div>
    </div>
  );
}
