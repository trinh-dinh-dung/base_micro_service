import { useEffect } from "react";
import { useAuth } from "react-oidc-context";
import { useAppDispatch } from "../../../app/hooks";
import { setAccessToken } from "../authSlice";
import { setAuthToken } from "../../../shared/api/gatewayClient";

/**
 * Đồng bộ token từ react-oidc-context → Redux + axios gateway client.
 */
export default function AuthTokenSync() {
  const auth = useAuth();
  const dispatch = useAppDispatch();

  useEffect(() => {
    const token = auth.user?.access_token ?? null;
    dispatch(setAccessToken(token));
    setAuthToken(token);
  }, [auth.user?.access_token, dispatch]);

  return null;
}
