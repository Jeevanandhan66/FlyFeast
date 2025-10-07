import { useEffect, useState, useMemo } from "react";
import { useToast } from "../../hooks/useToast";
import {
  getRefunds,
  createRefund,
  updateRefund,
  deleteRefund,
} from "../../services/adminService";
import { formatCurrency } from "../../utils/formatters";

function initialForm() {
  return {
    bookingId: "",
    amount: "",
    status: "Initiated",
    processedById: "",
  };
}

export default function Refunds() {
  const toast = useToast();
  const [list, setList] = useState([]);
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
    return list.filter((r) =>
      `${r.booking?.bookingRef} ${r.status} ${r.processedUser?.fullName}`
        .toLowerCase()
        .includes(f)
    );
  }, [list, filter]);

  async function load() {
    try {
      setLoading(true);
      setErr("");
      const data = await getRefunds();
      setList(Array.isArray(data) ? data : []);
    } catch (e) {
      setErr(e?.response?.data?.error || e.message || "Failed to load refunds.");
      toast.error("Failed to load refunds.");
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
    setEditingId(item.refundId);
    setForm({
      bookingId: item.booking?.bookingId || "",
      amount: item.amount,
      status: item.status,
      processedById: item.processedUser?.userId || "",
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
      const payload = {
        bookingId: Number(form.bookingId),
        amount: parseFloat(form.amount),
        status: form.status,
        processedById: form.processedById,
      };

      if (mode === "create") {
        await createRefund(payload);
        toast.success("Refund created successfully!");
      } else {
        await updateRefund(editingId, payload);
        toast.success("Refund updated successfully!");
      }
      await load();
      setModalOpen(false);
    } catch (e) {
      setErr(e?.response?.data?.error || e.message || "Failed to save refund.");
      toast.error("Error saving refund.");
    } finally {
      setSubmitting(false);
    }
  }

  async function onDelete(id) {
    if (!window.confirm("Delete this refund?")) return;
    try {
      await deleteRefund(id);
      await load();
      toast.success("Refund deleted.");
    } catch (e) {
      toast.error(e?.response?.data?.error || "Delete failed.");
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between">
        <input
          type="text"
          placeholder="Search refunds..."
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
          className="border rounded px-3 py-2 text-sm"
        />
        <button
          onClick={openCreate}
          className="bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700"
        >
          + Add Refund
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
                <th className="px-4 py-2">Booking Ref</th>
                <th className="px-4 py-2">Amount</th>
                <th className="px-4 py-2">Status</th>
                <th className="px-4 py-2">Processed By</th>
                <th className="px-4 py-2">Created</th>
                <th className="px-4 py-2">Actions</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map((r) => (
                <tr key={r.refundId} className="border-b">
                  <td className="px-4 py-2">{r.booking?.bookingRef}</td>
                  <td className="px-4 py-2">{formatCurrency(r.amount)}</td>
                  <td className="px-4 py-2">
                    <span
                      className={`px-2 py-1 rounded-full text-xs font-medium ${
                        r.status === "Processed"
                          ? "bg-green-100 text-green-700"
                          : r.status === "Failed"
                          ? "bg-red-100 text-red-700"
                          : "bg-yellow-100 text-yellow-700"
                      }`}
                    >
                      {r.status}
                    </span>
                  </td>
                  <td className="px-4 py-2">{r.processedUser?.fullName}</td>
                  <td className="px-4 py-2">
                    {new Date(r.createdAt).toLocaleString()}
                  </td>
                  <td className="px-4 py-2 space-x-2">
                    <button
                      onClick={() => openEdit(r)}
                      className="text-blue-600 hover:underline"
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => onDelete(r.refundId)}
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
              {mode === "create" ? "Add Refund" : "Edit Refund"}
            </h2>
            <form onSubmit={onSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium">Booking ID *</label>
                <input
                  type="number"
                  name="bookingId"
                  value={form.bookingId}
                  onChange={onChange}
                  required
                  className="w-full border rounded px-3 py-2"
                />
              </div>
              <div>
                <label className="block text-sm font-medium">Amount *</label>
                <input
                  type="number"
                  min="0"
                  step="0.01"
                  name="amount"
                  value={form.amount}
                  onChange={onChange}
                  required
                  className="w-full border rounded px-3 py-2"
                />
              </div>
              <div>
                <label className="block text-sm font-medium">Processed By *</label>
                <input
                  name="processedById"
                  value={form.processedById}
                  onChange={onChange}
                  required
                  className="w-full border rounded px-3 py-2"
                />
              </div>
              <div>
                <label className="block text-sm font-medium">Status</label>
                <select
                  name="status"
                  value={form.status}
                  onChange={onChange}
                  className="w-full border rounded px-3 py-2"
                >
                  <option value="Initiated">Initiated</option>
                  <option value="Processed">Processed</option>
                  <option value="Failed">Failed</option>
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
