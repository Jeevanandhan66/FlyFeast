import api from "./apiClient";

// Get passenger by userId
export async function getPassengerByUser(userId) {
  try {
    const res = await api.get(`/Passenger/user/${userId}`);
    return res.data;
  } catch (error) {
    throw new Error(
      error.response?.data?.message || "Failed to fetch passenger details."
    );
  }
}

// Create booking
export async function createBooking(payload) {
  try {
    const res = await api.post("/Booking", payload);
    return res.data;
  } catch (error) {
    throw new Error(
      error.response?.data?.message || "Failed to create booking."
    );
  }
}
