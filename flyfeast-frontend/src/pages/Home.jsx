import { useState } from "react";
import { useNavigate } from "react-router-dom";
import heroImage from "../assets/images/hero-bg.jpg";

export default function Home() {
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    origin: "Chennai",
    destination: "Bengaluru",
    date: new Date().toISOString().split("T")[0],
    passengers: 1,
  });

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    // 👉 For now just navigate to /results with query params
    navigate(
  `/results?originCity=${formData.origin}&destinationCity=${formData.destination}&date=${formData.date}`
);

  };

  return (
    <div className="flex flex-col min-h-screen bg-gray-50">
      {/* Hero Section */}
      <section className="relative bg-gradient-to-r from-blue-600 to-indigo-700 text-white">
        <div className="absolute inset-0">
          <img
            src={heroImage}
            alt="Airplane"
            className="w-full h-full object-cover opacity-80"
          />
        </div>
        <div className="relative z-10 flex flex-col items-center justify-center text-center py-20 px-4">
          <h1 className="text-4xl md:text-6xl font-bold mb-4 drop-shadow-lg">
            Fly high. Book fast. Feast easy
          </h1>
          <p className="max-w-2xl text-lg md:text-xl mb-8 text-gray-100">
            Book flights effortlessly, discover popular routes, and enjoy your
            journey.
          </p>
        </div>

        {/* Search Form */}
        <div className="relative z-20 max-w-4xl mx-auto px-4 pb-16">
          <form
            onSubmit={handleSubmit}
            className="bg-white shadow-2xl rounded-3xl p-6 md:p-8 grid grid-cols-1 md:grid-cols-4 gap-4 border-t-8 border-blue-600 relative overflow-hidden text-black"
          >
            {/* Decorative top notch / “ticket” look */}
            <div className="absolute -top-4 left-1/2 transform -translate-x-1/2 w-12 h-6 bg-gray-50 rounded-b-full shadow-md"></div>

            {/* Origin */}
            <div className="flex flex-col">
              <label className="text-sm font-medium text-black">Origin</label>
              <input
                type="text"
                name="origin"
                placeholder="e.g. Chennai"
                value={formData.origin}
                onChange={handleChange}
                className="mt-2 w-full border border-gray-300 rounded-xl p-3 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 bg-gray-50 text-black placeholder-black shadow-sm"
              />
            </div>

            {/* Destination */}
            <div className="flex flex-col">
              <label className="text-sm font-medium text-black">
                Destination
              </label>
              <input
                type="text"
                name="destination"
                placeholder="e.g. Delhi"
                value={formData.destination}
                onChange={handleChange}
                className="mt-2 w-full border border-gray-300 rounded-xl p-3 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 bg-gray-50 text-black placeholder-black shadow-sm"
              />
            </div>

            {/* Date */}
            <div className="flex flex-col">
              <label className="text-sm font-medium text-black">Date</label>
              <input
                type="date"
                name="date"
                value={formData.date}
                onChange={handleChange}
                className="mt-2 w-full border border-gray-300 rounded-xl p-3 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 bg-gray-50 text-black placeholder-black shadow-sm"
              />
            </div>

            {/* Passengers */}
            <div className="flex flex-col">
              <label className="text-sm font-medium text-black">
                Passengers
              </label>
              <input
                type="number"
                name="passengers"
                min="1"
                value={formData.passengers}
                onChange={handleChange}
                className="mt-2 w-full border border-gray-300 rounded-xl p-3 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 bg-gray-50 text-black placeholder-black shadow-sm"
              />
            </div>

            {/* Submit */}
            <div className="md:col-span-4 mt-4">
              <button
                type="submit"
                className="w-full bg-gradient-to-r from-blue-600 to-indigo-600 text-white py-3 rounded-2xl hover:from-blue-700 hover:to-indigo-700 transition shadow-lg"
              >
                Search Flights
              </button>
            </div>
          </form>
        </div>
      </section>

      {/* Popular Routes */}
      <section className="py-16 bg-gray-100">
        <div className="max-w-6xl mx-auto px-6">
          <h2 className="text-3xl font-bold text-center text-gray-800 mb-10">
            Popular Routes
          </h2>
          <div className="grid gap-6 md:grid-cols-3">
            {[
              { from: "Chennai", to: "Delhi", price: "₹4,999" },
              { from: "Mumbai", to: "Bangalore", price: "₹3,499" },
              { from: "Hyderabad", to: "Kolkata", price: "₹5,199" },
            ].map((route, i) => (
              <div
                key={i}
                className="bg-white p-6 rounded-xl shadow hover:shadow-lg transition"
              >
                <h3 className="text-xl font-semibold text-blue-600 mb-2">
                  {route.from} → {route.to}
                </h3>
                <p className="text-gray-700">Starting at {route.price}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Why Choose Us */}
      <section className="py-16 bg-white">
        <div className="max-w-6xl mx-auto px-6 text-center">
          <h2 className="text-3xl font-bold text-gray-800 mb-10">
            Why Choose FlyFeast?
          </h2>
          <div className="grid gap-8 md:grid-cols-3">
            <div className="p-6 bg-gray-50 rounded-xl shadow">
              <h3 className="text-xl font-semibold text-blue-600 mb-2">
                Best Deals
              </h3>
              <p className="text-gray-700">
                Get competitive ticket prices with exclusive offers.
              </p>
            </div>
            <div className="p-6 bg-gray-50 rounded-xl shadow">
              <h3 className="text-xl font-semibold text-blue-600 mb-2">
                Easy Booking
              </h3>
              <p className="text-gray-700">
                Book your flight in just a few clicks with a simple flow.
              </p>
            </div>
            <div className="p-6 bg-gray-50 rounded-xl shadow">
              <h3 className="text-xl font-semibold text-blue-600 mb-2">
                24/7 Support
              </h3>
              <p className="text-gray-700">
                Our support team is here to help anytime, anywhere.
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* Footer
      <footer className="bg-blue-600 text-white text-center py-6 mt-auto">
        <p>© {new Date().getFullYear()} FlyFeast. Alssl rights reserved.</p>
      </footer> */}
    </div>
  );
}
