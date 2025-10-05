import { useLocation, useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import { getScheduleById } from "../../services/scheduleService";
import { useAuth } from "../../context/AuthContext";
import { toast } from "react-toastify";

export default function Review() {
  const location = useLocation();
  const navigate = useNavigate();
  const { user } = useAuth();

  const { scheduleId, seats } = location.state || {};
  const [schedule, setSchedule] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!scheduleId || !seats) {
      navigate("/");
      return;
    }

    async function loadSchedule() {
      setLoading(true);
      try {
        const data = await getScheduleById(scheduleId);
        setSchedule(data);
      } catch (err) {
        toast.error(err.message);
      } finally {
        setLoading(false);
      }
    }
    loadSchedule();
  }, [scheduleId, seats, navigate]);

  if (loading) {
    return <p className="text-center mt-10">Loading review...</p>;
  }

  if (!schedule) {
    return <p className="text-center mt-10">No schedule found.</p>;
  }

  // calculate fare
  const totalAmount = seats.reduce((sum, s) => sum + (s.price || schedule.route.baseFare), 0);

  const handleProceed = () => {
    navigate("/booking/payment", {
      state: {
        schedule,
        seats,
        user,
        totalAmount,
      },
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
          <p className="text-gray-700 font-semibold">
            {schedule.route.aircraft.aircraftName} ({schedule.route.aircraft.aircraftCode})
          </p>
          <p className="text-gray-600">
            {schedule.route.originAirport.city} ({schedule.route.originAirport.code}) →
            {schedule.route.destinationAirport.city} ({schedule.route.destinationAirport.code})
          </p>
          <p className="text-gray-600">
            Departure: {new Date(schedule.departureTime).toLocaleString()}
          </p>
          <p className="text-gray-600">
            Arrival: {new Date(schedule.arrivalTime).toLocaleString()}
          </p>
          <p className="text-gray-700 mt-2">
            Seats Selected:{" "}
            <span className="font-medium">
              {seats.map((s) => s.seatNumber).join(", ")}
            </span>
          </p>
          <p className="text-lg font-bold text-gray-800 mt-2">
            Total Fare: ₹{totalAmount}
          </p>
        </div>

        {/* Passenger (User) Info */}
        <div className="bg-white shadow-lg rounded-xl p-6">
          <h2 className="text-xl font-semibold text-gray-800 mb-4">
            Passenger Information
          </h2>
          <p className="text-gray-700">
            Name: <span className="font-medium">{user?.fullName}</span>
          </p>
          <p className="text-gray-700">
            Email: <span className="font-medium">{user?.email}</span>
          </p>
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
