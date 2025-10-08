import { useEffect, useState, useMemo } from "react";
import { useToast } from "../../hooks/useToast";
import {
  getUsers,
  createUser,
  updateUser,
  deleteUser,
  toggleUserActive,
  assignRole,
  getRoles,
} from "../../services/adminService";

function initialForm() {
  return {
    userName: "",
    fullName: "",
    email: "",
    phone: "",
    gender: "",
    address: "",
    password: "",
    roleNames: [],
  };
}

export default function Users() {
  const toast = useToast(); //
  const [list, setList] = useState([]);
  const [roles, setRoles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [err, setErr] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [mode, setMode] = useState("create");
  const [form, setForm] = useState(initialForm());
  const [editingId, setEditingId] = useState(null);
  const [submitting, setSubmitting] = useState(false);
  const [filter, setFilter] = useState("");

  const filtered = useMemo(() => {
    if (!filter.trim()) return list;
    const f = filter.toLowerCase();
    return list.filter((u) =>
      `${u.fullName} ${u.email} ${u.userName} ${u.phone}`.toLowerCase().includes(f)
    );
  }, [list, filter]);

  async function load() {
    try {
      setLoading(true);
      setErr("");
      const data = await getUsers();
      const r = await getRoles();
      setList(Array.isArray(data) ? data : []);
      setRoles(Array.isArray(r) ? r : []);
    } catch (e) {
      setErr(e?.response?.data?.error || e.message || "Failed to load users.");
      toast.error("Failed to load users.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    load();
  }, []);

  function openCreate() {
    setMode("create");
    setEditingId(null);
    setForm(initialForm());
    setModalOpen(true);
  }

  function openEdit(item) {
    setMode("edit");
    setEditingId(item.id);
    setForm({
      userName: item.userName || "",
      fullName: item.fullName || "",
      email: item.email || "",
      phone: item.phone || "",
      gender: item.gender || "",
      address: item.address || "",
      password: "",
      roleNames: item.roles?.map((r) => r.name) || [],
    });
    setModalOpen(true);
  }

  function closeModal() {
    if (!submitting) setModalOpen(false);
  }

  function onChange(e) {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
  }

  async function onSubmit(e) {
    e.preventDefault();
    setSubmitting(true);
    setErr("");

    try {
      const payload = { ...form };
      if (mode === "create") {
        await createUser(payload);
        toast.success("User created successfully!");
      } else {
        await updateUser(editingId, payload);
        toast.success("User updated successfully!");
      }
      await load();
      setModalOpen(false);
    } catch (e) {
      setErr(e?.response?.data?.error || e.message || "Failed to save user.");
      toast.error("Error saving user.");
    } finally {
      setSubmitting(false);
    }
  }

  async function onDelete(id) {
    if (!window.confirm("Delete this user?")) return;
    try {
      await deleteUser(id);
      await load();
      toast.success("User deleted.");
    } catch (e) {
      toast.error(e?.response?.data?.error || "Delete failed.");
    }
  }

  async function onToggleActive(id, current) {
    try {
      await toggleUserActive(id, !current);
      await load();
      toast.success(`User ${!current ? "activated" : "deactivated"}.`);
    } catch (e) {
      toast.error(e?.response?.data?.error || "Toggle failed.");
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between">
        <input
          type="text"
          placeholder="Search users..."
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
          className="border rounded px-3 py-2 text-sm"
        />
        <button
          onClick={openCreate}
          className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700"
        >
          + Add User
        </button>
      </div>

      {loading ? (
        <p>Loading...</p>
      ) : err ? (
        <p className="text-red-500">{err}</p>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full text-sm text-left text-gray-600">
            <thead>
              <tr className="bg-gray-100 text-gray-700">
                <th className="px-4 py-2">Name</th>
                <th className="px-4 py-2">Email</th>
                <th className="px-4 py-2">Phone</th>
                <th className="px-4 py-2">Roles</th>
                <th className="px-4 py-2">Status</th>
                <th className="px-4 py-2">Actions</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map((u) => (
                <tr key={u.id} className="border-b">
                  <td className="px-4 py-2">{u.fullName}</td>
                  <td className="px-4 py-2">{u.email}</td>
                  <td className="px-4 py-2">{u.phone}</td>
                  <td className="px-4 py-2">
                    {u.roles?.map((r) => (
                      <span
                        key={r.name}
                        className="px-2 py-1 bg-indigo-100 text-indigo-700 rounded-full text-xs mr-1"
                      >
                        {r.name}
                      </span>
                    ))}
                  </td>
                  <td className="px-4 py-2">
                    {u.isActive ? (
                      <span className="text-green-600">Active</span>
                    ) : (
                      <span className="text-red-600">Inactive</span>
                    )}
                  </td>
                  <td className="px-4 py-2 space-x-2">
                    <button
                      onClick={() => openEdit(u)}
                      className="text-blue-600 hover:underline"
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => onToggleActive(u.id, u.isActive)}
                      className="text-yellow-600 hover:underline"
                    >
                      {u.isActive ? "Deactivate" : "Activate"}
                    </button>
                    <button
                      onClick={() => onDelete(u.id)}
                      className="text-red-600 hover:underline"
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* Modal */}
      {modalOpen && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg shadow-lg p-6 w-full max-w-lg">
            <h2 className="text-lg font-semibold mb-4">
              {mode === "create" ? "Add User" : "Edit User"}
            </h2>
            <form onSubmit={onSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium">Full Name</label>
                <input
                  name="fullName"
                  value={form.fullName}
                  onChange={onChange}
                  className="w-full border rounded px-3 py-2"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium">Username</label>
                <input
                  name="userName"
                  value={form.userName}
                  onChange={onChange}
                  className="w-full border rounded px-3 py-2"
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
                  className="w-full border rounded px-3 py-2"
                  required
                />
              </div>
              {mode === "create" && (
                <div>
                  <label className="block text-sm font-medium">Password</label>
                  <input
                    type="password"
                    name="password"
                    value={form.password}
                    onChange={onChange}
                    className="w-full border rounded px-3 py-2"
                    required
                  />
                </div>
              )}
              <div>
                <label className="block text-sm font-medium">Phone</label>
                <input
                  name="phone"
                  value={form.phone}
                  onChange={onChange}
                  className="w-full border rounded px-3 py-2"
                />
              </div>
              <div>
                <label className="block text-sm font-medium">Gender</label>
                <select
                  name="gender"
                  value={form.gender}
                  onChange={onChange}
                  className="w-full border rounded px-3 py-2"
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
                  className="w-full border rounded px-3 py-2"
                />
              </div>
              <div>
                <label className="block text-sm font-medium">Roles</label>
                <select
                  multiple
                  value={form.roleNames}
                  onChange={(e) =>
                    setForm((prev) => ({
                      ...prev,
                      roleNames: Array.from(
                        e.target.selectedOptions,
                        (o) => o.value
                      ),
                    }))
                  }
                  className="w-full border rounded px-3 py-2"
                >
                  {roles.map((r) => (
                    <option key={r.id} value={r.name}>
                      {r.name}
                    </option>
                  ))}
                </select>
              </div>

              <div className="flex justify-end space-x-3 pt-4">
                <button
                  type="button"
                  onClick={closeModal}
                  className="px-4 py-2 border rounded"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={submitting}
                  className="px-4 py-2 bg-indigo-600 text-white rounded hover:bg-indigo-700"
                >
                  {submitting ? "Saving..." : "Save"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
