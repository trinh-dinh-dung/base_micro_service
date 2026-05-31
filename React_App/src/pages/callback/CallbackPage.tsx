/**
 * Page OIDC callback — không cần Redux (chỉ redirect sau khi SSO xong).
 */
import { useEffect } from 'react';
import { useAuth } from 'react-oidc-context';
import { useNavigate } from 'react-router-dom';
import { LoadingScreen } from '../../shared/ui/LoadingScreen';
import './CallbackPage.module.scss';

export default function CallbackPage() {
  const auth = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (!auth.isLoading && !auth.error) {
      navigate('/', { replace: true });
    }
  }, [auth.isLoading, auth.error, navigate]);

  if (auth.error) {
    return (
      <LoadingScreen
        error={auth.error.message}
        onAction={() => navigate('/')}
        actionLabel="Go Home"
      />
    );
  }

  return <LoadingScreen message="Completing sign in..." />;
}
