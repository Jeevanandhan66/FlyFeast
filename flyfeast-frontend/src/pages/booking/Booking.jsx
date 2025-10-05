import { useParams, useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import { getSeatsBySchedule } from "../../services/seatService";
import { toast } from "react-toastify";

export default function Booking() {
  const { id } = useParams(); // scheduleId
  const navigate = useNavigate();

  const [seats, setSeats] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selectedSeats, setSelectedSeats] = useState([]);
  const [filterClass, setFilterClass] = useState("All");

  useEffect(() => {
    async function loadSeats() {
      setLoading(true);
      try {
        const data = await getSeatsBySchedule(id);
        setSeats(data);
      } catch (err) {
        toast.error(err.message);
      } finally {
        setLoading(false);
      }
    }
    loadSeats();
  }, [id]);

  const toggleSeat = (seat) => {
    if (seat.isBooked) return; // can't select booked seats
    if (selectedSeats.find((s) => s.seatId === seat.seatId)) {
      setSelectedSeats(selectedSeats.filter((s) => s.seatId !== seat.seatId));
    } else {
      setSelectedSeats([...selectedSeats, seat]);
    }
  };

  const handleProceed = () => {
    if (selectedSeats.length === 0) {
      toast.error("Please select at least one seat.");
      return;
    }
    navigate("/booking/review", {
      state: { scheduleId: id, seats: selectedSeats },
    });
  };

  const seatColors = {
    Economy: "bg-green-200 hover:bg-green-300 text-green-900",
    Business: "bg-yellow-200 hover:bg-yellow-300 text-yellow-900",
    First: "bg-purple-200 hover:bg-purple-300 text-purple-900",
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-indigo-50 to-purple-50 py-10 px-6">
      <div className="max-w-5xl mx-auto space-y-10">
        <h1 className="text-4xl font-extrabold bg-gradient-to-r from-blue-600 via-indigo-500 to-purple-500 bg-clip-text text-transparent mb-4 leading-tight pb-1">
          Select Your Seats
        </h1>

        {/* Filter */}
        <div className="flex flex-wrap gap-3 bg-white/60 backdrop-blur-md p-4 rounded-xl shadow-md">
          {["All", "Economy", "Business", "First"].map((cls) => (
            <button
              key={cls}
              onClick={() => setFilterClass(cls)}
              className={`px-5 py-2.5 rounded-lg font-medium transition ${
                filterClass === cls
                  ? "bg-gradient-to-r from-blue-600 to-indigo-600 text-white shadow-md"
                  : "bg-gray-200 hover:bg-gray-300 text-gray-700"
              }`}
            >
              {cls}
            </button>
          ))}
        </div>

        {/* Seat Grid */}
        {loading ? (
          <p className="text-center text-gray-600">Loading seats...</p>
        ) : seats.length === 0 ? (
          <p className="text-center text-gray-600">No seats available.</p>
        ) : (
          <div className="grid grid-cols-6 gap-4 bg-white/70 backdrop-blur-md rounded-xl p-6 shadow-md">
            {seats
              .filter((s) => filterClass === "All" || s.class === filterClass)
              .map((seat) => {
                const isSelected = selectedSeats.find(
                  (s) => s.seatId === seat.seatId
                );
                return (
                  <button
                    key={seat.seatId}
                    onClick={() => toggleSeat(seat)}
                    disabled={seat.isBooked}
                    className={`p-3 rounded-lg font-semibold border transition text-center ${
                      seat.isBooked
                        ? "bg-gray-300 text-gray-500 cursor-not-allowed"
                        : isSelected
                        ? "bg-gradient-to-r from-blue-600 to-indigo-600 text-white shadow-md"
                        : seatColors[seat.class] || "bg-gray-100"
                    }`}
                  >
                    {seat.seatNumber}
                  </button>
                );
              })}
          </div>
        )}

        {/* Proceed */}
        <div className="bg-gradient-to-r from-blue-50 via-indigo-50 to-purple-50 shadow-lg rounded-xl p-6 flex justify-between items-center border border-indigo-100">
          <div>
            <p className="text-gray-700">
              Selected Seats:{" "}
              <span className="font-medium text-indigo-700">
                {selectedSeats.map((s) => s.seatNumber).join(", ") || "None"}
              </span>
            </p>
            <p className="text-lg font-bold text-gray-800">
              Total:{" "}
              <span className="bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">
                ₹{selectedSeats.reduce((sum, s) => sum + (s.price || 0), 0)}
              </span>
            </p>
          </div>
          <button
            onClick={handleProceed}
            className="px-6 py-3 bg-gradient-to-r from-blue-600 to-indigo-600 text-white rounded-lg hover:from-blue-700 hover:to-indigo-700 transition shadow-md"
          >
            Proceed to Review
          </button>
        </div>
      </div>
    </div>
  );
}
