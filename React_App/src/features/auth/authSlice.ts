import { createSlice, type PayloadAction } from "@reduxjs/toolkit";
import {
  getAccessToken,
  persistAccessToken,
  clearAccessToken,
} from "../../shared/api/authStorage";
import { setAuthToken } from "../../shared/api/gatewayClient";

export interface AuthState {
  accessToken: string | null;
  isAuthenticated: boolean;
}

const storedToken = getAccessToken();

const initialState: AuthState = {
  accessToken: storedToken,
  isAuthenticated: !!storedToken,
};

// Sync stored token vào axios ngay lúc khởi động
if (storedToken) {
  setAuthToken(storedToken);
}

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    setAccessToken(state, action: PayloadAction<string>) {
      state.accessToken = action.payload;
      state.isAuthenticated = true;
      persistAccessToken(action.payload);
      setAuthToken(action.payload);
    },
    clearAuth(state) {
      state.accessToken = null;
      state.isAuthenticated = false;
      clearAccessToken();
      setAuthToken(null);
    },
  },
});

export const { setAccessToken, clearAuth } = authSlice.actions;
export default authSlice.reducer;
