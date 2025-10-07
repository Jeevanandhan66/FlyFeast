import { useEffect, useState, useMemo } from "react";
import { useToast } from "../../hooks/useToast";
import {
  getPayments,
  createPayment,
  updatePayment,
  deletePayment,
} from "../../services/adminService";
import { formatCurrency } from "../../utils/formatters";

function initialForm() {
  return {
    bookingId: "",
    amount: "",
    provider: "",
    userId: "",
    status: "Initiated",
  };
}

export default function Payments() {
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
    return list.filter((p) =>
      `${p.provider} ${p.status} ${p.booking?.bookingRef}`.toLowerCase().includes(f)
    );
  }, [list, filter]);

  async function load() {
    try {
      setLoading(true);
      setErr("");
      const data = await getPayments();
      setList(Array.isArray(data) ? data : []);
    } catch (e) {
      setErr(e?.response?.data?.error || e.message || "Failed to load payments.");
      toast.error("Failed to load payments.");
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
    setEditingId(item.paymentId);
    setForm({
      bookingId: item.booking?.bookingId || "",
      amount: item.amount,
      provider: item.provider || "",
      userId: item.booking?.user?.userId || "",
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

  async function onSubmit(e) {
    e.preventDefault();
    setSubmitting(true);
    setErr("");

    try {
      const payload = {
        bookingId: Number(form.bookingId),
        amount: parseFloat(form.amount),
        provider: form.provider,
        userId: form.userId,
        status: form.status,
      };

      if (mode === "create") {
        await createPayment(payload);
        toast.success("Payment created successfully!");
      } else {
        await updatePayment(editingId, payload);
        toast.success("Payment updated successfully!");
      }
      await load();
      setModalOpen(false);
    } catch (e) {
      setErr(e?.response?.data?.error || e.message || "Failed to save payment.");
      toast.error("Error saving payment.");
    } finally {
      setSubmitting(false);
    }
  }

  async function onDelete(id) {
    if (!window.confirm("Delete this payment?")) return;
    try {
      await deletePayment(id);
      await load();
      toast.success("Payment deleted.");
    } catch (e) {
      toast.error(e?.response?.data?.error || "Delete failed.");
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <input
          type="text"
          placeholder="Search payments..."
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
          className="border rounded-lg px-3 py-2 text-sm w-64 focus:ring-2 focus:ring-indigo-500 focus:outline-none"
        />
        <button
          onClick={openCreate}
          className="bg-indigo-600 text-white px-5 py-2 rounded-lg shadow hover:bg-indigo-700 transition"
        >
          + Add Payment
        </button>
      </div>

      {loading ? (
        <p>Loading...</p>
      ) : err ? (
        <p className="text-red-500">{err}</p>
      ) : (
        <div className="overflow-x-auto rounded-lg shadow">
          <table className="w-full text-sm text-left text-gray-600">
            <thead>
              <tr className="bg-gray-100 text-gray-700 text-sm uppercase">
                <th className="px-4 py-3">Booking Ref</th>
                <th className="px-4 py-3">Amount</th>
                <th className="px-4 py-3">Provider</th>
                <th className="px-4 py-3">Provider Ref</th>
                <th className="px-4 py-3">Status</th>
                <th className="px-4 py-3">Created</th>
                <th className="px-4 py-3 text-right">Actions</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map((p) => (
                <tr
                  key={p.paymentId}
                  className="border-b hover:bg-gray-50 transition"
                >
                  <td className="px-4 py-2">{p.booking?.bookingRef}</td>
                  <td className="px-4 py-2 font-medium">
                    {formatCurrency(p.amount)}
                  </td>
                  <td className="px-4 py-2">{p.provider}</td>
                  <td className="px-4 py-2">{p.providerRef}</td>
                  <td className="px-4 py-2">
                    <span
                      className={`px-2 py-1 rounded-full text-xs font-semibold ${
                        p.status === "Success"
                          ? "bg-green-100 text-green-700"
                          : p.status === "Failed"
                          ? "bg-red-100 text-red-700"
                          : "bg-yellow-100 text-yellow-700"
                      }`}
                    >
                      {p.status}
                    </span>
                  </td>
                  <td className="px-4 py-2 text-gray-500">
                    {new Date(p.createdAt).toLocaleString()}
                  </td>
                  <td className="px-4 py-2 text-right space-x-2">
                    <button
                      onClick={() => openEdit(p)}
                      className="px-3 py-1 bg-blue-100 text-blue-700 rounded hover:bg-blue-200 text-xs font-medium transition"
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => onDelete(p.paymentId)}
                      className="px-3 py-1 bg-red-100 text-red-700 rounded hover:bg-red-200 text-xs font-medium transition"
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
          <div className="bg-white rounded-xl shadow-xl p-6 w-full max-w-lg">
            <h2 className="text-lg font-semibold mb-4">
              {mode === "create" ? "Add Payment" : "Edit Payment"}
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
                  className="w-full border rounded-lg px-3 py-2 focus:ring-2 focus:ring-indigo-500 focus:outline-none"
                />
              </div>
              <div>
                <label className="block text-sm font-medium">User ID *</label>
                <input
                  name="userId"
                  value={form.userId}
                  onChange={onChange}
                  required
                  className="w-full border rounded-lg px-3 py-2 focus:ring-2 focus:ring-indigo-500 focus:outline-none"
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
                  className="w-full border rounded-lg px-3 py-2 focus:ring-2 focus:ring-indigo-500 focus:outline-none"
                />
              </div>
              <div>
                <label className="block text-sm font-medium">Provider</label>
                <input
                  name="provider"
                  value={form.provider}
                  onChange={onChange}
                  className="w-full border rounded-lg px-3 py-2 focus:ring-2 focus:ring-indigo-500 focus:outline-none"
                />
              </div>
              <div>
                <label className="block text-sm font-medium">Status</label>
                <select
                  name="status"
                  value={form.status}
                  onChange={onChange}
                  className="w-full border rounded-lg px-3 py-2 focus:ring-2 focus:ring-indigo-500 focus:outline-none"
                >
                  <option value="Initiated">Initiated</option>
                  <option value="Success">Success</option>
                  <option value="Failed">Failed</option>
                </select>
              </div>

              <div className="flex justify-end space-x-3 pt-4">
                <button
                  type="button"
                  onClick={closeModal}
                  className="px-4 py-2 border rounded-lg hover:bg-gray-100 transition"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={submitting}
                  className="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition"
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
