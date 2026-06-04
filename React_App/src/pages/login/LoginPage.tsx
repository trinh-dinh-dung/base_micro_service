import { useState } from "react";
import { Navigate, useLocation } from "react-router-dom";
import { useAppSelector } from "../../app/hooks";
import { selectIsAuthenticated } from "../../features/auth/authSelectors";
import styles from "./LoginPage.module.scss";

export default function LoginPage() {
  const isAuthenticated = useAppSelector(selectIsAuthenticated);
  const location = useLocation();
  const from =
    (location.state as { from?: { pathname: string } })?.from?.pathname ?? "/";

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const buildSsoSignInUrl = async (): Promise<string | null> => {
    const gatewayURL = import.meta.env.VITE_GATEWAY_URL as string;

    if (!gatewayURL) {
      setError("Thiếu cấu hình môi trường (VITE_GATEWAY_URL).");
      return null;
    }

    try {
      const response = await fetch(`${gatewayURL}/api/auth/sso/signin-url`);
      if (!response.ok) {
        setError("Không tạo được URL đăng nhập SSO từ backend.");
        return null;
      }

      const payload = (await response.json()) as { url?: string };
      if (!payload.url) {
        setError("Backend không trả về URL đăng nhập SSO hợp lệ.");
        return null;
      }

      return payload.url;
    } catch {
      setError("Không kết nối được backend để tạo URL đăng nhập SSO.");
      return null;
    }
  };

  if (isAuthenticated) {
    return <Navigate to={from} replace />;
  }

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError("");

    const signInUrl = await buildSsoSignInUrl();
    if (signInUrl) {
      window.location.replace(signInUrl);
    }
  };

  return (
    <div className={styles["login-page"]}>
      <div className={styles.card}>
        <div className={styles.brand}>MS</div>
        <h1 className={styles.title}>Đăng nhập</h1>
        <form className={styles.form} onSubmit={handleSubmit}>
          <div className={styles.field}>
            <label htmlFor="username">Tên đăng nhập</label>
            <input
              id="username"
              type="text"
              autoComplete="username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              placeholder="Nhập tài khoản"
            />
          </div>
          <div className={styles.field}>
            <label htmlFor="password">Mật khẩu</label>
            <input
              id="password"
              type="password"
              autoComplete="current-password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="••••••"
            />
          </div>
          {error && <p className={styles.error}>{error}</p>}
          <button type="submit" className={styles.submit}>
            Đăng nhập qua SSO
          </button>
        </form>
      </div>
    </div>
  );
}
