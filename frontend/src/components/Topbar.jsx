import React from "react";

export default function Topbar({
  collapsed,
  onToggle,
  title = "Cargas de Canales Alternos",
  subtitle,
  channel = "sms",
  channels = [],              // [{label, value}]
  onChannelChange = () => {},
  onSearch = () => {},
  onImport = () => {},
}) {
  return (
    <header className="topbar">
      <div className="left">
        <button className="btn icon" aria-label="Mostrar/Ocultar menú" onClick={onToggle}>
          <svg width="22" height="22" viewBox="0 0 24 24" aria-hidden="true">
            <path d="M3 6h18M3 12h18M3 18h18" stroke="currentColor" strokeWidth="2" strokeLinecap="round"/>
          </svg>
        </button>

        <div className="headline">
          <h1>{title}</h1>
          <nav className="crumbs">
            <span>Opciones</span><span className="sep">/</span><span className="current">Cargas</span>
            {subtitle && <span className="sep">•</span>}
            {subtitle && <span className="current">{subtitle}</span>}
          </nav>
        </div>
      </div>

      <div className="right">
        <label className="field search" aria-label="Buscar">
          <svg width="18" height="18" viewBox="0 0 24 24" aria-hidden="true">
            <path d="M21 21l-4.3-4.3M10.5 18a7.5 7.5 0 1 1 0-15 7.5 7.5 0 0 1 0 15z"
              stroke="currentColor" strokeWidth="2" fill="none" strokeLinecap="round"/>
          </svg>
          <input placeholder="Buscar archivo o lote…" onChange={(e)=>onSearch(e.target.value)}/>
        </label>

        <select className="select" value={channel} onChange={(e)=>onChannelChange(e.target.value)}>
          {(channels.length?channels:[{label:"SMS",value:"sms"},{label:"Email",value:"email"},{label:"IVR",value:"ivr"}])
            .map(o=> <option key={o.value} value={o.value}>{o.label}</option>)}
        </select>

        <button className="btn primary" onClick={onImport}>
          <svg width="18" height="18" viewBox="0 0 24 24" aria-hidden="true" style={{marginRight:6}}>
            <path d="M12 5v14M5 12h14" stroke="currentColor" strokeWidth="2" strokeLinecap="round"/>
          </svg>
          Importar 
        </button>
      </div>
    </header>
  );
}
