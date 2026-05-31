import styles from './HeroSection.module.scss';

interface HeroSectionProps {
  isAuthenticated: boolean;
  userName: string;
}

export default function HeroSection({ isAuthenticated, userName }: HeroSectionProps) {
  return (
    <section className={styles.section}>
      <h2 className={styles.title}>
        Hello, {isAuthenticated ? userName : 'Guest'} 👋
      </h2>
      <p className={styles.subtitle}>
        {isAuthenticated
          ? 'You are authenticated. Call services via the Gateway below.'
          : 'Sign in to access protected API endpoints through the Gateway.'}
      </p>
    </section>
  );
}
