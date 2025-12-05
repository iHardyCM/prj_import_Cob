const TZ = "America/Lima";
const LOCALE = "es-PE";

function toDateSafe(v){
  if (!v) return null;
  const s = typeof v === "string" ? v.replace(" ", "T") : v;
  const d = s instanceof Date ? s : new Date(s);
  return Number.isNaN(d.getTime()) ? null : d;
}

export function formatDateTime(v){
  const d = toDateSafe(v); if(!d) return "";
  return new Intl.DateTimeFormat(LOCALE,{
    timeZone:TZ, day:"2-digit", month:"2-digit", year:"numeric",
    hour:"2-digit", minute:"2-digit", second:"2-digit", hour12:false
  }).format(d);
}
export function formatDate(v){
  const d = toDateSafe(v); if(!d) return "";
  return new Intl.DateTimeFormat(LOCALE,{ timeZone:TZ, day:"2-digit", month:"2-digit", year:"numeric" }).format(d);
}
export function formatTime(v,{withSeconds=true}={}){
  const d = toDateSafe(v); if(!d) return "";
  return new Intl.DateTimeFormat(LOCALE,{
    timeZone:TZ, hour:"2-digit", minute:"2-digit", second:withSeconds?"2-digit":undefined, hour12:false
  }).format(d);
}
function sameLocalDay(a,b){
  const fa = new Intl.DateTimeFormat(LOCALE,{timeZone:TZ,year:"numeric",month:"2-digit",day:"2-digit"}).format(toDateSafe(a));
  const fb = new Intl.DateTimeFormat(LOCALE,{timeZone:TZ,year:"numeric",month:"2-digit",day:"2-digit"}).format(toDateSafe(b));
  return fa===fb;
}
export function formatRange(start,end,{withSeconds=false}={}){
  if(!start && !end) return "—";
  if(start && !end)  return formatDateTime(start);
  if(!start && end)  return formatDateTime(end);
  return sameLocalDay(start,end)
    ? `${formatDate(start)} ${formatTime(start,{withSeconds})} – ${formatTime(end,{withSeconds})}`
    : `${formatDateTime(start)} → ${formatDateTime(end)}`;
}
