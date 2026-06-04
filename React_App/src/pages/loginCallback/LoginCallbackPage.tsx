import { useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { useAppDispatch } from "../../app/hooks";
import { setAccessToken } from "../../features/auth/authSlice";
import { LoadingScreen } from "../../shared/ui/LoadingScreen";

/**
 * IAM redirect về đây sau khi Microsoft SSO thành công.
 * URL dạng: /login-callback?access_token=...&refresh_token=...&sign=...
 */
export default function LoginCallbackPage() {
  const [searchParams] = useSearchParams();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  useEffect(() => {
    const hashParams = new URLSearchParams(window.location.hash.replace(/^#/, ""));
    const accessToken =
      searchParams.get("access_token")
      ?? searchParams.get("accessToken")
      ?? searchParams.get("AccessToken")
      ?? searchParams.get("token")
      ?? hashParams.get("access_token")
      ?? hashParams.get("accessToken")
      ?? hashParams.get("AccessToken")
      ?? hashParams.get("token");

    if (accessToken) {
      // Lưu token vào Redux + localStorage + axios header
      dispatch(setAccessToken(accessToken));
      // Xóa query params nhạy cảm khỏi URL sau khi xử lý
      window.history.replaceState({}, document.title, "/login-callback");
      navigate("/", { replace: true });
    } else {
      // Không có token → về trang login
      navigate("/login", { replace: true });
    }
  }, [searchParams, dispatch, navigate]);

  return <LoadingScreen message="Đang xử lý đăng nhập..." />;
}
