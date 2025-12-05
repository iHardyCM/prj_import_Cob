import React, { useEffect } from "react";

export default function Alert({ kind="success", children, onClose=()=>{}, autoCloseMs=4000 }) {
  useEffect(() => {
    if (!autoCloseMs) return;
    const t = setTimeout(onClose, autoCloseMs);
    return () => clearTimeout(t);
  }, [autoCloseMs, onClose]);

  return (
    <div role="status" aria-live="polite" className={`alert alert-${kind}`}>
      <div className="alert-icon" aria-hidden>
        <svg width="20" height="20" viewBox="0 0 24 24"><path d="M12 22a10 10 0 1 1 0-20 10 10 0 0 1 0 20zm-2-6 7-7-1.4-1.4L10 13.2 8.4 11.6 7 13l3 3z" fill="currentColor"/></svg>
      </div>
      <div className="alert-content">{children}</div>
      <button className="alert-close" onClick={onClose} aria-label="Cerrar">×</button>
    </div>
  );
}
