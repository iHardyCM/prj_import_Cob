// GET /api/carteras
// src/api/client.js
// Si VITE_API no está definida en el build, usa misma-origen (""), o sea /api/... al mismo host
export const API = import.meta.env.VITE_API || "";

export async function get(path, params) {
  const qs = params ? `?${new URLSearchParams(params)}` : "";
  const res = await fetch(`${API}${path}${qs}`);
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}

export async function postJson(path, body) {
  const res = await fetch(`${API}${path}`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  });
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}

export async function postForm(path, formData) {
  const res = await fetch(`${API}${path}`, { method: "POST", body: formData });
  if (!res.ok) throw new Error(await res.text());
  return res.json();
}
