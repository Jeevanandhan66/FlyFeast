import { RouterProvider } from "react-router-dom";
import router from "../routes";
import { AuthProvider } from "../context/AuthContext";

export default function AppProviders() {
  return (
    <AuthProvider>
      <RouterProvider router={router} />
    </AuthProvider>
  );
}
