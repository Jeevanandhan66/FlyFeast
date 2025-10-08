import { useState, useEffect, useRef } from "react";
import api from "../../services/apiClient";

export default function AirportSearch({ label, value, onSelect }) {
  const [query, setQuery] = useState(value || "");
  const [results, setResults] = useState([]);
  const [showDropdown, setShowDropdown] = useState(false);
  const timeoutRef = useRef(null);
  const wrapperRef = useRef(null);

  // Fixed placeholders
  const placeholderExamples = {
    Origin: "Chennai (MAA)",
    Destination: "Bengaluru (BLR)",
  };

  // Fetch airports with debounce
  useEffect(() => {
    if (!query) {
      setResults([]);
      return;
    }

    if (timeoutRef.current) clearTimeout(timeoutRef.current);

    timeoutRef.current = setTimeout(async () => {
      try {
        const res = await api.get(`/Airport?search=${query}`);
        setResults(res.data || []);
        setShowDropdown(true);
      } catch (err) {
        console.error("Airport search failed:", err);
        setResults([]);
      }
    }, 300);

    return () => clearTimeout(timeoutRef.current);
  }, [query]);

  // Hide dropdown if clicked outside
  useEffect(() => {
    const handleClickOutside = (e) => {
      if (wrapperRef.current && !wrapperRef.current.contains(e.target)) {
        setShowDropdown(false);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  const handleSelect = (airport) => {
    setQuery(`${airport.city} (${airport.code})`);
    onSelect(airport);
    setShowDropdown(false);
  };

  return (
    <div className="flex flex-col relative" ref={wrapperRef}>
      <label className="text-sm font-medium text-black">{label}</label>
      <input
        type="text"
        value={query}
        onFocus={() => query && setShowDropdown(true)}
        onChange={(e) => {
          setQuery(e.target.value);
          setShowDropdown(true);
        }}
        placeholder={placeholderExamples[label] || "City (Code)"}
        className="mt-2 w-full border border-gray-300 rounded-xl p-3 
                   focus:ring-2 focus:ring-blue-500 focus:border-blue-500
                   bg-gray-50 text-black placeholder-gray-500 shadow-sm"
      />
      {showDropdown && results.length > 0 && (
        <ul className="absolute top-full left-0 right-0 bg-white border rounded-lg shadow-lg max-h-60 overflow-y-auto z-50">
          {results.map((airport) => (
            <li
              key={airport.airportId}
              onClick={() => handleSelect(airport)}
              className="px-3 py-2 hover:bg-blue-100 cursor-pointer text-sm"
            >
              <span className="font-medium">{airport.city}</span>{" "}
              <span className="text-gray-500">({airport.code})</span>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
