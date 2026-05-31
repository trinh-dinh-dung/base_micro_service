import axios from "axios";

const GATEWAY_URL = import.meta.env.VITE_GATEWAY_URL ?? "http://localhost:5050";

export const gatewayClient = axios.create({
  baseURL: GATEWAY_URL,
  headers: { "Content-Type": "application/json" },
});

export function setAuthToken(token: string | null) {
  if (token) {
    gatewayClient.defaults.headers.common["Authorization"] = `Bearer ${token}`;
  } else {
    delete gatewayClient.defaults.headers.common["Authorization"];
  }
}
