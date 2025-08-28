// popup para importar
import { useState } from "react";
import Modal from "../../components/Modal";
import { CHANNELS, templateByChannel } from "./channels";
import { importFile } from "../../api/import";
import { API } from "../../api/client";

export default function ImportModal({ carteras, onClose, onImported }) {
  const [channel, setChannel]   = useState("sms");
  const [idCartera, setCartera] = useState("");
  const [file, setFile]         = useState(null);
  const [usuario, setUsuario]   = useState("");
  const [busy, setBusy]         = useState(false);
  const hint = CHANNELS.find(c => c.value === channel)?.hint;
  const [downloading, setDownloading] = useState(false);

  // const downloadTemplate = () => {
  //   const blob = new Blob([templateByChannel[channel]], { type:"text/csv;charset=utf-8" });
  //   const a = document.createElement("a");
  //   a.href = URL.createObjectURL(blob);
  //   a.download = `plantilla_${channel}.csv`;
  //   a.click(); URL.revokeObjectURL(a.href);
  // };

  // const downloadTemplate = () => {
  //   window.open(`/api/templates/import?channel=${channel}`, "_blank");
  // };

  const downloadTemplate = async () => {
    try {
      setDownloading(true);
      const res = await fetch(`/api/templates/import?channel=${channel}`);
      if (!res.ok) throw new Error(await res.text());
      const cd = res.headers.get("Content-Disposition") || "";
      const m = cd.match(/filename\*?=(?:UTF-8'')?["']?([^"';]+)["']?/i);
      const name = m ? decodeURIComponent(m[1]) : `plantilla_${channel}.xlsx`;
      const blob = await res.blob();
      const url = URL.createObjectURL(blob);
      const a = Object.assign(document.createElement("a"), { href: url, download: name });
      document.body.appendChild(a); a.click(); a.remove(); URL.revokeObjectURL(url);
    } catch (e) {
      alert("No se pudo descargar la plantilla: " + e.message);
    } finally {
      setDownloading(false);
    }
  };

  const submit = async (e) => {
    e.preventDefault();
    if (!idCartera || !file) return;
    setBusy(true);
    try {
      await importFile(channel, { idCartera, file, usuario });
      onImported?.();
    } catch (err) {
      alert(`Error importando: ${err.message}`);
    } finally { setBusy(false); }
  };

  return (
    <Modal title="Importar" onClose={onClose}>
      <form onSubmit={submit} style={{display:"grid", gap:12}}>
        <label>Canal</label>
        <select value={channel} onChange={e=>setChannel(e.target.value)}>
          {CHANNELS.map(c => <option key={c.value} value={c.value}>{c.label}</option>)}
        </select>

        <label>Cartera</label>
        <select value={idCartera} onChange={e=>setCartera(e.target.value)}>
          <option value="" disabled>Selecciona…</option>
          {carteras.map(c => <option key={c.idCartera} value={c.idCartera}>{c.nombre} ({c.idCartera})</option>)}
        </select>

        <label>Archivo (.xlsx)</label>
        <input type="file" accept=".xlsx" onChange={e=>setFile(e.target.files?.[0] ?? null)} />

        <label>Usuario (opcional)</label>
        <input value={usuario} onChange={e=>setUsuario(e.target.value)} placeholder="hcruz" />

        <div style={{fontSize:12, opacity:.8}}>{hint}</div>

        <div style={{display:"flex", justifyContent:"space-between", marginTop:8}}>
          <button type="button" onClick={downloadTemplate} disabled={downloading}>
            {downloading ? "Descargando…" : "Descargar plantilla"}
          </button>
          <div style={{display:"flex", gap:8}}>
            <button type="button" onClick={onClose}>Cancelar</button>
            <button type="submit" disabled={!idCartera || !file || busy}>
              {busy ? "Importando…" : "Importar"}
            </button>
          </div>
        </div>
      </form>
    </Modal>
  );
}
