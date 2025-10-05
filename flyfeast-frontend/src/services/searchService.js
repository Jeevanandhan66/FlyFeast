import api from "./apiClient";

export async function searchFlights({ originCity, destinationCity, date }) {
  try {
    const res = await api.get("/Search", {
      params: { originCity, destinationCity, date },
    });

    // Direct array returned from backend
    const items = Array.isArray(res.data) ? res.data : [];

    return items.map((it) => ({
      id: it.scheduleId,
      airline: it.route?.aircraft?.aircraftName || "Unknown Airline",
      flightNumber: it.route?.aircraft?.aircraftCode || "N/A",
      originCode: it.route?.originAirport?.code,
      originName: it.route?.originAirport?.city,
      destinationCode: it.route?.destinationAirport?.code,
      destinationName: it.route?.destinationAirport?.city,
      departureUtc: it.departureTime,
      arrivalUtc: it.arrivalTime,
      durationMinutes: it.durationMinutes,
      priceAmount: it.route?.baseFare,
      currency: "INR",
      availableSeats: it.availableSeats,
      aircraft: it.route?.aircraft?.aircraftName,
      status: it.status,
    }));
  } catch (error) {
    const message =
      error.response?.data?.message ||
      error.response?.statusText ||
      "Failed to search flights.";
    throw new Error(message);
  }
}
