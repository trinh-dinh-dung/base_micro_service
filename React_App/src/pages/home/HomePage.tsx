/**
 * Page = compose sections. Không chứa business logic / Redux trực tiếp.
 */
import { useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../app/hooks';
import { selectIsAuthenticated, selectAccessToken } from '../../features/auth/authSelectors';
import { clearAuth } from '../../features/auth/authSlice';
import ArchitectureSection from './components/ArchitectureSection/ArchitectureSection';
import HeroSection from './components/HeroSection/HeroSection';
import HomeHeader from './components/HomeHeader/HomeHeader';
import TokenSection from './components/TokenSection/TokenSection';
import ServiceHealthSection from './sections/ServiceHealthSection/ServiceHealthSection';
import UserInfoSection from './sections/UserInfoSection/UserInfoSection';
import { SyncAuditMetadataButton } from '../../shared/ui/SyncAuditMetadataButton';
import styles from './HomePage.module.scss';

const PAGE_CODE = 'HOME_DASHBOARD';

export default function HomePage() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const isAuthenticated = useAppSelector(selectIsAuthenticated);
  const accessToken = useAppSelector(selectAccessToken);

  const handleSignOut = () => {
    dispatch(clearAuth());
    navigate('/login', { replace: true });
  };

  return (
    <div className={styles.app}>
      <HomeHeader
        isAuthenticated={isAuthenticated}
        displayName="Admin"
        onSignIn={() => navigate('/login')}
        onSignOut={handleSignOut}
      />

      <main className={styles.main}>
        <div style={{ display: 'flex', justifyContent: 'flex-end', marginBottom: 12 }}>
          <SyncAuditMetadataButton pageCode={PAGE_CODE} />
        </div>
        <HeroSection isAuthenticated={isAuthenticated} userName="Admin" />
        <ArchitectureSection />
        <UserInfoSection />
        <ServiceHealthSection />
        {isAuthenticated && accessToken && (
          <TokenSection accessToken={accessToken} />
        )}
      </main>
    </div>
  );
}
