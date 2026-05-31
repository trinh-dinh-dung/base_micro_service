import type { ReactNode } from "react";
import { Provider } from "react-redux";
import { ConfigProvider } from "antd";
import viVN from "antd/locale/vi_VN";
import { AuthProvider } from "react-oidc-context";
import { BrowserRouter } from "react-router-dom";
import { oidcConfig } from "../shared/config/oidcConfig";
import { AuthTokenSync } from "../features/auth";
import { store } from "./store";

interface AppProvidersProps {
  children: ReactNode;
}

export default function AppProviders({ children }: AppProvidersProps) {
  return (
    <Provider store={store}>
      <ConfigProvider locale={viVN}>
        <AuthProvider {...oidcConfig}>
          <AuthTokenSync />
          <BrowserRouter>{children}</BrowserRouter>
        </AuthProvider>
      </ConfigProvider>
    </Provider>
  );
}
