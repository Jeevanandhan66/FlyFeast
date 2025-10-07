import { useEffect, useState, useMemo } from "react";
import { useToast } from "../../hooks/useToast";

import {
  getAirports,
  createAirport,
  updateAirport,
  deleteAirport,
} from "../../services/adminService";

function initialForm() {
  return {
    airportName: "",
    code: "",
    city: "",
    country: "",
  };
}

export default function Airports() {
  const [list, setList] = useState([]);
  const [loading, setLoading] = useState(true);
  const [err, setErr] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [mode, setMode] = useState("create"); // create | edit
  const [form, setForm] = useState(initialForm());
  const [editingId, setEditingId] = useState(null);
  const [submitting, setSubmitting] = useState(false);
  const [filter, setFilter] = useState("");
  const [vErrors, setVErrors] = useState({});
  const toast = useToast();

  const filtered = useMemo(() => {
    if (!filter.trim()) return list;
    const f = filter.toLowerCase();
    return list.filter((a) =>
      `${a.airportName} ${a.code} ${a.city} ${a.country}`.toLowerCase().includes(f)
    );
  }, [list, filter]);

  async function load() {
    try {
      setLoading(true);
      setErr("");
      const data = await getAirports();
      setList(Array.isArray(data) ? data : []);
    } catch (e) {
      setErr(e?.response?.data?.message || e?.message || "Failed to load airports.");
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
    setEditingId(item.airportId);
    setForm({
      airportName: item.airportName || "",
      code: item.code || "",
      city: item.city || "",
      country: item.country || "",
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

  function validate() {
    const errors = {};
    if (!form.airportName?.trim()) errors.airportName = "Airport name is required.";
    if (!form.code?.trim()) errors.code = "Code is required.";
    if (!form.city?.trim()) errors.city = "City is required.";
    if (!form.country?.trim()) errors.country = "Country is required.";
    if (form.code && form.code.length > 10)
      errors.code = "Code must be at most 10 characters.";
    if (form.airportName && form.airportName.length > 200)
      errors.airportName = "Name max length is 200.";
    return errors;
  }

  async function onSubmit(e) {
    e.preventDefault();
    const errors = validate();
    setVErrors(errors);
    if (Object.keys(errors).length) return;

    setSubmitting(true);
    setErr("");

    const payload = {
      airportName: form.airportName.trim(),
      code: form.code.trim(),
      city: form.city.trim(),
      country: form.country.trim(),
    };

    try {
      if (mode === "create") {
        await createAirport(payload);
        toast.success("Airport added successfully!");
      } else {
        await updateAirport(editingId, payload);
        toast.success("Airport updated successfully!");
      }
      await load();
      setModalOpen(false);
    } catch (e) {
      setErr(
        e?.response?.data?.message || e?.message || "Failed to save airport."
      );
    } finally {
      setSubmitting(false);
    }
  }

  async function onDelete(id) {
    if (!window.confirm("Delete this airport?")) return;
    try {
      await deleteAirport(id);
      toast.success("Airport deleted successfully!");
      await load();
    } catch (e) {
      const message =
        e?.response?.data?.message || e?.message || "Failed to delete airport.";
      setErr(message);
      toast.error(message);
    }
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3">
        <div>
          <h1 className="text-xl font-semibold">Airports</h1>
          <p className="text-sm text-gray-500">
            Manage airports, codes, cities, and countries.
          </p>
        </div>
        <div className="flex items-center gap-3">
          <input
            type="text"
            value={filter}
            onChange={(e) => setFilter(e.target.value)}
            placeholder="Search name, code, city, country..."
            className="w-64 rounded-lg border border-gray-300 px-3 py-2 text-sm focus:ring-2 focus:ring-indigo-500"
          />
          <button
            onClick={openCreate}
            className="rounded-lg bg-indigo-600 px-4 py-2 text-white text-sm font-medium hover:bg-indigo-700 transition"
          >
            Add Airport
          </button>
        </div>
      </div>

      {/* Error */}
      {err && (
        <div className="rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {err}
        </div>
      )}

      {/* Table */}
      <div className="bg-white rounded-xl shadow">
        {loading ? (
          <div className="p-8 text-center text-gray-500">Loading airports…</div>
        ) : filtered.length === 0 ? (
          <div className="p-8 text-center text-gray-500">
            No airports found.
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm text-left text-gray-700">
              <thead>
                <tr className="bg-gray-100 text-gray-700">
                  <th className="px-4 py-2">ID</th>
                  <th className="px-4 py-2">Name</th>
                  <th className="px-4 py-2">Code</th>
                  <th className="px-4 py-2">City</th>
                  <th className="px-4 py-2">Country</th>
                  <th className="px-4 py-2 text-right">Actions</th>
                </tr>
              </thead>
              <tbody>
                {filtered.map((a) => (
                  <tr key={a.airportId} className="border-b last:border-0">
                    <td className="px-4 py-2">{a.airportId}</td>
                    <td className="px-4 py-2 font-medium">{a.airportName}</td>
                    <td className="px-4 py-2">{a.code}</td>
                    <td className="px-4 py-2">{a.city}</td>
                    <td className="px-4 py-2">{a.country}</td>
                    <td className="px-4 py-2 text-right">
                      <button
                        onClick={() => openEdit(a)}
                        className="mr-2 rounded border border-gray-300 px-3 py-1.5 text-xs hover:bg-gray-50"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => onDelete(a.airportId)}
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
          <div className="w-full max-w-md rounded-xl bg-white shadow-lg">
            <div className="flex items-center justify-between border-b px-5 py-4">
              <h2 className="text-lg font-semibold">
                {mode === "create" ? "Add Airport" : "Edit Airport"}
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
              <div>
                <label className="block text-sm font-medium">
                  Airport Name *
                </label>
                <input
                  name="airportName"
                  value={form.airportName}
                  onChange={onChange}
                  className="mt-1 w-full rounded-lg border px-3 py-2 text-sm"
                />
                {vErrors.airportName && (
                  <p className="text-xs text-red-600">{vErrors.airportName}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium">Code *</label>
                <input
                  name="code"
                  value={form.code}
                  onChange={onChange}
                  className="mt-1 w-full rounded-lg border px-3 py-2 text-sm"
                />
                {vErrors.code && (
                  <p className="text-xs text-red-600">{vErrors.code}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium">City *</label>
                <input
                  name="city"
                  value={form.city}
                  onChange={onChange}
                  className="mt-1 w-full rounded-lg border px-3 py-2 text-sm"
                />
                {vErrors.city && (
                  <p className="text-xs text-red-600">{vErrors.city}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium">Country *</label>
                <input
                  name="country"
                  value={form.country}
                  onChange={onChange}
                  className="mt-1 w-full rounded-lg border px-3 py-2 text-sm"
                />
                {vErrors.country && (
                  <p className="text-xs text-red-600">{vErrors.country}</p>
                )}
              </div>

              <div className="flex justify-end gap-3">
                <button
                  type="button"
                  onClick={closeModal}
                  className="rounded-lg border border-gray-300 px-4 py-2 text-sm"
                  disabled={submitting}
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="rounded-lg bg-indigo-600 px-4 py-2 text-sm text-white hover:bg-indigo-700 disabled:opacity-50"
                  disabled={submitting}
                >
                  {submitting
                    ? "Saving..."
                    : mode === "create"
                      ? "Create"
                      : "Update"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
