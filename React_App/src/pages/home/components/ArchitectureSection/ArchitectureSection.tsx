import styles from './ArchitectureSection.module.scss';

export default function ArchitectureSection() {
  return (
    <section className={styles.section}>
      <h3 className={styles.title}>Architecture Overview</h3>
      <div className={styles.flow}>
        <div className={`${styles.node} ${styles.react}`}>
          React App<span>:3000</span>
        </div>
        <div className={styles.arrow}>→ SSO →</div>
        <div className={`${styles.node} ${styles.auth}`}>
          AuthServer<span>:5010</span>
        </div>
        <div className={styles.arrow}>→ Token →</div>
        <div className={`${styles.node} ${styles.gateway}`}>
          Gateway (YARP)<span>:5050</span>
        </div>
        <div className={styles.arrow}>→ Routes →</div>
        <div className={styles.nodesCol}>
          <div className={`${styles.node} ${styles.service}`}>
            Service_Base<span>:5000</span>
          </div>
          <div className={`${styles.node} ${styles.service}`}>
            Service_Upload<span>:5001</span>
          </div>
        </div>
      </div>
      <div className={styles.note}>
        📍 Optional: <strong>Consul</strong> (:8500) for dynamic service discovery
      </div>
    </section>
  );
}
