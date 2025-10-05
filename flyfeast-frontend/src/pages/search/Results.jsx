import { useEffect, useState } from "react";
import { useLocation, Link } from "react-router-dom";
import { searchFlights } from "../../services/searchService";
import { toast } from "react-toastify";
import { formatCurrency, minutesToDuration, shortTimeFromIso } from "../../utils/formatters";

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
    <div className="min-h-screen bg-gray-50 py-12 px-6">
      <div className="max-w-5xl mx-auto">
        <h1 className="text-3xl font-bold text-gray-800 mb-2">
          Flights from {originCity || "??"} to {destinationCity || "??"}
        </h1>
        <p className="text-gray-600 mb-8">Date: {date || "N/A"}</p>

        {loading ? (
          <p className="text-center text-gray-600">Searching flights...</p>
        ) : flights.length === 0 ? (
          <p className="text-center text-gray-600">No flights found.</p>
        ) : (
          <div className="space-y-6">
            {flights.map((f) => (
              <div
                key={f.id}
                className="bg-white shadow-lg rounded-xl p-6 flex flex-col md:flex-row justify-between items-center hover:shadow-xl transition"
              >
                <div className="flex-1">
                  <h2 className="text-xl font-semibold text-blue-600">
                    {f.airline} ({f.flightNumber})
                  </h2>
                  <p className="text-gray-700">
                    {f.originName || f.originCode} → {f.destinationName || f.destinationCode}
                  </p>
                  <p className="text-gray-600 text-sm">
                    {shortTimeFromIso(f.departureUtc)} - {shortTimeFromIso(f.arrivalUtc)} (
                    {minutesToDuration(f.durationMinutes)})
                  </p>
                  <p className="text-gray-600 text-xs">Aircraft: {f.aircraft}</p>
                </div>
                <div className="text-right mt-4 md:mt-0">
                  <p className="text-lg font-bold text-gray-800">
                    {formatCurrency(f.priceAmount, f.currency)}
                  </p>
                  <p className="text-xs text-gray-500">
                    {f.availableSeats != null ? `${f.availableSeats} seats left` : ""}
                  </p>
                  <Link
                    to={`/booking/${f.id}`}
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
