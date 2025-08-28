export default function Modal({ title, onClose, children }) {
  return (
    <div style={styles.overlay} onClick={onClose}>
      <div style={styles.modal} onClick={(e)=>e.stopPropagation()}>
        <div style={styles.header}>
          <h3 style={{margin:0}}>{title}</h3>
          <button onClick={onClose} style={styles.x}>✕</button>
        </div>
        <div>{children}</div>
      </div>
    </div>
  );
}

const styles = {
  overlay: { position:"fixed", inset:0, background:"rgba(0,0,0,.35)",
    display:"grid", placeItems:"center", padding:16, zIndex:50 },
  modal: { width:"min(700px,100%)", background:"#fff", borderRadius:16,
    padding:20, boxShadow:"0 20px 40px rgba(0,0,0,.2)" },
  header: { display:"flex", justifyContent:"space-between", alignItems:"center", marginBottom:8 },
  x: { border:0, background:"transparent", fontSize:18, cursor:"pointer" }
};
