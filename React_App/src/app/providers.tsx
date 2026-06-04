import type { ReactNode } from "react";
import { Provider } from "react-redux";
import { ConfigProvider } from "antd";
import viVN from "antd/locale/vi_VN";
import { BrowserRouter } from "react-router-dom";
import { store } from "./store";

interface AppProvidersProps {
  children: ReactNode;
}

export default function AppProviders({ children }: AppProvidersProps) {
  return (
    <Provider store={store}>
      <ConfigProvider locale={viVN}>
        <BrowserRouter>{children}</BrowserRouter>
      </ConfigProvider>
    </Provider>
  );
}
