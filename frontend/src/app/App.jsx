import { useState } from "react"; 
import "./styles.css";
import ImporterPage from "../features/importer/ImporterPage";

export default function App() {
  const [collapsed, setCollapsed] = useState(false);

  return (
    <div className={`shell ${collapsed ? "collapsed" : ""}`}>
      <aside className="sidebar">
        <div className="brand">⊙ Importador</div>
        <nav>
          <a className="active">Cargas</a>
          <a>Reportes (próx.)</a>
          <a>Ajustes (próx.)</a>
        </nav>
      </aside>
      <main className="main">
        <div style={{display:"flex", justifyContent:"space-between", alignItems:"center", marginBottom:12}}>
          <button className="ghost" onClick={() => setCollapsed(v => !v)}>
            {collapsed ? "☰ Mostrar menú" : "☰ Ocultar menú"}
          </button>
        </div>
        <ImporterPage />
      </main>
    </div>
  );
}

