/** Section chỉ thuộc Home — dùng model local pages/home/model. */
import { useAuth } from 'react-oidc-context';
import { useAppDispatch, useAppSelector } from '../../../../app/hooks';
import { pingService, selectServiceHealth, SERVICE_META, type ServiceKey } from '../../model';
import ServiceCard from './ServiceCard';
import styles from './ServiceHealthSection.module.scss';

const SERVICES: ServiceKey[] = ['base', 'upload'];

export default function ServiceHealthSection() {
  const auth = useAuth();
  const dispatch = useAppDispatch();
  const baseHealth = useAppSelector(selectServiceHealth('base'));
  const uploadHealth = useAppSelector(selectServiceHealth('upload'));

  const healthByKey = { base: baseHealth, upload: uploadHealth };

  return (
    <section className={styles.section}>
      <h3 className={styles.title}>API Health Check</h3>
      <div className={styles.cards}>
        {SERVICES.map((key) => {
          const meta = SERVICE_META[key];
          const health = healthByKey[key];
          return (
            <ServiceCard
              key={key}
              name={meta.name}
              description={meta.description}
              port={meta.port}
              status={health.status}
              data={health.data}
              error={health.error}
              onPing={() => dispatch(pingService(key))}
              requiresAuth
              isAuthenticated={auth.isAuthenticated}
              onLogin={() => auth.signinRedirect()}
            />
          );
        })}
      </div>
    </section>
  );
}
