import styles from './HomeHeader.module.scss';

interface HomeHeaderProps {
  isAuthenticated: boolean;
  displayName: string;
  onSignIn: () => void;
  onSignOut: () => void;
}

export default function HomeHeader({
  isAuthenticated,
  displayName,
  onSignIn,
  onSignOut,
}: HomeHeaderProps) {
  const initial = displayName[0]?.toUpperCase() ?? '?';

  return (
    <header className={styles.header}>
      <div className={styles.left}>
        <span className={styles.logo}>⚡</span>
        <h1 className={styles.title}>Microservice Dashboard</h1>
      </div>
      <div className={styles.right}>
        {isAuthenticated ? (
          <div className={styles.userInfo}>
            <span className={styles.avatar}>{initial}</span>
            <span className={styles.username}>{displayName}</span>
            <button type="button" className={styles.btnLogout} onClick={onSignOut}>
              Sign Out
            </button>
          </div>
        ) : (
          <button type="button" className={styles.btnLogin} onClick={onSignIn}>
            Sign In with SSO
          </button>
        )}
      </div>
    </header>
  );
}
