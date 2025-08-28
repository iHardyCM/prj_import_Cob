export default function Badge({ text }) {
  const t = String(text).toUpperCase();
  const color = t === "SMS" ? "#2563eb" : t === "IVR" ? "#059669" : "#7c3aed";
  return (
    <span style={{
      display:"inline-block", padding:"2px 8px", borderRadius:999,
      backgroundColor:`${color}20`, color, fontSize:12, fontWeight:600
    }}>{t}</span>
  );
}
