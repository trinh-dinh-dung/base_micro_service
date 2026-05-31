import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./shared/styles/index.scss";
import AppProviders from "./app/providers";
import App from "./app/App";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <AppProviders>
      <App />
    </AppProviders>
  </StrictMode>
);
