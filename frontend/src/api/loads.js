// GET /api/loads, export URL
import { API, get } from "./client";

export const fetchLoads = (params) => get("/api/loads", params);

// export const exportLoadUrl = (channel, lote, format = "xlsx") =>
//   `/api/loads/export?channel=${channel}&lote=${lote}&format=${format}`;

// export const exportLoadUrl = (channel, lote, format = "xlsx") =>
//   `/api/loads/export?channel=${encodeURIComponent(channel)}&lote=${encodeURIComponent(lote)}&format=${encodeURIComponent(format)}`;

// export const exportLoadUrl = (channel, lote, format = "xlsx") =>
//   `/api/loads/export?channel=${encodeURIComponent(channel)}&lote=${encodeURIComponent(lote)}&format=${encodeURIComponent(format)}`;

export const exportLoadUrl = (channel, lote, format = "csv") =>
    `/api/loads/export?channel=${encodeURIComponent(String(channel).toLowerCase())}&lote=${encodeURIComponent(lote)}&format=${encodeURIComponent(format)}`;