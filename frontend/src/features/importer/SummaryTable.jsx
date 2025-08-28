// src/features/importer/SummaryTable.jsx
import Badge from "../../components/Badge";
import { download } from "../../utils/download";
import { exportLoadUrl } from "../../api/loads";

// formateo robusto
const fmt = (val) => {
  if (!val) return "—";
  let s = val;
  if (typeof s === "string" && /^\d{4}-\d{2}-\d{2}\s\d/.test(s)) s = s.replace(" ", "T");
  const d = new Date(s);
  if (isNaN(d.getTime())) return String(val);
  return new Intl.DateTimeFormat("es-PE", {
    dateStyle: "short",
    timeStyle: "medium",
    timeZone: "America/Lima",
  }).format(d);
};


// SummaryTable.jsx
export default function SummaryTable({ rows, onExport }) {
  return (
    <div style={{maxHeight: 420, overflowY: "auto", borderRadius: 8, border:"1px solid #e5e7eb"}}>
      <table style={{width:"100%", borderCollapse:"separate", borderSpacing:0}}>
        <thead style={{position:"sticky", top:0, zIndex:1}}>
          <tr style={{background:"#fff", boxShadow:"0 1px 0 #eee"}}>
            <th>Canal</th>
            <th>Cartera</th>
            <th>Lote</th>
            <th>Archivo</th>
            <th style={{textAlign:"right"}}>Filas</th>
            <th>Rango</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {rows.length === 0 && (
            <tr><td colSpan={7} style={{opacity:.7, padding:12}}>Sin cargas aún.</td></tr>
          )}
          {rows.map(r => (
            <tr key={`${r.canal}-${r.lote}`}>
              <td><span className="badge">{r.canal}</span></td>
              <td>{r.cartera ?? r.idCartera}</td>
              <td style={{fontFamily:"ui-monospace,Menlo,monospace"}}>{r.lote}</td>
              <td title={r.nombreArchivo}>{r.nombreArchivo}</td>
              <td style={{textAlign:"right"}}>{Number(r.filas).toLocaleString()}</td>
              <td>{formatRange(r.minFecha, r.maxFecha)}</td>
              <td>
                <button
                  type="button"
                  onClick={() =>
                    download(
                      exportLoadUrl(r.canal, r.lote, "csv"),
                      `${r.canal}_lote_${r.lote}.csv`
                    ).catch(e => { alert(e.message); console.error(e); })
                  }
                >
                Exportar CSV
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

// util: evita "Invalid Date" y formatea bonito
function formatRange(a, b) {
  const da = a ? new Date(a) : null;
  const db = b ? new Date(b) : null;
  const okA = da && !isNaN(da);
  const okB = db && !isNaN(db);
  if (okA && okB) return `${da.toLocaleString()} — ${db.toLocaleString()}`;
  if (okA) return da.toLocaleString();
  if (okB) return db.toLocaleString();
  return "—";
}

