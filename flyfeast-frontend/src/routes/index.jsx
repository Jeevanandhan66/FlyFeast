import { createBrowserRouter } from "react-router-dom";

// Layouts
import AppLayout from "../layouts/AppLayout";
import AuthLayout from "../layouts/AuthLayout";
import AdminLayout from "../layouts/AdminLayout";

// Pages
import Home from "../pages/Home";
import Login from "../pages/auth/Login";
import Register from "../pages/auth/Register";
import Results from "../pages/search/Results";
import Booking from "../pages/booking/Booking";
import Review from "../pages/booking/Review";
import Payment from "../pages/booking/Payment";
import Confirmation from "../pages/booking/Confirmation";
import Bookings from "../pages/user/Bookings";
import BookingDetail from "../pages/user/BookingDetail";

import NotFound from "../pages/errors/NotFound";
import Unauthorized from "../pages/errors/Unauthorized";
// import Dashboard from "../pages/admin/Dashboard";
// import ProtectedRoute from "./ProtectedRoute";

const router = createBrowserRouter([
  // Public/User routes
  {
    path: "/",
    element: <AppLayout />,
    errorElement: <NotFound />,
    children: [
      { index: true, element: <Home /> },
      { path: "results", element: <Results /> },
      { path: "booking/:id", element: <Booking /> },
      { path: "booking/review", element: <Review /> },
      { path: "booking/payment", element: <Payment /> },
      { path: "booking/confirmation", element: <Confirmation /> },
      { path: "user/bookings", element: <Bookings /> },
      { path: "user/bookings/:id", element: <BookingDetail /> },
      
    ],
  },

  // Auth routes
  {
    path: "/",
    element: <AuthLayout />,
    children: [
      { path: "login", element: <Login /> },
      { path: "register", element: <Register /> },
    ],
  },

  // Admin protected routes
  // {
  //   path: "/admin",
  //   element: <ProtectedRoute role="Admin" />,
  //   children: [
  //     {
  //       element: <AdminLayout />,
  //       children: [
  //         { index: true, element: <Dashboard /> },
  //         // later add: /admin/flights, /admin/schedules, etc.
  //       ],
  //     },
  //   ],
  // },

  // Unauthorized
  {
    path: "/unauthorized",
    element: <Unauthorized />,
  },
]);

export default router;
