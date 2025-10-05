import api from "./apiClient";

/**
 * Get all seats for a schedule
 * @param {number|string} scheduleId 
 */
export async function getSeatsBySchedule(scheduleId) {
  try {
    const res = await api.get(`/Seat/byschedule/${scheduleId}`);
    return Array.isArray(res.data) ? res.data.map((s) => ({
      seatId: s.seatId,
      seatNumber: s.seatNumber,
      class: s.class,
      price: s.price,
      isBooked: s.isBooked,
    })) : [];
  } catch (error) {
    const message =
      error.response?.data?.message ||
      error.response?.statusText ||
      "Failed to fetch seats.";
    throw new Error(message);
  }
}
