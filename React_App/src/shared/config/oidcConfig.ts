import type { AuthProviderProps } from "react-oidc-context";

export const oidcConfig: AuthProviderProps = {
  authority: import.meta.env.VITE_AUTH_AUTHORITY ?? "http://localhost:5010",
  client_id: import.meta.env.VITE_CLIENT_ID ?? "react-app",
  redirect_uri: import.meta.env.VITE_REDIRECT_URI ?? `${window.location.origin}/callback`,
  post_logout_redirect_uri: import.meta.env.VITE_POST_LOGOUT_URI ?? window.location.origin,
  scope: "openid profile email api",
  response_type: "code",
  automaticSilentRenew: true,
  onSigninCallback: () => {
    window.history.replaceState({}, document.title, window.location.pathname);
  },
};
