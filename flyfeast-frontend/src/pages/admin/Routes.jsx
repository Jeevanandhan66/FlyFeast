import { useEffect, useState, useMemo } from "react";
import { useToast } from "../../hooks/useToast";
import { formatCurrency } from "../../utils/formatters";
import {
  getRoutes,
  createRoute,
  updateRoute,
  deleteRoute,
} from "../../services/adminService";
import { getAirports, getAircrafts } from "../../services/adminService";

function initialForm() {
  return {
    aircraftId: "",
    originAirportId: "",
    destinationAirportId: "",
    baseFare: 1,
  };
}

export default function RoutesPage() {
  const toast = useToast();

  const [list, setList] = useState([]);
  const [airports, setAirports] = useState([]);
  const [aircrafts, setAircrafts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [err, setErr] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [mode, setMode] = useState("create");
  const [form, setForm] = useState(initialForm());
  const [editingId, setEditingId] = useState(null);
  const [submitting, setSubmitting] = useState(false);
  const [filter, setFilter] = useState("");
  const [vErrors, setVErrors] = useState({});

  const filtered = useMemo(() => {
    if (!filter.trim()) return list;
    const f = filter.toLowerCase();
    return list.filter((r) =>
      `${r.aircraft.aircraftCode} ${r.originAirport.airportName} ${r.destinationAirport.airportName}`
        .toLowerCase()
        .includes(f)
    );
  }, [list, filter]);

  async function load() {
    try {
      setLoading(true);
      setErr("");
      const [routesData, airportsData, aircraftsData] = await Promise.all([
        getRoutes(),
        getAirports(),
        getAircrafts(),
      ]);
      setList(routesData || []);
      setAirports(airportsData || []);
      setAircrafts(aircraftsData || []);
    } catch (e) {
      setErr(
        e?.response?.data?.message || e?.message || "Failed to load routes."
      );
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
    setEditingId(item.routeId);
    setForm({
      aircraftId: item.aircraft.aircraftId,
      originAirportId: item.originAirport.airportId,
      destinationAirportId: item.destinationAirport.airportId,
      baseFare: item.baseFare,
    });
    setModalOpen(true);
  }

  function closeModal() {
    if (!submitting) setModalOpen(false);
  }

  function onChange(e) {
    const { name, value } = e.target;
    setForm((prev) => ({
      ...prev,
      [name]: name === "baseFare" ? Number(value) : value,
    }));
  }

  function validate() {
    const errors = {};
    if (!form.aircraftId) errors.aircraftId = "Aircraft is required.";
    if (!form.originAirportId)
      errors.originAirportId = "Origin airport is required.";
    if (!form.destinationAirportId)
      errors.destinationAirportId = "Destination airport is required.";
    if (
      form.originAirportId &&
      form.destinationAirportId &&
      form.originAirportId === form.destinationAirportId
    )
      errors.destinationAirportId =
        "Origin and destination cannot be the same.";
    if (!(Number(form.baseFare) > 0))
      errors.baseFare = "Base fare must be greater than 0.";
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
      aircraftId: Number(form.aircraftId),
      originAirportId: Number(form.originAirportId),
      destinationAirportId: Number(form.destinationAirportId),
      baseFare: Number(form.baseFare),
    };

    try {
      if (mode === "create") {
        await createRoute(payload);
        toast.success("Route added successfully!");
      } else {
        await updateRoute(editingId, payload);
        toast.success("Route updated successfully!");
      }
      await load();
      setModalOpen(false);
    } catch (e) {
      const msg =
        e?.response?.data?.message || e?.message || "Failed to save route.";
      setErr(msg);
      toast.error(msg);
    } finally {
      setSubmitting(false);
    }
  }

  async function onDelete(id) {
    if (!window.confirm("Delete this route?")) return;
    try {
      await deleteRoute(id);
      toast.success("Route deleted successfully!");
      await load();
    } catch (e) {
      const msg =
        e?.response?.data?.message || e?.message || "Failed to delete route.";
      setErr(msg);
      toast.error(msg);
    }
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-xl font-semibold">Routes</h1>
          <p className="text-sm text-gray-500">
            Manage flight routes between airports.
          </p>
        </div>
        <div className="flex gap-3">
          <input
            type="text"
            value={filter}
            onChange={(e) => setFilter(e.target.value)}
            placeholder="Search route..."
            className="w-64 border rounded-lg px-3 py-2 text-sm"
          />
          <button
            onClick={openCreate}
            className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700"
          >
            Add Route
          </button>
        </div>
      </div>

      {/* Error */}
      {err && <div className="p-3 bg-red-100 text-red-700 rounded">{err}</div>}

      {/* Table */}
      <div className="bg-white rounded-xl shadow">
        {loading ? (
          <div className="p-8 text-center text-gray-500">Loading routes…</div>
        ) : filtered.length === 0 ? (
          <div className="p-8 text-center text-gray-500">No routes found.</div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-gray-100 text-gray-700">
              <tr>
                <th className="px-4 py-2">ID</th>
                <th className="px-4 py-2">Aircraft</th>
                <th className="px-4 py-2">Origin</th>
                <th className="px-4 py-2">Destination</th>
                <td className="px-4 py-2">
                </td>

                <th className="px-4 py-2 text-right">Actions</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map((r) => (
                <tr key={r.routeId} className="border-b">
                  <td className="px-4 py-2">{r.routeId}</td>
                  <td className="px-4 py-2">{r.aircraft?.aircraftCode}</td>
                  <td className="px-4 py-2">{r.originAirport?.airportName}</td>
                  <td className="px-4 py-2">
                    {r.destinationAirport?.airportName}
                  </td>
                  <td className="px-4 py-2">
                    {formatCurrency(Number(r.baseFare), "INR")}
                  </td>
                  <td className="px-4 py-2 text-right">
                    <button
                      onClick={() => openEdit(r)}
                      className="mr-2 border px-3 py-1 rounded hover:bg-gray-50"
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => onDelete(r.routeId)}
                      className="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600"
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {/* Modal */}
      {modalOpen && (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
          <div className="bg-white rounded-xl shadow-lg w-full max-w-lg p-6">
            <h2 className="text-lg font-semibold mb-4">
              {mode === "create" ? "Add Route" : "Edit Route"}
            </h2>
            <form onSubmit={onSubmit} className="space-y-4">
              {/* Aircraft */}
              <div>
                <label className="block text-sm font-medium">Aircraft *</label>
                <select
                  name="aircraftId"
                  value={form.aircraftId}
                  onChange={onChange}
                  className="mt-1 w-full border rounded-lg px-3 py-2 text-sm"
                >
                  <option value="">Select aircraft</option>
                  {aircrafts.map((a) => (
                    <option key={a.aircraftId} value={a.aircraftId}>
                      {a.aircraftCode} - {a.aircraftName}
                    </option>
                  ))}
                </select>
                {vErrors.aircraftId && (
                  <p className="text-xs text-red-600">{vErrors.aircraftId}</p>
                )}
              </div>

              {/* Origin */}
              <div>
                <label className="block text-sm font-medium">
                  Origin Airport *
                </label>
                <select
                  name="originAirportId"
                  value={form.originAirportId}
                  onChange={onChange}
                  className="mt-1 w-full border rounded-lg px-3 py-2 text-sm"
                >
                  <option value="">Select origin</option>
                  {airports.map((a) => (
                    <option key={a.airportId} value={a.airportId}>
                      {a.code} - {a.airportName}
                    </option>
                  ))}
                </select>
                {vErrors.originAirportId && (
                  <p className="text-xs text-red-600">
                    {vErrors.originAirportId}
                  </p>
                )}
              </div>

              {/* Destination */}
              <div>
                <label className="block text-sm font-medium">
                  Destination Airport *
                </label>
                <select
                  name="destinationAirportId"
                  value={form.destinationAirportId}
                  onChange={onChange}
                  className="mt-1 w-full border rounded-lg px-3 py-2 text-sm"
                >
                  <option value="">Select destination</option>
                  {airports.map((a) => (
                    <option key={a.airportId} value={a.airportId}>
                      {a.code} - {a.airportName}
                    </option>
                  ))}
                </select>
                {vErrors.destinationAirportId && (
                  <p className="text-xs text-red-600">
                    {vErrors.destinationAirportId}
                  </p>
                )}
              </div>

              {/* Base Fare */}
              <div>
                <label className="block text-sm font-medium">Base Fare *</label>
                <div className="relative">
                  <span className="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3 text-gray-500">
                    ₹
                  </span>
                  <input
                    type="number"
                    min={1}
                    step="0.01"
                    name="baseFare"
                    value={form.baseFare}
                    onChange={onChange}
                    className="mt-1 w-full rounded-lg border border-gray-300 pl-7 pr-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  />
                </div>
                {vErrors.baseFare && (
                  <p className="text-xs text-red-600">{vErrors.baseFare}</p>
                )}
              </div>

              {/* Actions */}
              <div className="flex justify-end gap-3 pt-2">
                <button
                  type="button"
                  onClick={closeModal}
                  className="border px-4 py-2 rounded-lg"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={submitting}
                  className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700"
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
