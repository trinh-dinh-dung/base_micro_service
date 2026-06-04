import type { RootState } from "../../app/store";

export const selectAccessToken = (state: RootState) => state.auth.accessToken;
export const selectIsAuthenticated = (state: RootState) => state.auth.isAuthenticated;
