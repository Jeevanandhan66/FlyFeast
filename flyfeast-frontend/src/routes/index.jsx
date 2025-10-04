import { createBrowserRouter } from "react-router-dom";
import AppLayout from "../layouts/AppLayout";
import Login from "../pages/auth/Login";
import Register from "../pages/auth/Register";
import NotFound from "../pages/errors/NotFound";
import Home from "../pages/Home";

const router = createBrowserRouter([
  {
    path: "/",
    element: <AppLayout />,
    errorElement: <NotFound />,
    children: [
      { index: true, element: <Home /> }, // 👈 this ensures "/" shows Home
      { path: "login", element: <Login /> },
      { path: "register", element: <Register /> },
    ],
  },
]);

export default router;
