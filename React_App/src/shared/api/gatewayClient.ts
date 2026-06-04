import axios from "axios";

const GATEWAY_URL = import.meta.env.VITE_GATEWAY_URL ?? "http://localhost:5050";
const ACCESS_TOKEN_KEY = "app_access_token";

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
  console.debug("[Trace]", config.method?.toUpperCase(), config.url, traceparent);
  return config;
});

// Interceptor: 401 → xóa token, để UI tự xử lý hiển thị lỗi/điều hướng
gatewayClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      if (typeof window !== "undefined") {
        localStorage.removeItem(ACCESS_TOKEN_KEY);
        window.dispatchEvent(new CustomEvent("app:auth:unauthorized"));
      }
      delete gatewayClient.defaults.headers.common["Authorization"];
    }
    return Promise.reject(error);
  }
);

export function setAuthToken(token: string | null) {
  if (token) {
    gatewayClient.defaults.headers.common["Authorization"] = `Bearer ${token}`;
  } else {
    delete gatewayClient.defaults.headers.common["Authorization"];
  }
}
