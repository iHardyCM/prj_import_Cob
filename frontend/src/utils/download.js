// src/utils/download.js
function jsonToCsv(data) {
  if (!Array.isArray(data) || data.length === 0) return "";
  const headers = Object.keys(data[0]);
  const esc = v => `"${String(v ?? "").replace(/"/g, '""')}"`;
  const rows = data.map(r => headers.map(h => esc(r[h])).join(","));
  return [headers.join(","), ...rows].join("\r\n");
}

function downloadBlob(blob, filename) {
  const link = URL.createObjectURL(blob);
  const a = Object.assign(document.createElement("a"), { href: link, download: filename });
  document.body.appendChild(a); a.click(); a.remove(); URL.revokeObjectURL(link);
}

// Asegura que la petición llegue autenticada (si usas cookies/JWT en cookie)
export async function download(url, fallbackName = "archivo.csv") {
  const res = await fetch(url, {
    credentials: "include",
    headers: { Accept: "text/csv, application/json;q=0.9, */*;q=0.5" }
  });

  // Si el server respondió HTML (SPAs, 401 con login page, 404 con index.html)
  const ct = (res.headers.get("content-type") || "").toLowerCase();
  if (ct.includes("text/html")) {
    const html = await res.text();
    throw new Error(`La exportación devolvió HTML (posible SPA/redirect o error). Status ${res.status}. Revisa la URL/proxy/auth.\nPreview: ${html.slice(0,200)}...`);
  }

  // Nombre de archivo (si el backend lo manda)
  const cd = res.headers.get("Content-Disposition") || "";
  const m = cd.match(/filename\*?=(?:UTF-8'')?["']?([^"';]+)["']?/i);
  const name = m ? decodeURIComponent(m[1]) : fallbackName;

  // 1) CSV directo
  if (ct.includes("text/csv") || ct.includes("application/vnd.ms-excel")) {
    const blob = await res.blob();
    return downloadBlob(blob, name);
  }

  // 2) JSON -> CSV
  if (ct.includes("application/json") || ct.includes("application/problem+json")) {
    const data = await res.json();
    if (res.ok) {
      const csv = "\uFEFF" + jsonToCsv(data);
      const blob = new Blob([csv], { type: "text/csv;charset=utf-8" });
      return downloadBlob(blob, name.endsWith(".csv") ? name : `${name}.csv`);
    } else {
      // Error JSON del backend
      throw new Error(typeof data?.message === "string" ? data.message : JSON.stringify(data));
    }
  }

  // Fallback: tratar como binario si no hay content-type claro
  const blob = await res.blob();
  return downloadBlob(blob, name);
}
