import { useEffect, useState } from "react";
import { useLocation, Link } from "react-router-dom";
import api from "../../services/apiClient";
import { toast } from "react-toastify";

// Helper to parse query params
function useQuery() {
  return new URLSearchParams(useLocation().search);
}

export default function Results() {
  const query = useQuery();
  const origin = query.get("origin") || "";
  const destination = query.get("destination") || "";
  const date = query.get("date") || "";
  const passengers = query.get("passengers") || 1;

  const [flights, setFlights] = useState([]);
  const [loading, setLoading] = useState(true);

  // 👉 Later we’ll replace this with API call
  useEffect(() => {
    setLoading(true);

    // Dummy data for now
    const dummyFlights = [
      {
        id: 1,
        airline: "IndiGo",
        flightNumber: "6E 203",
        origin,
        destination,
        departureTime: "08:30",
        arrivalTime: "11:15",
        duration: "2h 45m",
        price: 4999,
      },
      {
        id: 2,
        airline: "Air India",
        flightNumber: "AI 101",
        origin,
        destination,
        departureTime: "14:00",
        arrivalTime: "16:45",
        duration: "2h 45m",
        price: 5499,
      },
    ];

    setTimeout(() => {
      setFlights(dummyFlights);
      setLoading(false);
    }, 500);
  }, [origin, destination, date, passengers]);

  return (
    <div className="min-h-screen bg-gray-50 py-12 px-6">
      <div className="max-w-5xl mx-auto">
        <h1 className="text-3xl font-bold text-gray-800 mb-6">
          Flights from {origin || "??"} to {destination || "??"}
        </h1>
        <p className="text-gray-600 mb-8">
          Date: {date || "N/A"} | Passengers: {passengers}
        </p>

        {loading ? (
          <p className="text-center text-gray-600">Searching flights...</p>
        ) : flights.length === 0 ? (
          <p className="text-center text-gray-600">No flights found.</p>
        ) : (
          <div className="space-y-6">
            {flights.map((flight) => (
              <div
                key={flight.id}
                className="bg-white shadow-lg rounded-xl p-6 flex flex-col md:flex-row justify-between items-center hover:shadow-xl transition"
              >
                <div className="flex-1">
                  <h2 className="text-xl font-semibold text-blue-600">
                    {flight.airline} ({flight.flightNumber})
                  </h2>
                  <p className="text-gray-700">
                    {flight.origin} → {flight.destination}
                  </p>
                  <p className="text-gray-600 text-sm">
                    {flight.departureTime} - {flight.arrivalTime} ({flight.duration})
                  </p>
                </div>
                <div className="text-right mt-4 md:mt-0">
                  <p className="text-lg font-bold text-gray-800">
                    ₹{flight.price.toLocaleString()}
                  </p>
                  <Link
                    to={`/booking/${flight.id}`}
                    className="inline-block mt-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
                  >
                    Book Now
                  </Link> 
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
