import { useParams, useNavigate } from "react-router-dom";
import { useState } from "react";

export default function Booking() {
  const { id } = useParams(); // flight id from URL
  const navigate = useNavigate();

  // Dummy flight details
  const flight = {
    id,
    airline: "IndiGo",
    flightNumber: "6E 203",
    origin: "Chennai",
    destination: "Delhi",
    departureTime: "08:30",
    arrivalTime: "11:15",
    duration: "2h 45m",
    price: 4999,
  };

  // Dummy seats (6x4 grid = 24 seats)
  const totalSeats = 24;
  const [selectedSeats, setSelectedSeats] = useState([]);

  const toggleSeat = (seat) => {
    if (selectedSeats.includes(seat)) {
      setSelectedSeats(selectedSeats.filter((s) => s !== seat));
    } else {
      setSelectedSeats([...selectedSeats, seat]);
    }
  };

  const handleProceed = () => {
    if (selectedSeats.length === 0) {
      alert("Please select at least one seat.");
      return;
    }
    navigate(`/booking/review`, {
      state: { flight, seats: selectedSeats },
    });
  };

  return (
    <div className="min-h-screen bg-gray-50 py-10 px-6">
      <div className="max-w-5xl mx-auto space-y-10">
        {/* Flight Info */}
        <div className="bg-white shadow-lg rounded-xl p-6">
          <h1 className="text-2xl font-bold text-blue-600 mb-4">
            {flight.airline} ({flight.flightNumber})
          </h1>
          <p className="text-gray-700">
            {flight.origin} → {flight.destination}
          </p>
          <p className="text-gray-600">
            {flight.departureTime} - {flight.arrivalTime} ({flight.duration})
          </p>
          <p className="text-gray-800 font-semibold mt-2">
            Base Fare: ₹{flight.price.toLocaleString()}
          </p>
        </div>

        {/* Seat Selection */}
        <div className="bg-white shadow-lg rounded-xl p-6">
          <h2 className="text-xl font-semibold text-gray-800 mb-4">
            Select Your Seats
          </h2>
          <div className="grid grid-cols-4 gap-4 max-w-md mx-auto">
            {[...Array(totalSeats)].map((_, i) => {
              const seat = i + 1;
              const isSelected = selectedSeats.includes(seat);
              return (
                <button
                  key={seat}
                  type="button"
                  onClick={() => toggleSeat(seat)}
                  className={`p-4 rounded-lg text-sm font-semibold ${
                    isSelected
                      ? "bg-blue-600 text-white"
                      : "bg-gray-100 text-gray-700 hover:bg-blue-100"
                  }`}
                >
                  {seat}
                </button>
              );
            })}
          </div>
        </div>

        {/* Price & Proceed */}
        <div className="bg-white shadow-lg rounded-xl p-6 flex justify-between items-center">
          <div>
            <p className="text-gray-600">Selected Seats: {selectedSeats.join(", ") || "None"}</p>
            <p className="text-lg font-bold text-gray-800">
              Total: ₹{(flight.price * selectedSeats.length).toLocaleString()}
            </p>
          </div>
          <button
            onClick={handleProceed}
            className="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
          >
            Proceed to Review
          </button>
        </div>
      </div>
    </div>
  );
}
