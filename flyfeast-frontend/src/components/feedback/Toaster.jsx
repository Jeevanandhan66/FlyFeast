import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

export default function Toaster() {
  return (
    <ToastContainer
      position="top-right"
      autoClose={3000}
      toastClassName="z-[9999]" // ensures it appears above everything
    />
  );
}
