import api from "./apiClient";

export async function getScheduleById(scheduleId) {
  try {
    const res = await api.get(`/Schedule/${scheduleId}`);
    return res.data;
  } catch (error) {
    const message =
      error.response?.data?.message ||
      error.response?.statusText ||
      "Failed to fetch schedule.";
    throw new Error(message);
  }
}
