import { useState } from "react";
import "./styles.css";
import ImporterPage from "../features/importer/ImporterPage";

export default function App(){
  const [collapsed, setCollapsed] = useState(false);
  return (
    <div className={`shell ${collapsed ? "collapsed" : ""}`}>
      <aside className="sidebar">
        <div className="brand">Opciones</div>
        <nav>
          <a className="active">Canales Alternos</a>
          <a>Reportes (próx.)</a>
          <a>Ajustes (próx.)</a>
        </nav>
      </aside>

      <main className="main">
        <ImporterPage
          collapsed={collapsed}
          onToggle={() => setCollapsed(v => !v)}
        />
      </main>
    </div>
  );
}
