import { useEffect, useState, useMemo } from "react";
import { useToast } from "../../hooks/useToast";
import {
  getSchedules,
  createSchedule,
  updateSchedule,
  deleteSchedule,
  getRoutes,
} from "../../services/adminService";

function initialForm() {
  return {
    routeId: "",
    departureTime: "",
    arrivalTime: "",
    seatCapacity: 1,
    status: "Scheduled",
  };
}

const STATUS_OPTIONS = ["Scheduled", "Delayed", "Cancelled", "Completed"];

export default function Schedules() {
  const toast = useToast();

  const [list, setList] = useState([]);
  const [routes, setRoutes] = useState([]);
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
    return list.filter((s) =>
      `${s.route.originAirport.airportName} ${s.route.destinationAirport.airportName}`
        .toLowerCase()
        .includes(f)
    );
  }, [list, filter]);

  async function load() {
    try {
      setLoading(true);
      setErr("");
      const [schedulesData, routesData] = await Promise.all([
        getSchedules(),
        getRoutes(),
      ]);
      setList(schedulesData || []);
      setRoutes(routesData || []);
    } catch (e) {
      setErr(
        e?.response?.data?.message || e?.message || "Failed to load schedules."
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
    setEditingId(item.scheduleId);
    setForm({
      routeId: item.route.routeId,
      departureTime: item.departureTime?.slice(0, 16), // format for input
      arrivalTime: item.arrivalTime?.slice(0, 16),
      seatCapacity: item.seatCapacity,
      status: item.status,
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
    if (!form.routeId) errors.routeId = "Route is required.";
    if (!form.departureTime)
      errors.departureTime = "Departure time is required.";
    if (!form.arrivalTime) errors.arrivalTime = "Arrival time is required.";
    if (new Date(form.arrivalTime) <= new Date(form.departureTime)) {
      errors.arrivalTime = "Arrival must be after departure.";
    }
    if (!(Number(form.seatCapacity) > 0))
      errors.seatCapacity = "Seats must be > 0.";
    if (!STATUS_OPTIONS.includes(form.status))
      errors.status = "Invalid status.";
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
      routeId: Number(form.routeId),
      departureTime: form.departureTime + ":00",
      arrivalTime: form.arrivalTime + ":00",
      seatCapacity: Number(form.seatCapacity),
      status: form.status,
    };

    try {
      if (mode === "create") {
        await createSchedule(payload);
        toast.success("Schedule added successfully!");
      } else {
        await updateSchedule(editingId, payload);
        toast.success("Schedule updated successfully!");
      }
      await load();
      setModalOpen(false);
    } catch (e) {
      const msg =
        e?.response?.data?.message || e?.message || "Failed to save schedule.";
      setErr(msg);
      toast.error(msg);
    } finally {
      setSubmitting(false);
    }
  }

  async function onDelete(id) {
    if (!window.confirm("Delete this schedule?")) return;
    try {
      await deleteSchedule(id);
      toast.success("Schedule deleted successfully!");
      await load();
    } catch (e) {
      const msg =
        e?.response?.data?.error || e?.message || "Failed to delete schedule.";
      setErr(msg);
      toast.error(msg);
    }
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-xl font-semibold">Schedules</h1>
          <p className="text-sm text-gray-500">
            Manage schedules for routes and flights.
          </p>
        </div>
        <div className="flex gap-3">
          <input
            type="text"
            value={filter}
            onChange={(e) => setFilter(e.target.value)}
            placeholder="Search by origin/destination"
            className="w-64 border rounded-lg px-3 py-2 text-sm"
          />
          <button
            onClick={openCreate}
            className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700"
          >
            Add Schedule
          </button>
        </div>
      </div>

      {/* Error */}
      {err && <div className="p-3 bg-red-100 text-red-700 rounded">{err}</div>}

      {/* Table */}
      <div className="bg-white rounded-xl shadow">
        {loading ? (
          <div className="p-8 text-center text-gray-500">
            Loading schedules…
          </div>
        ) : filtered.length === 0 ? (
          <div className="p-8 text-center text-gray-500">
            No schedules found.
          </div>
        ) : (
          <table className="w-full text-sm">
            <thead className="bg-gray-100 text-gray-700">
              <tr>
                <th className="px-4 py-2">ID</th>
                <th className="px-4 py-2">Route</th>
                <th className="px-4 py-2">Departure</th>
                <th className="px-4 py-2">Arrival</th>
                <th className="px-4 py-2">Seats</th>
                <th className="px-4 py-2">Status</th>
                <th className="px-4 py-2 text-right">Actions</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map((s) => (
                <tr key={s.scheduleId} className="border-b">
                  <td className="px-4 py-2">{s.scheduleId}</td>
                  <td className="px-4 py-2">
                    {s.route.originAirport.code} →{" "}
                    {s.route.destinationAirport.code}
                  </td>
                  <td className="px-4 py-2">
                    {new Date(s.departureTime).toLocaleString()}
                  </td>
                  <td className="px-4 py-2">
                    {new Date(s.arrivalTime).toLocaleString()}
                  </td>
                  <td className="px-4 py-2">{s.seatCapacity}</td>
                  <td className="px-4 py-2">{s.status}</td>
                  <td className="px-4 py-2 text-right">
                    <button
                      onClick={() => openEdit(s)}
                      className="mr-2 border px-3 py-1 rounded hover:bg-gray-50"
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => onDelete(s.scheduleId)}
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
              {mode === "create" ? "Add Schedule" : "Edit Schedule"}
            </h2>
            <form onSubmit={onSubmit} className="space-y-4">
              {/* Route */}
              <div>
                <label className="block text-sm font-medium">Route *</label>
                <select
                  name="routeId"
                  value={form.routeId}
                  onChange={onChange}
                  className="mt-1 w-full border rounded-lg px-3 py-2 text-sm"
                >
                  <option value="">Select route</option>
                  {routes.map((r) => (
                    <option key={r.routeId} value={r.routeId}>
                      {r.originAirport.code} → {r.destinationAirport.code} (
                      {r.aircraft.aircraftCode})
                    </option>
                  ))}
                </select>
                {vErrors.routeId && (
                  <p className="text-xs text-red-600">{vErrors.routeId}</p>
                )}
              </div>

              {/* Departure */}
              <div>
                <label className="block text-sm font-medium">
                  Departure Time *
                </label>
                <input
                  type="datetime-local"
                  name="departureTime"
                  value={form.departureTime}
                  min={new Date().toISOString().slice(0, 16)}
                  onChange={onChange}
                  className="mt-1 w-full border rounded-lg px-3 py-2 text-sm"
                />
                {vErrors.departureTime && (
                  <p className="text-xs text-red-600">
                    {vErrors.departureTime}
                  </p>
                )}
              </div>

              {/* Arrival */}
              <div>
                <label className="block text-sm font-medium">
                  Arrival Time *
                </label>
                <input
                  type="datetime-local"
                  name="arrivalTime"
                  value={form.arrivalTime}
                  min={new Date().toISOString().slice(0, 16)}
                  onChange={onChange}
                  className="mt-1 w-full border rounded-lg px-3 py-2 text-sm"
                />
                {vErrors.arrivalTime && (
                  <p className="text-xs text-red-600">{vErrors.arrivalTime}</p>
                )}
              </div>

              {/* Seats */}
              <div>
                <label className="block text-sm font-medium">
                  Seat Capacity *
                </label>
                <input
                  type="number"
                  min={1}
                  name="seatCapacity"
                  value={form.seatCapacity}
                  onChange={onChange}
                  className="mt-1 w-full border rounded-lg px-3 py-2 text-sm"
                />
                {vErrors.seatCapacity && (
                  <p className="text-xs text-red-600">{vErrors.seatCapacity}</p>
                )}
              </div>

              {/* Status */}
              <div>
                <label className="block text-sm font-medium">Status *</label>
                <select
                  name="status"
                  value={form.status}
                  onChange={onChange}
                  className="mt-1 w-full border rounded-lg px-3 py-2 text-sm"
                >
                  {STATUS_OPTIONS.map((opt) => (
                    <option key={opt} value={opt}>
                      {opt}
                    </option>
                  ))}
                </select>
                {vErrors.status && (
                  <p className="text-xs text-red-600">{vErrors.status}</p>
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
