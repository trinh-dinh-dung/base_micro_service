/**
 * Page = compose sections. Không chứa business logic / Redux trực tiếp.
 */
import { useAuth } from 'react-oidc-context';
import { LoadingScreen } from '../../shared/ui/LoadingScreen';
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
  const auth = useAuth();

  if (auth.isLoading) {
    return <LoadingScreen message="Loading..." />;
  }

  if (auth.error) {
    return (
      <LoadingScreen
        error={auth.error.message}
        onAction={() => auth.signinRedirect()}
        actionLabel="Try Again"
      />
    );
  }

  const displayName = auth.user?.profile?.name ?? auth.user?.profile?.sub ?? 'User';

  return (
    <div className={styles.app}>
      <HomeHeader
        isAuthenticated={auth.isAuthenticated}
        displayName={displayName}
        onSignIn={() => auth.signinRedirect()}
        onSignOut={() => auth.signoutRedirect()}
      />

      <main className={styles.main}>
        <div style={{ display: 'flex', justifyContent: 'flex-end', marginBottom: 12 }}>
          <SyncAuditMetadataButton pageCode={PAGE_CODE} />
        </div>
        <HeroSection isAuthenticated={auth.isAuthenticated} userName={displayName} />
        <ArchitectureSection />
        <UserInfoSection />
        <ServiceHealthSection />
        {auth.isAuthenticated && auth.user?.access_token && (
          <TokenSection accessToken={auth.user.access_token} />
        )}
      </main>
    </div>
  );
}
