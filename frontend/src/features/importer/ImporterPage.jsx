// página del módulo
import { useEffect, useState } from "react";
import { fetchCarteras } from "../../api/carteras";
import { fetchLoads } from "../../api/loads";
import { CHANNELS } from "./channels";
import ImportModal from "./ImportModal";
import SummaryTable from "./SummaryTable";

export default function ImporterPage() {
  const [carteras, setCarteras] = useState([]);
  const [rows, setRows] = useState([]);
  const [channel, setChannel] = useState("sms");
  const [open, setOpen] = useState(false);

  useEffect(() => { fetchCarteras().then(setCarteras).catch(console.error); }, []);
  const loadSummary = async () => {
  try {
    const data = await fetchLoads({ channel });
    setRows(Array.isArray(data) ? data : []);   // ⬅️ evita rows.map is not a function
    } catch (err) {
      console.error("Error /api/loads:", err);
      setRows([]);
    }
  };
  useEffect(() => { loadSummary(); }, [channel]);

  return (
    <div style={{display:"grid", gap:16}}>
      <header style={{display:"flex", justifyContent:"space-between", alignItems:"center"}}>
        <h2 style={{margin:0}}>Cargas de campañas</h2>
        <div style={{display:"flex", gap:8, alignItems:"center"}}>
          <select value={channel} onChange={e=>setChannel(e.target.value)}>
            {CHANNELS.map(c => <option key={c.value} value={c.value}>{c.label}</option>)}
          </select>
          <button onClick={()=>setOpen(true)}>+ Importar</button>
        </div>
      </header>

      <section style={{background:"#fff", borderRadius:16, padding:16, boxShadow:"0 6px 18px rgba(0,0,0,.06)"}}>
        <SummaryTable rows={rows} />
      </section>

      {open && (
        <ImportModal
          carteras={carteras}
          onClose={()=>setOpen(false)}
          onImported={()=>{ setOpen(false); loadSummary(); }}
        />
      )}
    </div>
  );
}
