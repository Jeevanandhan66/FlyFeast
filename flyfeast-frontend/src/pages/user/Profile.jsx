import { useEffect, useState } from "react";
import { useAuth } from "../../context/AuthContext";
import api from "../../services/apiClient";
import { toast } from "react-toastify";

export default function Profile() {
  const { user } = useAuth();
  const [passenger, setPassenger] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchPassenger() {
      try {
        if (!user?.userId) return;
        const res = await api.get(`/Passenger/user/${user.userId}`);
        setPassenger(res.data);
      } catch (err) {
        toast.error("Failed to load profile.");
      } finally {
        setLoading(false);
      }
    }
    fetchPassenger();
  }, [user]);

  if (loading) {
    return <p className="text-center mt-10">Loading profile...</p>;
  }

  if (!passenger) {
    return <p className="text-center mt-10">No profile found.</p>;
  }

  return (
    <div className="max-w-3xl mx-auto bg-white shadow-lg rounded-2xl p-8">
      <h1 className="text-2xl font-bold text-blue-600 mb-6">My Profile</h1>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {/* Full Name */}
        <div>
          <p className="text-sm text-gray-500">Full Name</p>
          <p className="font-medium">{passenger.fullName || "—"}</p>
        </div>

        {/* Email */}
        <div>
          <p className="text-sm text-gray-500">Email</p>
          <p className="font-medium">{passenger.email || "—"}</p>
        </div>

        {/* Passport */}
        <div>
          <p className="text-sm text-gray-500">Passport Number</p>
          <p className="font-medium">{passenger.passportNumber || "—"}</p>
        </div>

        {/* Nationality */}
        <div>
          <p className="text-sm text-gray-500">Nationality</p>
          <p className="font-medium">{passenger.nationality || "—"}</p>
        </div>

        {/* Gender */}
        <div>
          <p className="text-sm text-gray-500">Gender</p>
          <p className="font-medium">{passenger.gender || "—"}</p>
        </div>

        {/* Address */}
        <div className="md:col-span-2">
          <p className="text-sm text-gray-500">Address</p>
          <p className="font-medium">{passenger.address || "—"}</p>
        </div>
      </div>
    </div>
  );
}
