import { useEffect, useState } from "react";
import { useLocation, Link } from "react-router-dom";
import { searchFlights } from "../../services/searchService";
import { toast } from "react-toastify";
import {
  formatCurrency,
  minutesToDuration,
  shortTimeFromIso,
} from "../../utils/formatters";

function useQuery() {
  return new URLSearchParams(useLocation().search);
}

export default function Results() {
  const query = useQuery();
  const originCity = query.get("originCity") || "";
  const destinationCity = query.get("destinationCity") || "";
  const date = query.get("date") || "";

  const [flights, setFlights] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function run() {
      setLoading(true);
      try {
        const items = await searchFlights({ originCity, destinationCity, date });
        setFlights(items);
      } catch (err) {
        toast.error(err.message || "Failed to fetch flights.");
        setFlights([]);
      } finally {
        setLoading(false);
      }
    }
    run();
  }, [originCity, destinationCity, date]);

  return (
    <div className="min-h-screen bg-gradient-to-b from-blue-50 via-indigo-50 to-blue-100 py-12 px-6">
      <div className="max-w-5xl mx-auto">
        <h1 className="text-4xl font-extrabold bg-gradient-to-r from-blue-600 via-indigo-500 to-purple-500 bg-clip-text text-transparent mb-2 leading-tight pb-1">

          Flights from {originCity || "??"} to {destinationCity || "??"}
        </h1>
        <p className="text-gray-700 mb-8 font-medium tracking-wide">
          Date: {date || "N/A"}
        </p>

        {loading ? (
          <p className="text-center text-gray-600 animate-pulse">
            Searching flights...
          </p>
        ) : flights.length === 0 ? (
          <div className="text-center text-gray-600 bg-gradient-to-r from-blue-100 to-purple-100 rounded-xl p-8 shadow-inner">
            <p>No flights found.</p>
          </div>
        ) : (
          <div className="space-y-6">
            {flights.map((f) => (
              <div
                key={f.id}
                className="relative rounded-2xl overflow-hidden shadow-lg hover:shadow-2xl transition-all duration-300 transform hover:-translate-y-1"
              >
                {/* Gradient Border */}
                <div className="absolute inset-0 bg-gradient-to-r from-blue-500 via-indigo-500 to-purple-500 opacity-80 blur-sm"></div>

                {/* Card Content */}
                <div className="relative bg-white/90 backdrop-blur-sm rounded-2xl p-6 flex flex-col md:flex-row justify-between items-center">
                  <div className="flex-1">
                    <h2 className="text-xl font-semibold text-blue-700">
                      {f.airline} ({f.flightNumber})
                    </h2>
                    <p className="text-gray-700 font-medium">
                      {f.originName || f.originCode} →{" "}
                      {f.destinationName || f.destinationCode}
                    </p>
                    <p className="text-gray-600 text-sm">
                      {shortTimeFromIso(f.departureUtc)} -{" "}
                      {shortTimeFromIso(f.arrivalUtc)} (
                      {minutesToDuration(f.durationMinutes)})
                    </p>
                    <p className="text-gray-500 text-xs mt-1">
                      Aircraft: {f.aircraft}
                    </p>
                  </div>

                  <div className="text-right mt-4 md:mt-0">
                    <p className="text-lg font-bold text-gray-900">
                      {formatCurrency(f.priceAmount, f.currency)}
                    </p>
                    <p className="text-xs text-gray-500">
                      {f.availableSeats != null
                        ? `${f.availableSeats} seats left`
                        : ""}
                    </p>
                    <Link
                      to={`/booking/${f.id}`}
                      className="inline-block mt-3 px-5 py-2 rounded-full bg-gradient-to-r from-blue-600 via-indigo-500 to-purple-600 text-white font-semibold shadow-md hover:opacity-90 transition"
                    >
                      Book Now
                    </Link>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
