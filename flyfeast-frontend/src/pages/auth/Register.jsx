import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import api from "../../services/apiClient";
import { toast } from "react-toastify";

export default function Register() {
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    fullName: "",
    email: "",
    password: "",
    confirmPassword: "",
    gender: "",
    address: "",
    phoneNumber: "",
    passportNumber: "",
    nationality: "",
    dateOfBirth: "",
  });

  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);

  const validate = () => {
    const newErrors = {};
    if (!formData.fullName) newErrors.fullName = "Full name is required";
    if (!formData.email) {
      newErrors.email = "Email is required";
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = "Invalid email format";
    }
    if (!formData.password || formData.password.length < 6) {
      newErrors.password = "Password must be at least 6 characters";
    }
    if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = "Passwords do not match";
    }
    if (!formData.gender) newErrors.gender = "Gender is required";
    if (!formData.dateOfBirth) newErrors.dateOfBirth = "Date of Birth required";
    return newErrors;
  };

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
    setErrors({ ...errors, [e.target.name]: "" });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const validationErrors = validate();
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    setLoading(true);
    try {
      const res = await api.post("/Auth/register", formData);
      toast.success(res.data.message || "Registration successful!");
      navigate("/login");
    } catch (error) {
      const message =
        error.response?.data?.message ||
        "Registration failed. Please try again.";
      toast.error(message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen">
      <div className="w-full max-w-2xl p-8">
        <h2 className="text-3xl font-bold text-center text-white mb-8">
          Create Your FlyFeast Account
        </h2>
        <form
          onSubmit={handleSubmit}
          className="grid grid-cols-1 md:grid-cols-2 gap-4"
        >
          {/* Full Name */}
          <div>
            <input
              type="text"
              name="fullName"
              placeholder="Full Name"
              className="w-full border border-white/50 rounded-lg p-3 bg-white/10 text-white placeholder-white focus:ring-2 focus:ring-blue-500 focus:outline-none"
              value={formData.fullName}
              onChange={handleChange}
            />
            {errors.fullName && (
              <p className="text-sm text-red-500">{errors.fullName}</p>
            )}
          </div>

          {/* Email */}
          <div>
            <input
              type="email"
              name="email"
              placeholder="Email"
              className="w-full border border-white/50 rounded-lg p-3 bg-white/10 text-white placeholder-white focus:ring-2 focus:ring-blue-500 focus:outline-none"
              value={formData.email}
              onChange={handleChange}
            />
            {errors.email && (
              <p className="text-sm text-red-500">{errors.email}</p>
            )}
          </div>

          {/* Password */}
          <div>
            <input
              type="password"
              name="password"
              placeholder="Password"
              className="w-full border border-white/50 rounded-lg p-3 bg-white/10 text-white placeholder-white focus:ring-2 focus:ring-blue-500 focus:outline-none"
              value={formData.password}
              onChange={handleChange}
            />
            {errors.password && (
              <p className="text-sm text-red-500">{errors.password}</p>
            )}
          </div>

          {/* Confirm Password */}
          <div>
            <input
              type="password"
              name="confirmPassword"
              placeholder="Confirm Password"
              className="w-full border border-white/50 rounded-lg p-3 bg-white/10 text-white placeholder-white focus:ring-2 focus:ring-blue-500 focus:outline-none"
              value={formData.confirmPassword}
              onChange={handleChange}
            />
            {errors.confirmPassword && (
              <p className="text-sm text-red-500">{errors.confirmPassword}</p>
            )}
          </div>

          {/* Gender */}
          <div>
            <select
              name="gender"
              className="w-full border border-white/50 rounded-lg p-3 bg-white/10 text-white focus:ring-2 focus:ring-blue-500 focus:outline-none"
              value={formData.gender}
              onChange={handleChange}
            >
              <option value="" disabled hidden className="text-gray-400">
                Select Gender
              </option>

              <option className="text-black">Male</option>
              <option className="text-black">Female</option>
              <option className="text-black">Other</option>
            </select>
            {errors.gender && (
              <p className="text-sm text-red-500">{errors.gender}</p>
            )}
          </div>

          {/* Phone Number */}
          <div>
            <input
              type="text"
              name="phoneNumber"
              placeholder="Phone Number"
              className="w-full border border-white/50 rounded-lg p-3 bg-white/10 text-white placeholder-white focus:ring-2 focus:ring-blue-500 focus:outline-none"
              value={formData.phoneNumber}
              onChange={handleChange}
            />
          </div>

          {/* Passport Number */}
          <div>
            <input
              type="text"
              name="passportNumber"
              placeholder="Passport Number"
              className="w-full border border-white/50 rounded-lg p-3 bg-white/10 text-white placeholder-white focus:ring-2 focus:ring-blue-500 focus:outline-none"
              value={formData.passportNumber}
              onChange={handleChange}
            />
          </div>

          {/* Nationality */}
          <div>
            <input
              type="text"
              name="nationality"
              placeholder="Nationality"
              className="w-full border border-white/50 rounded-lg p-3 bg-white/10 text-white placeholder-white focus:ring-2 focus:ring-blue-500 focus:outline-none"
              value={formData.nationality}
              onChange={handleChange}
            />
          </div>

          {/* Address */}
          <div className="md:col-span-2">
            <input
              type="text"
              name="address"
              placeholder="Address"
              className="w-full border border-white/50 rounded-lg p-3 bg-white/10 text-white placeholder-white focus:ring-2 focus:ring-blue-500 focus:outline-none"
              value={formData.address}
              onChange={handleChange}
            />
          </div>

          {/* Date of Birth */}
          <div className="md:col-span-2">
            <label className="block text-sm font-medium text-white mb-1">
              DOB
            </label>
            <input
              type="date"
              name="dateOfBirth"
              className="w-full border border-white/50 rounded-lg p-3 bg-white/10 text-white placeholder-white focus:ring-2 focus:ring-blue-500 focus:outline-none"
              value={formData.dateOfBirth}
              onChange={handleChange}
            />
            {errors.dateOfBirth && (
              <p className="text-sm text-red-500">{errors.dateOfBirth}</p>
            )}
          </div>

          {/* Submit */}
          <div className="md:col-span-2">
            <button
              type="submit"
              disabled={loading}
              className="w-full bg-gradient-to-r from-blue-600 to-indigo-600 text-white py-3 rounded-2xl hover:from-blue-700 hover:to-indigo-700 transition shadow-lg disabled:opacity-50"
            >
              {loading ? "Registering..." : "Register"}
            </button>
          </div>
        </form>

        <p className="text-sm text-center mt-6 text-white/80">
          Already have an account?{" "}
          <Link to="/login" className="text-blue-300 hover:underline">
            Login
          </Link>
        </p>
      </div>
    </div>
  );
}
