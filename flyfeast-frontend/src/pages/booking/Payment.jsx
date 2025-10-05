import { useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import { useState, useEffect } from "react";
import { getPassengerByUser, createBooking } from "../../services/bookingService";
import { toast } from "react-toastify";

export default function Payment() {
  const location = useLocation();
  const navigate = useNavigate();
  const { user } = useAuth();

  const { schedule, seats, totalAmount } = location.state || {};

  const [passenger, setPassenger] = useState(null);
  const [loading, setLoading] = useState(true);
  const [card, setCard] = useState({ number: "", expiry: "", cvv: "" });
  const [processing, setProcessing] = useState(false);

  useEffect(() => {
    if (!user?.token || !user?.role || !schedule || !seats) {
      navigate("/");
      return;
    }

    async function fetchPassenger() {
      try {
        const data = await getPassengerByUser(user.userId);
        setPassenger(data);
      } catch (err) {
        toast.error(err.message);
      } finally {
        setLoading(false);
      }
    }

    fetchPassenger();
  }, [user, schedule, seats, navigate]);

  const handlePay = async (e) => {
    e.preventDefault();
    if (!card.number || !card.expiry || !card.cvv) {
      toast.error("Please fill in all payment details.");
      return;
    }

    if (!passenger) {
      toast.error("Passenger details not found.");
      return;
    }

    setProcessing(true);
    try {
      const payload = {
        userId: user.userId,
        scheduleId: schedule.scheduleId,
        totalAmount,
        seats: seats.map((s) => ({
          seatId: s.seatId,
          passengerId: passenger.passengerId,
        })),
        status: "Confirmed", // you may later extend with "Pending/Confirmed"
      };

      const booking = await createBooking(payload);

      toast.success("Payment successful! Booking confirmed.");
      navigate("/booking/confirmation", {
        state: { booking, schedule, seats, passenger },
      });
    } catch (err) {
      toast.error(err.message);
    } finally {
      setProcessing(false);
    }
  };

  if (loading) {
    return <p className="text-center mt-10">Loading payment page...</p>;
  }

  return (
    <div className="min-h-screen bg-gray-50 py-10 px-6">
      <div className="max-w-3xl mx-auto bg-white shadow-xl rounded-2xl p-8">
        <h1 className="text-2xl font-bold text-blue-600 mb-6">Payment</h1>

        {/* Booking Summary */}
        <div className="mb-6">
          <p className="font-semibold text-gray-700">
            {schedule.route.originAirport.city} ({schedule.route.originAirport.code}) →
            {schedule.route.destinationAirport.city} ({schedule.route.destinationAirport.code})
          </p>
          <p className="text-gray-600">
            Departure: {new Date(schedule.departureTime).toLocaleString()}
          </p>
          <p className="text-gray-600">
            Seats: {seats.map((s) => s.seatNumber).join(", ")}
          </p>
          <p className="text-lg font-bold text-gray-800 mt-2">
            Total: ₹{totalAmount}
          </p>
        </div>

        {/* Passenger Info */}
        {passenger && (
          <div className="mb-6">
            <h2 className="text-lg font-semibold text-gray-800 mb-2">
              Passenger
            </h2>
            <p className="text-gray-700">{passenger.fullName}</p>
            <p className="text-gray-600">{passenger.email}</p>
            <p className="text-gray-600">Passport: {passenger.passportNumber}</p>
          </div>
        )}

        {/* Payment Form */}
        <form onSubmit={handlePay} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700">
              Card Number
            </label>
            <input
              type="text"
              value={card.number}
              onChange={(e) => setCard({ ...card, number: e.target.value })}
              className="mt-1 w-full border rounded-lg p-2 focus:ring-2 focus:ring-blue-500 focus:outline-none"
              placeholder="1234 5678 9012 3456"
            />
          </div>
          <div className="flex space-x-4">
            <div className="flex-1">
              <label className="block text-sm font-medium text-gray-700">
                Expiry
              </label>
              <input
                type="text"
                value={card.expiry}
                onChange={(e) => setCard({ ...card, expiry: e.target.value })}
                className="mt-1 w-full border rounded-lg p-2 focus:ring-2 focus:ring-blue-500 focus:outline-none"
                placeholder="MM/YY"
              />
            </div>
            <div className="flex-1">
              <label className="block text-sm font-medium text-gray-700">
                CVV
              </label>
              <input
                type="password"
                value={card.cvv}
                onChange={(e) => setCard({ ...card, cvv: e.target.value })}
                className="mt-1 w-full border rounded-lg p-2 focus:ring-2 focus:ring-blue-500 focus:outline-none"
                placeholder="***"
              />
            </div>
          </div>
          <button
            type="submit"
            disabled={processing}
            className="w-full bg-blue-600 text-white py-3 rounded-lg hover:bg-blue-700 transition disabled:opacity-50"
          >
            {processing ? "Processing..." : "Pay Now"}
          </button>
        </form>
      </div>
    </div>
  );
}
