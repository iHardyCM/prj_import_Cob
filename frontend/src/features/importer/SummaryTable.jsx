import { download } from "../../utils/download";
import { exportLoadUrl } from "../../api/loads";
import { formatRange } from "../../utils/dates";

export default function SummaryTable({ rows }){
  return (
    <div style={{maxHeight:420, overflowY:"auto", borderRadius:8, border:"1px solid #e5e7eb"}}>
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
          {rows.length===0 && (
            <tr><td colSpan={7} style={{opacity:.7, padding:12}}>Sin cargas aún.</td></tr>
          )}
          {rows.map(r=>(
            <tr key={`${r.canal}-${r.lote}`}>
              <td><span className="badge">{r.canal}</span></td>
              <td>{r.cartera ?? r.idCartera}</td>
              <td style={{fontFamily:"ui-monospace,Menlo,monospace"}}>{r.lote}</td>
              <td title={r.nombreArchivo}>{r.nombreArchivo}</td>
              <td style={{textAlign:"right"}}>{Number(r.filas).toLocaleString()}</td>
              <td>{formatRange(r.minFecha, r.maxFecha, { withSeconds:false })}</td>
              <td>
                <button
                  className="btn ghost"
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
