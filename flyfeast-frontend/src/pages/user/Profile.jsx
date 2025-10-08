import { useEffect, useState } from "react";
import { useAuth } from "../../context/AuthContext";
import { getUsers, updateUser } from "../../services/adminService";
import { toast } from "react-toastify";

function initialForm() {
  return {
    userName: "", // kept but hidden
    fullName: "",
    email: "",
    phone: "",
    gender: "",
    address: "",
    password: "",
    confirmPassword: "",
  };
}

export default function Profile() {
  const { user } = useAuth(); // has userId from auth
  const [form, setForm] = useState(initialForm());
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState(false);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    async function loadUser() {
      try {
        setLoading(true);
        const users = await getUsers();
        const me = users.find((u) => u.id === user?.userId);
        if (me) {
          setForm({
            userName: me.userName || "",
            fullName: me.fullName || "",
            email: me.email || "",
            phone: me.phone || "",
            gender: me.gender || "",
            address: me.address || "",
            password: "",
            confirmPassword: "",
          });
        }
      } catch (err) {
        toast.error("Failed to load profile.");
      } finally {
        setLoading(false);
      }
    }
    if (user?.userId) loadUser();
  }, [user]);

  function onChange(e) {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
  }

async function onSubmit(e) {
  e.preventDefault();
  if (!user?.userId) return;

  if (form.password && form.password !== form.confirmPassword) {
    toast.error("Passwords do not match.");
    return;
  }

  try {
    setSaving(true);

    const payload = {
      id: user.userId,          // include ID
      userName: form.userName,  // required for backend
      fullName: form.fullName,
      email: form.email,
      phone: form.phone,
      gender: form.gender,
      address: form.address,
    };

    if (form.password) {
      payload.password = form.password;
    }

    console.log("🔍 Sending payload:", payload);

    await updateUser(user.userId, payload);
    toast.success("Profile updated successfully!");
    setEditing(false);
    // clear sensitive fields
    setForm((prev) => ({ ...prev, password: "", confirmPassword: "" }));
  } catch (err) {
    console.error("❌ Update error:", err?.response || err);
    toast.error(err?.response?.data?.error || "Failed to update profile.");
  } finally {
    setSaving(false);
  }
}

  if (loading) {
    return <p className="text-center mt-10">Loading profile...</p>;
  }

  return (
    <div className="max-w-2xl mx-auto bg-white shadow-lg rounded-2xl p-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-indigo-600">My Profile</h1>
        {!editing && (
          <button
            onClick={() => setEditing(true)}
            className="px-4 py-2 rounded-lg bg-indigo-600 text-white hover:bg-indigo-700 transition"
          >
            Edit
          </button>
        )}
      </div>

      {!editing ? (
        // ----- VIEW MODE -----
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6 text-gray-700">
          <div className="p-4 bg-gray-50 rounded-lg shadow-sm">
            <p className="text-xs uppercase text-gray-500">Full Name</p>
            <p className="text-base font-semibold">{form.fullName || "—"}</p>
          </div>
          <div className="p-4 bg-gray-50 rounded-lg shadow-sm">
            <p className="text-xs uppercase text-gray-500">Email</p>
            <p className="text-base font-semibold">{form.email || "—"}</p>
          </div>
          <div className="p-4 bg-gray-50 rounded-lg shadow-sm">
            <p className="text-xs uppercase text-gray-500">Phone</p>
            <p className="text-base font-semibold">{form.phone || "—"}</p>
          </div>
          <div className="p-4 bg-gray-50 rounded-lg shadow-sm">
            <p className="text-xs uppercase text-gray-500">Gender</p>
            <p className="text-base font-semibold">{form.gender || "—"}</p>
          </div>
          <div className="md:col-span-2 p-4 bg-gray-50 rounded-lg shadow-sm">
            <p className="text-xs uppercase text-gray-500">Address</p>
            <p className="text-base font-semibold">{form.address || "—"}</p>
          </div>
        </div>
      ) : (
        // ----- EDIT MODE -----
        <form onSubmit={onSubmit} className="space-y-4">
          {/* Hidden Username (sent but not shown) */}
          <input type="hidden" name="userName" value={form.userName} />

          <div>
            <label className="block text-sm font-medium">Full Name</label>
            <input
              name="fullName"
              value={form.fullName}
              onChange={onChange}
              className="w-full border rounded-lg px-3 py-2 focus:ring-2 focus:ring-indigo-500"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium">Email</label>
            <input
              type="email"
              name="email"
              value={form.email}
              onChange={onChange}
              className="w-full border rounded-lg px-3 py-2 focus:ring-2 focus:ring-indigo-500"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium">Phone</label>
            <input
              name="phone"
              value={form.phone}
              onChange={onChange}
              className="w-full border rounded-lg px-3 py-2 focus:ring-2 focus:ring-indigo-500"
            />
          </div>
          <div>
            <label className="block text-sm font-medium">Gender</label>
            <select
              name="gender"
              value={form.gender}
              onChange={onChange}
              className="w-full border rounded-lg px-3 py-2 focus:ring-2 focus:ring-indigo-500"
            >
              <option value="">Select...</option>
              <option value="Male">Male</option>
              <option value="Female">Female</option>
              <option value="Other">Other</option>
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium">Address</label>
            <input
              name="address"
              value={form.address}
              onChange={onChange}
              className="w-full border rounded-lg px-3 py-2 focus:ring-2 focus:ring-indigo-500"
            />
          </div>
          <div>
            <label className="block text-sm font-medium">Password</label>
            <input
              type="password"
              name="password"
              value={form.password}
              onChange={onChange}
              className="w-full border rounded-lg px-3 py-2 focus:ring-2 focus:ring-indigo-500"
              placeholder="Leave blank to keep current password"
            />
          </div>
          <div>
            <label className="block text-sm font-medium">Confirm Password</label>
            <input
              type="password"
              name="confirmPassword"
              value={form.confirmPassword}
              onChange={onChange}
              className="w-full border rounded-lg px-3 py-2 focus:ring-2 focus:ring-indigo-500"
              placeholder="Re-enter password"
            />
          </div>

          <div className="flex justify-end gap-3 pt-4">
            <button
              type="button"
              onClick={() => setEditing(false)}
              className="px-4 py-2 border rounded-lg hover:bg-gray-100"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={saving}
              className="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition"
            >
              {saving ? "Saving..." : "Save Changes"}
            </button>
          </div>
        </form>
      )}
    </div>
  );
}
