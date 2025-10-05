export function formatCurrency(amount, currency = "INR") {
  if (amount == null) return "-";
  try {
    return new Intl.NumberFormat("en-IN", { style: "currency", currency }).format(amount);
  } catch {
    return `₹${amount.toLocaleString()}`;
  }
}

export function minutesToDuration(mins) {
  if (!mins && mins !== 0) return "-";
  const h = Math.floor(mins / 60);
  const m = mins % 60;
  return `${h}h ${m}m`;
}

export function shortTimeFromIso(iso) {
  if (!iso) return "-";
  const d = new Date(iso);
  return d.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
}
