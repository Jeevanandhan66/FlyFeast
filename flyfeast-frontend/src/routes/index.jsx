import { createBrowserRouter } from "react-router-dom";


// Layouts
import AppLayout from "../layouts/AppLayout";
import AuthLayout from "../layouts/AuthLayout";
import AdminLayout from "../layouts/AdminLayout";
import UserLayout from "../layouts/UserLayout";


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
import Profile from "../pages/user/Profile";
//Admin
import Aircraft from "../pages/admin/Aircraft";
import Airports from "../pages/admin/Airports";
import Routes from "../pages/admin/Routes";
import Schedules from "../pages/admin/Schedules";
import AdminBookings from "../pages/admin/Bookings";
import Users from "../pages/admin/Users";
import Payments from "../pages/admin/Payments";
import Refunds from "../pages/admin/Refunds";

import NotFound from "../pages/errors/NotFound";
import Unauthorized from "../pages/errors/Unauthorized";
import Dashboard from "../pages/admin/Dashboard";
import ProtectedRoute from "./ProtectedRoute";

const router = createBrowserRouter([
  // Public/User routes
  {
    path: "/",
    element: <AppLayout />,
    errorElement: <NotFound />,
    children: [
      { index: true, element: <Home /> },
      { path: "login", element: <Login /> },
      { path: "register", element: <Register /> },
      { path: "results", element: <Results /> },
      { path: "booking/:id", element: <Booking /> },
      { path: "booking/review", element: <Review /> },
      { path: "booking/payment", element: <Payment /> },
      { path: "booking/confirmation", element: <Confirmation /> },
      { path: "user/bookings", element: <Bookings /> },
      { path: "user/bookings/:id", element: <BookingDetail /> },
    ],
  },

  // User routes (protected by UserLayout)
  {
    path: "/user",
    element: <UserLayout />,
    children: [
      { path: "profile", element: <Profile /> },
      { path: "bookings", element: <Bookings /> },
      { path: "bookings/:id", element: <BookingDetail /> },
    ],
  },

  // Admin protected routes
  {
    path: "/admin",
    element: <ProtectedRoute role="Admin" />, // wrapper that checks admin role
    children: [
      {
        element: <AdminLayout />,
        children: [
          { index: true, element: <Dashboard /> },
          { path: "aircraft", element: <Aircraft /> },
          { path: "airports", element: <Airports /> },
          { path: "routes", element: <Routes /> },
          { path: "schedules", element: <Schedules /> },
          { path: "bookings", element: <AdminBookings /> },
          { path: "users", element: <Users /> },
          { path: "payments", element: <Payments /> },
          { path: "refunds", element: <Refunds /> },


        ],
      },
    ],
  },

  // Unauthorized page
  {
    path: "/unauthorized",
    element: <Unauthorized />,
  },
]);

export default router;
