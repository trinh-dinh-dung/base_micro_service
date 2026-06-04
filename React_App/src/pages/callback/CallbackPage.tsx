/**
 * /callback – route cũ (OIDC). Giờ không dùng, chuyển hết về /login-callback.
 * Giữ lại để không 404 nếu còn bookmark cũ.
 */
import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { LoadingScreen } from '../../shared/ui/LoadingScreen';

export default function CallbackPage() {
  const navigate = useNavigate();

  useEffect(() => {
    navigate('/login-callback', { replace: true });
  }, [navigate]);

  return <LoadingScreen message="Đang chuyển hướng..." />;
}
