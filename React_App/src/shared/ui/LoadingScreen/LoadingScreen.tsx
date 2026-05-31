import styles from './LoadingScreen.module.scss';

interface LoadingScreenProps {
  message?: string;
  error?: string;
  onAction?: () => void;
  actionLabel?: string;
}

export default function LoadingScreen({
  message = 'Loading...',
  error,
  onAction,
  actionLabel = 'Go Home',
}: LoadingScreenProps) {
  return (
    <div className={styles.root}>
      {!error && <div className={styles.spinner} />}
      {error ? (
        <p className={styles.error}>{error}</p>
      ) : (
        <p>{message}</p>
      )}
      {onAction && (
        <button type="button" className={styles.button} onClick={onAction}>
          {actionLabel}
        </button>
      )}
    </div>
  );
}
