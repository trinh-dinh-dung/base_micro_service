import styles from './TokenSection.module.scss';

interface TokenSectionProps {
  accessToken: string;
}

export default function TokenSection({ accessToken }: TokenSectionProps) {
  return (
    <section className={styles.section}>
      <h3 className={styles.title}>Access Token</h3>
      <div className={styles.box}>
        <pre>{accessToken}</pre>
      </div>
    </section>
  );
}
