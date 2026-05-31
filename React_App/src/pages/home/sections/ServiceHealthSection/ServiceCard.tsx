import type { ServiceStatus } from '../../model/types';
import styles from './ServiceCard.module.scss';

interface ServiceCardProps {
  name: string;
  description: string;
  port: number;
  status: ServiceStatus;
  data?: unknown;
  error?: string;
  onPing: () => void;
  requiresAuth: boolean;
  isAuthenticated: boolean;
  onLogin: () => void;
}

export default function ServiceCard({
  name,
  description,
  port,
  status,
  data,
  error,
  onPing,
  requiresAuth,
  isAuthenticated,
  onLogin,
}: ServiceCardProps) {
  const displayData = data ?? error;
  const responseClass =
    status === 'ok'
      ? styles.responseOk
      : status === 'error'
        ? styles.responseError
        : '';

  return (
    <div className={styles.card}>
      <div className={styles.header}>
        <div>
          <h4>{name}</h4>
          <p className={styles.desc}>{description}</p>
          <span className={styles.port}>:{port}</span>
        </div>
        <div
          className={`${styles.statusDot} ${styles[`status-${status}`]}`}
          title={status}
        />
      </div>
      <div className={styles.actions}>
        {requiresAuth && !isAuthenticated ? (
          <button type="button" className={`${styles.btn} ${styles.secondary}`} onClick={onLogin}>
            Login to Test
          </button>
        ) : (
          <button
            type="button"
            className={`${styles.btn} ${styles.primary}`}
            onClick={onPing}
            disabled={status === 'loading'}
          >
            {status === 'loading' ? 'Pinging...' : 'Ping via Gateway'}
          </button>
        )}
      </div>
      {displayData !== undefined && (
        <div className={`${styles.response} ${responseClass}`}>
          <pre>
            {typeof displayData === 'string'
              ? displayData
              : JSON.stringify(displayData, null, 2)}
          </pre>
        </div>
      )}
    </div>
  );
}
