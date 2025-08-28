// POST /api/import/{channel}
import { postForm } from "./client";

export function importFile(channel, { idCartera, file, usuario }) {
  const fd = new FormData();
  fd.append("IdCartera", String(idCartera));
  fd.append("File", file);
  if (usuario) fd.append("Usuario", usuario);
  return postForm(`/api/import/${channel}`, fd);
}
