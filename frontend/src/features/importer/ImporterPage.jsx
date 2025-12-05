import { useEffect, useMemo, useState } from "react";
import { fetchCarteras } from "../../api/carteras";
import { fetchLoads } from "../../api/loads";
import { CHANNELS } from "./channels";       // [{label,value}]
import ImportModal from "./ImportModal";
import SummaryTable from "./SummaryTable";
import Topbar from "../../components/Topbar";
import Alert from "../../components/Alert";

export default function ImporterPage({ collapsed=false, onToggle=()=>{} }){
  const [carteras, setCarteras] = useState([]);
  const [rows, setRows] = useState([]);
  const [channel, setChannel] = useState("sms");
  const [open, setOpen] = useState(false);
  const [query, setQuery] = useState("");
  const [successMsg, setSuccessMsg] = useState("");

  useEffect(() => { fetchCarteras().then(setCarteras).catch(console.error); }, []);

  const loadSummary = async () => {
    try{
      const data = await fetchLoads({ channel });
      setRows(Array.isArray(data) ? data : []);
    }catch(e){
      console.error("Error /api/loads:", e);
      setRows([]);
    }
  };
  useEffect(() => { loadSummary(); }, [channel]);

  const filtered = useMemo(() => {
    const q = (query||"").trim().toLowerCase();
    if(!q) return rows;
    return rows.filter(r=>{
      const archivo = String(r?.fileName ?? r?.nombreArchivo ?? "").toLowerCase();
      const lote    = String(r?.batchId  ?? r?.lote           ?? "").toLowerCase();
      const cartera = String(r?.cartera  ?? r?.idCartera      ?? "").toLowerCase();
      return archivo.includes(q) || lote.includes(q) || cartera.includes(q);
    });
  }, [rows, query]);
  const total = rows.length;
  const shown = filtered.length;

  const norm = (s) =>
  String(s ?? "")
    .normalize("NFD").replace(/\p{Diacritic}/gu, "") // quita acentos
    .toLowerCase()
    .trim();


  return (
    <div style={{display:"grid", gap:16}}>
      <Topbar
        collapsed={collapsed}
        onToggle={onToggle}
        title="Cargas de Canales Alternos"
        channel={channel}
        channels={CHANNELS}
        onChannelChange={setChannel}
        onSearch={setQuery}
        onImport={()=>setOpen(true)}

        subtitle={shown === total ? `${total.toLocaleString()} cargas` 
                            : `Mostrando ${shown.toLocaleString()} de ${total.toLocaleString()}`}
      />

      {successMsg && (
        <Alert kind="success" onClose={()=>setSuccessMsg("")}>
          {successMsg}
        </Alert>
      )}

      <section style={{background:"#fff", borderRadius:16, padding:16, boxShadow:"0 6px 18px rgba(0,0,0,.06)"}}>
        <SummaryTable rows={filtered} />
      </section>

      {open && (
        <ImportModal
          carteras={carteras}
          onClose={()=>setOpen(false)}
          onImported={(fileName)=>{
            setOpen(false);
            loadSummary();
            setSuccessMsg(fileName ? `✅ “${fileName}” importado correctamente.` : "✅ Archivo importado correctamente.");
          }}
        />
      )}
    </div>
  );
}
