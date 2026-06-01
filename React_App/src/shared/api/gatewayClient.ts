import axios from "axios";

const GATEWAY_URL = import.meta.env.VITE_GATEWAY_URL ?? "http://localhost:5050";

/** Tạo random hex string độ dài n ký tự */
function randomHex(length: number): string {
  return Array.from(crypto.getRandomValues(new Uint8Array(length / 2)))
    .map((b) => b.toString(16).padStart(2, "0"))
    .join("");
}

/** W3C traceparent: 00-{32 hex traceId}-{16 hex parentId}-01 */
function generateTraceparent(): string {
  return `00-${randomHex(32)}-${randomHex(16)}-01`;
}

export const gatewayClient = axios.create({
  baseURL: GATEWAY_URL,
  headers: { "Content-Type": "application/json" },
});

// Interceptor: tự động thêm traceparent vào mọi request
gatewayClient.interceptors.request.use((config) => {
  const traceparent = generateTraceparent();
  config.headers["traceparent"] = traceparent;
  // Lưu lại để dễ debug trên console
  console.debug("[Trace]", config.method?.toUpperCase(), config.url, traceparent);
  return config;
});

export function setAuthToken(token: string | null) {
  if (token) {
    gatewayClient.defaults.headers.common["Authorization"] = `Bearer ${token}`;
  } else {
    delete gatewayClient.defaults.headers.common["Authorization"];
  }
}
