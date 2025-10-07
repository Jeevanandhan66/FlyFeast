// src/services/adminService.js
import apiClient from "./apiClient";
import { ENDPOINTS } from "./endpoints";

/** -------------------- AIRCRAFT -------------------- **/

export async function getAircrafts() {
  const { data } = await apiClient.get(ENDPOINTS.AIRCRAFT.BASE);
  return data; // array of AircraftResponseDTO (camelCase)
}

export async function getAircraftById(id) {
  const { data } = await apiClient.get(ENDPOINTS.AIRCRAFT.BY_ID(id));
  return data; // AircraftResponseDTO
}

export async function createAircraft(payload) {
  // payload must be AircraftRequestDTO in camelCase
  const { data } = await apiClient.post(ENDPOINTS.AIRCRAFT.BASE, payload);
  return data; // created AircraftResponseDTO
}

export async function updateAircraft(id, payload) {
  const { data } = await apiClient.put(ENDPOINTS.AIRCRAFT.BY_ID(id), payload);
  return data; // updated AircraftResponseDTO
}

export async function deleteAircraft(id) {
  await apiClient.delete(ENDPOINTS.AIRCRAFT.BY_ID(id));
  return true;
}

/** -------------------- AIRPORT -------------------- **/
export async function getAirports() {
  const { data } = await apiClient.get(ENDPOINTS.AIRPORT.BASE);
  return data; // array of AirportDTO
}

export async function getAirportById(id) {
  const { data } = await apiClient.get(ENDPOINTS.AIRPORT.BY_ID(id));
  return data;
}

export async function createAirport(payload) {
  const { data } = await apiClient.post(ENDPOINTS.AIRPORT.BASE, payload);
  return data;
}

export async function updateAirport(id, payload) {
  const { data } = await apiClient.put(ENDPOINTS.AIRPORT.BY_ID(id), payload);
  return data;
}

export async function deleteAirport(id) {
  await apiClient.delete(ENDPOINTS.AIRPORT.BY_ID(id));
  return true;
}

/** -------------------- ROUTES -------------------- **/
export async function getRoutes() {
  const { data } = await apiClient.get(ENDPOINTS.ROUTES.BASE);
  return data; // array of RouteResponseDTO
}

export async function getRouteById(id) {
  const { data } = await apiClient.get(ENDPOINTS.ROUTES.BY_ID(id));
  return data;
}

export async function createRoute(payload) {
  const { data } = await apiClient.post(ENDPOINTS.ROUTES.BASE, payload);
  return data;
}

export async function updateRoute(id, payload) {
  const { data } = await apiClient.put(ENDPOINTS.ROUTES.BY_ID(id), payload);
  return data;
}

export async function deleteRoute(id) {
  await apiClient.delete(ENDPOINTS.ROUTES.BY_ID(id));
  return true;
}


/** -------------------- SCHEDULES -------------------- **/
export async function getSchedules() {
  const { data } = await apiClient.get(ENDPOINTS.SCHEDULES.BASE);
  return data; // array of ScheduleResponseDTO
}

export async function getScheduleById(id) {
  const { data } = await apiClient.get(ENDPOINTS.SCHEDULES.BY_ID(id));
  return data;
}

export async function createSchedule(payload) {
  const { data } = await apiClient.post(ENDPOINTS.SCHEDULES.BASE, payload);
  return data;
}

export async function updateSchedule(id, payload) {
  const { data } = await apiClient.put(ENDPOINTS.SCHEDULES.BY_ID(id), payload);
  return data;
}

export async function deleteSchedule(id) {
  await apiClient.delete(ENDPOINTS.SCHEDULES.BY_ID(id));
  return true;
}


/** -------------------- BOOKINGS -------------------- **/
export async function getBookings() {
  const { data } = await apiClient.get(ENDPOINTS.BOOKINGS.BASE);
  return data;
}

export async function getBookingById(id) {
  const { data } = await apiClient.get(ENDPOINTS.BOOKINGS.BY_ID(id));
  return data;
}

export async function updateBookingStatus(id, payload) {
  const { data } = await apiClient.put(ENDPOINTS.BOOKINGS.BY_ID(id), payload);
  return data;
}

export async function cancelBooking(id, reason = "Cancelled by admin") {
  const { data } = await apiClient.put(ENDPOINTS.BOOKINGS.CANCEL(id), {
    cancelledById: "admin", // could be current user ID if needed
    reason,
  });
  return data;
}

export async function deleteBooking(id) {
  await apiClient.delete(ENDPOINTS.BOOKINGS.BY_ID(id));
  return true;
}


// ------- USERS -------
export async function getUsers() {
  const res = await apiClient.get("/Admin/users");
  return res.data;
}

export async function createUser(payload) {
  const res = await apiClient.post("/Admin/admin/users", payload);
  return res.data;
}

export async function updateUser(userId, payload) {
  const res = await apiClient.put(`/Admin/users/${userId}`, payload);
  return res.data;
}

export async function deleteUser(userId) {
  const res = await apiClient.delete(`/Admin/users/${userId}`);
  return res.data;
}

export async function toggleUserActive(userId, isActive) {
  const res = await apiClient.put(`/Admin/users/${userId}/activate?isActive=${isActive}`);
  return res.data;
}

export async function assignRole(userId, roleName) {
  const res = await apiClient.post(`/Admin/users/${userId}/roles/${roleName}`);
  return res.data;
}

export async function getRoles() {
  const res = await apiClient.get("/Admin/roles");
  return res.data;
}


//Payments
export async function getPayments() {
  const res = await apiClient.get(ENDPOINTS.PAYMENTS.BASE);
  return res.data;
}

export async function getPaymentById(id) {
  const res = await apiClient.get(ENDPOINTS.PAYMENTS.BY_ID(id));
  return res.data;
}

export async function getPaymentsByBooking(bookingId) {
  const res = await apiClient.get(ENDPOINTS.PAYMENTS.BY_BOOKING(bookingId));
  return res.data;
}

export async function createPayment(payload) {
  const res = await apiClient.post(ENDPOINTS.PAYMENTS.BASE, payload);
  return res.data;
}

export async function updatePayment(id, payload) {
  const res = await apiClient.put(ENDPOINTS.PAYMENTS.BY_ID(id), payload);
  return res.data;
}

export async function deletePayment(id) {
  const res = await apiClient.delete(ENDPOINTS.PAYMENTS.BY_ID(id));
  return res.data;
}

// ---------------- REFUNDS ---------------- 
export async function getRefunds() {
  const { data } = await apiClient.get(ENDPOINTS.REFUNDS.BASE);
  return data;
}

export async function getRefundById(id) {
  const { data } = await apiClient.get(ENDPOINTS.REFUNDS.BY_ID(id));
  return data;
}

export async function createRefund(payload) {
  const { data } = await apiClient.post(ENDPOINTS.REFUNDS.BASE, payload);
  return data;
}

export async function updateRefund(id, payload) {
  const { data } = await apiClient.put(ENDPOINTS.REFUNDS.BY_ID(id), payload);
  return data;
}

export async function deleteRefund(id) {
  return apiClient.delete(ENDPOINTS.REFUNDS.BY_ID(id));
}

