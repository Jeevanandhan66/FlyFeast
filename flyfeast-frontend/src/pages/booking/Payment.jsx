import { useLocation, useNavigate } from "react-router-dom";
import { useState } from "react";

export default function Payment() {
  const location = useLocation();
  const navigate = useNavigate();

  // booking info from Review.jsx
  const { flight, seats, passenger } = location.state || {};

  if (!flight || !seats || !passenger) {
    navigate("/");
  }

  const [paymentData, setPaymentData] = useState({
    cardNumber: "",
    expiry: "",
    cvv: "",
    nameOnCard: "",
  });

  const handleChange = (e) => {
    setPaymentData({ ...paymentData, [e.target.name]: e.target.value });
  };

  const handlePay = (e) => {
    e.preventDefault();

    if (
      !paymentData.cardNumber ||
      !paymentData.expiry ||
      !paymentData.cvv ||
      !paymentData.nameOnCard
    ) {
      alert("Please fill in all payment fields.");
      return;
    }

    // For now simulate success
    navigate("/booking/confirmation", {
      state: { flight, seats, passenger, payment: paymentData },
    });
  };

  return (
    <div className="min-h-screen bg-gray-50 py-10 px-6">
      <div className="max-w-3xl mx-auto space-y-10">
        {/* Booking Summary */}
        <div className="bg-white shadow-lg rounded-xl p-6">
          <h1 className="text-2xl font-bold text-blue-600 mb-4">
            Payment Details
          </h1>
          <p className="text-gray-700">
            {flight.airline} ({flight.flightNumber})
          </p>
          <p className="text-gray-600">
            {flight.origin} → {flight.destination}
          </p>
          <p className="text-gray-600">
            Seats: {seats.join(", ")} | Passenger: {passenger.fullName}
          </p>
          <p className="text-lg font-bold text-gray-800 mt-2">
            Total: ₹{(flight.price * seats.length).toLocaleString()}
          </p>
        </div>

        {/* Payment Form */}
        <div className="bg-white shadow-lg rounded-xl p-6">
          <h2 className="text-xl font-semibold text-gray-800 mb-4">
            Enter Card Information
          </h2>
          <form onSubmit={handlePay} className="space-y-5">
            <div>
              <label className="block text-sm font-medium text-gray-700">
                Card Number
              </label>
              <input
                type="text"
                name="cardNumber"
                value={paymentData.cardNumber}
                onChange={handleChange}
                placeholder="1234 5678 9012 3456"
                className="mt-1 w-full border rounded-lg p-2 focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <div className="grid grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Expiry Date
                </label>
                <input
                  type="text"
                  name="expiry"
                  value={paymentData.expiry}
                  onChange={handleChange}
                  placeholder="MM/YY"
                  className="mt-1 w-full border rounded-lg p-2 focus:ring-2 focus:ring-blue-500"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">
                  CVV
                </label>
                <input
                  type="password"
                  name="cvv"
                  value={paymentData.cvv}
                  onChange={handleChange}
                  placeholder="123"
                  className="mt-1 w-full border rounded-lg p-2 focus:ring-2 focus:ring-blue-500"
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">
                Name on Card
              </label>
              <input
                type="text"
                name="nameOnCard"
                value={paymentData.nameOnCard}
                onChange={handleChange}
                placeholder="John Doe"
                className="mt-1 w-full border rounded-lg p-2 focus:ring-2 focus:ring-blue-500"
              />
            </div>

            <button
              type="submit"
              className="w-full bg-blue-600 text-white py-3 rounded-lg hover:bg-blue-700 transition"
            >
              Pay Now
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}
