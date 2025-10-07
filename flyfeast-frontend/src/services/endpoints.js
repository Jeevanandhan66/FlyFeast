export const ENDPOINTS = {
  
  AIRCRAFT: {
    BASE: "/Aircraft",
    BY_ID: (id) => `/Aircraft/${id}`,
  },

  AIRPORT: {
    BASE: "/Airport",
    BY_ID: (id) => `/Airport/${id}`,
  },

  ROUTES: {
    BASE: "/FlightRoute",
    BY_ID: (id) => `/FlightRoute/${id}`,
  },

  SCHEDULES: {
    BASE: "/Schedule",
    BY_ID: (id) => `/Schedule/${id}`,
  },

  BOOKINGS: {
    BASE: "/Booking",
    BY_ID: (id) => `/Booking/${id}`,
    CANCEL: (id) => `/Booking/${id}/cancel`,
  },

  PAYMENTS: {
    BASE: "/Payment",
    BY_ID: (id) => `/Payment/${id}`,
    BY_BOOKING: (bookingId) => `/Payment/bybooking/${bookingId}`,
  },
};
