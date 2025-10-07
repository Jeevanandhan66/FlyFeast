// src/pages/admin/Aircraft.jsx
import { useEffect, useMemo, useState } from "react";
import {
  getAircrafts,
  createAircraft,
  updateAircraft,
  deleteAircraft,
} from "../../services/adminService";

function initialForm() {
  return {
    aircraftCode: "",
    aircraftName: "",
    ownerId: "",
    economySeats: 1,
    businessSeats: 0,
    firstClassSeats: 0,
  };
}

export default function Aircraft() {
  const [list, setList] = useState([]);
  const [loading, setLoading] = useState(true);
  const [err, setErr] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [mode, setMode] = useState("create"); // "create" | "edit"
  const [form, setForm] = useState(initialForm());
  const [editingId, setEditingId] = useState(null);
  const [submitting, setSubmitting] = useState(false);
  const [filter, setFilter] = useState("");

  // Derived filtered list
  const filtered = useMemo(() => {
    if (!filter.trim()) return list;
    const f = filter.toLowerCase();
    return list.filter((a) => {
      const ownerDisplay =
        a?.owner?.fullName ||
        a?.owner?.userName ||
        a?.owner?.email ||
        a?.owner?.id ||
        a?.ownerId ||
        "";
      return (
        `${a.aircraftCode || ""} ${a.aircraftName || ""} ${ownerDisplay}`.toLowerCase().includes(f)
      );
    });
  }, [list, filter]);

  async function load() {
    try {
      setLoading(true);
      setErr("");
      const data = await getAircrafts();
      setList(Array.isArray(data) ? data : []);
    } catch (e) {
      setErr(e?.response?.data?.message || e?.message || "Failed to load aircraft.");
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
    setEditingId(item.aircraftId);
    setForm({
      aircraftCode: item.aircraftCode || "",
      aircraftName: item.aircraftName || "",
      ownerId: item.owner?.id || item.ownerId || "",
      economySeats: Number(item.economySeats ?? 1),
      businessSeats: Number(item.businessSeats ?? 0),
      firstClassSeats: Number(item.firstClassSeats ?? 0),
    });
    setModalOpen(true);
  }

  function closeModal() {
    if (submitting) return;
    setModalOpen(false);
  }

  function onChange(e) {
    const { name, value } = e.target;
    setForm((prev) => ({
      ...prev,
      [name]:
        name === "economySeats" || name === "businessSeats" || name === "firstClassSeats"
          ? Number(value)
          : value,
    }));
  }

  function validate() {
    const errors = {};
    // Required
    if (!form.aircraftCode?.trim()) errors.aircraftCode = "Aircraft code is required.";
    if (!form.ownerId?.trim()) errors.ownerId = "Owner ID is required.";
    // Ranges
    if (!(Number(form.economySeats) >= 1))
      errors.economySeats = "Economy seats must be at least 1.";
    if (!(Number(form.businessSeats) >= 0))
      errors.businessSeats = "Business seats must be 0 or more.";
    if (!(Number(form.firstClassSeats) >= 0))
      errors.firstClassSeats = "First-class seats must be 0 or more.";
    // String lengths
    if (form.aircraftCode && form.aircraftCode.length > 20)
      errors.aircraftCode = "Max 20 characters.";
    if (form.aircraftName && form.aircraftName.length > 100)
      errors.aircraftName = "Max 100 characters.";
    return errors;
  }

  const [vErrors, setVErrors] = useState({});
  async function onSubmit(e) {
    e.preventDefault();
    const errors = validate();
    setVErrors(errors);
    if (Object.keys(errors).length) return;

    setSubmitting(true);
    setErr("");

    const payload = {
      aircraftCode: form.aircraftCode.trim(),
      aircraftName: form.aircraftName?.trim() || null,
      ownerId: form.ownerId.trim(),
      economySeats: Number(form.economySeats),
      businessSeats: Number(form.businessSeats),
      firstClassSeats: Number(form.firstClassSeats),
    };

    try {
      if (mode === "create") {
        await createAircraft(payload);
      } else {
        await updateAircraft(editingId, payload);
      }
      await load();
      setModalOpen(false);
    } catch (e) {
      setErr(e?.response?.data?.message || e?.message || "Failed to save aircraft.");
    } finally {
      setSubmitting(false);
    }
  }

  async function onDelete(id) {
    const confirm = window.confirm("Delete this aircraft? This action cannot be undone.");
    if (!confirm) return;
    try {
      await deleteAircraft(id);
      await load();
    } catch (e) {
      setErr(e?.response?.data?.message || e?.message || "Failed to delete aircraft.");
    }
  }

  function ownerDisplay(a) {
    return (
      a?.owner?.fullName ||
      a?.owner?.userName ||
      a?.owner?.email ||
      a?.owner?.id ||
      a?.ownerId ||
      "-"
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3">
        <div>
          <h1 className="text-xl font-semibold">Aircraft</h1>
          <p className="text-sm text-gray-500">Manage aircraft inventory and seat capacities.</p>
        </div>
        <div className="flex items-center gap-3">
          <input
            type="text"
            value={filter}
            onChange={(e) => setFilter(e.target.value)}
            placeholder="Search code, name, owner…"
            className="w-56 rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
          />
          <button
            onClick={openCreate}
            className="rounded-lg bg-indigo-600 px-4 py-2 text-white text-sm font-medium hover:bg-indigo-700 transition"
          >
            Add Aircraft
          </button>
        </div>
      </div>

      {/* Error */}
      {err && (
        <div className="rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {err}
        </div>
      )}

      {/* Table / Loading / Empty */}
      <div className="bg-white rounded-xl shadow">
        {loading ? (
          <div className="p-8 text-center text-gray-500">Loading aircraft…</div>
        ) : filtered.length === 0 ? (
          <div className="p-8 text-center text-gray-500">No aircraft found.</div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm text-left text-gray-700">
              <thead>
                <tr className="bg-gray-100 text-gray-700">
                  <th className="px-4 py-2">ID</th>
                  <th className="px-4 py-2">Code</th>
                  <th className="px-4 py-2">Name</th>
                  <th className="px-4 py-2">Owner</th>
                  <th className="px-4 py-2">Economy</th>
                  <th className="px-4 py-2">Business</th>
                  <th className="px-4 py-2">First</th>
                  <th className="px-4 py-2 text-right">Actions</th>
                </tr>
              </thead>
              <tbody>
                {filtered.map((a) => (
                  <tr key={a.aircraftId} className="border-b last:border-0">
                    <td className="px-4 py-2">{a.aircraftId}</td>
                    <td className="px-4 py-2 font-medium">{a.aircraftCode}</td>
                    <td className="px-4 py-2">{a.aircraftName || "-"}</td>
                    <td className="px-4 py-2">{ownerDisplay(a)}</td>
                    <td className="px-4 py-2">{a.economySeats}</td>
                    <td className="px-4 py-2">{a.businessSeats}</td>
                    <td className="px-4 py-2">{a.firstClassSeats}</td>
                    <td className="px-4 py-2 text-right">
                      <button
                        onClick={() => openEdit(a)}
                        className="mr-2 rounded border border-gray-300 px-3 py-1.5 text-xs hover:bg-gray-50"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => onDelete(a.aircraftId)}
                        className="rounded bg-red-500 px-3 py-1.5 text-xs text-white hover:bg-red-600"
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
      </div>

      {/* Modal */}
      {modalOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <div className="w-full max-w-xl rounded-xl bg-white shadow-lg">
            <div className="flex items-center justify-between border-b px-5 py-4">
              <h2 className="text-lg font-semibold">
                {mode === "create" ? "Add Aircraft" : "Edit Aircraft"}
              </h2>
              <button
                onClick={closeModal}
                className="rounded px-2 py-1 text-gray-500 hover:bg-gray-100"
                disabled={submitting}
              >
                ✕
              </button>
            </div>

            <form onSubmit={onSubmit} className="space-y-4 px-5 py-5">
              {/* Aircraft Code */}
              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Aircraft Code <span className="text-red-500">*</span>
                </label>
                <input
                  name="aircraftCode"
                  value={form.aircraftCode}
                  onChange={onChange}
                  placeholder="e.g., A320, B737-8"
                  maxLength={20}
                  className="mt-1 w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
                {vErrors.aircraftCode && (
                  <p className="mt-1 text-xs text-red-600">{vErrors.aircraftCode}</p>
                )}
              </div>

              {/* Aircraft Name (optional) */}
              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Aircraft Name
                </label>
                <input
                  name="aircraftName"
                  value={form.aircraftName}
                  onChange={onChange}
                  placeholder="e.g., Airbus A320neo"
                  maxLength={100}
                  className="mt-1 w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
                {vErrors.aircraftName && (
                  <p className="mt-1 text-xs text-red-600">{vErrors.aircraftName}</p>
                )}
              </div>

              {/* Owner ID */}
              <div>
                <label className="block text-sm font-medium text-gray-700">
                  Owner ID <span className="text-red-500">*</span>
                </label>
                <input
                  name="ownerId"
                  value={form.ownerId}
                  onChange={onChange}
                  placeholder="User ID (GUID/Identity)"
                  className="mt-1 w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
                {vErrors.ownerId && (
                  <p className="mt-1 text-xs text-red-600">{vErrors.ownerId}</p>
                )}
                <p className="mt-1 text-xs text-gray-500">
                  (Later you can replace this with a user picker.)
                </p>
              </div>

              {/* Seats */}
              <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700">
                    Economy Seats <span className="text-red-500">*</span>
                  </label>
                  <input
                    type="number"
                    min={1}
                    name="economySeats"
                    value={form.economySeats}
                    onChange={onChange}
                    className="mt-1 w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  />
                  {vErrors.economySeats && (
                    <p className="mt-1 text-xs text-red-600">{vErrors.economySeats}</p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">
                    Business Seats
                  </label>
                  <input
                    type="number"
                    min={0}
                    name="businessSeats"
                    value={form.businessSeats}
                    onChange={onChange}
                    className="mt-1 w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  />
                  {vErrors.businessSeats && (
                    <p className="mt-1 text-xs text-red-600">{vErrors.businessSeats}</p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">
                    First-class Seats
                  </label>
                  <input
                    type="number"
                    min={0}
                    name="firstClassSeats"
                    value={form.firstClassSeats}
                    onChange={onChange}
                    className="mt-1 w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  />
                  {vErrors.firstClassSeats && (
                    <p className="mt-1 text-xs text-red-600">{vErrors.firstClassSeats}</p>
                  )}
                </div>
              </div>

              {/* Actions */}
              <div className="flex items-center justify-end gap-3 pt-2">
                <button
                  type="button"
                  className="rounded-lg border border-gray-300 px-4 py-2 text-sm hover:bg-gray-50"
                  onClick={closeModal}
                  disabled={submitting}
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="rounded-lg bg-indigo-600 px-4 py-2 text-sm font-medium text-white hover:bg-indigo-700 disabled:opacity-50"
                  disabled={submitting}
                >
                  {submitting ? "Saving..." : mode === "create" ? "Create" : "Update"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
