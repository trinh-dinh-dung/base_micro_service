/** API gá»i trá»±c tiáº¿p táº¡i page â†’ dispatch vÃ o features/user slice. */
import { Alert, Button, Descriptions, Space, Spin } from 'antd';
import { UserOutlined, ArrowRightOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { selectIsAuthenticated } from '../../../../features/auth/authSelectors';
import { useAppDispatch, useAppSelector } from '../../../../app/hooks';
import {
  fetchUserFromApi,
  selectHasUser,
  selectUser,
  selectUserError,
  selectUserStatus,
  setFailed,
  setLoading,
  setUser,
} from '../../../../features/user';
import styles from './UserInfoSection.module.scss';

export default function UserInfoSection() {
  const navigate = useNavigate();
  const isAuthenticated = useAppSelector(selectIsAuthenticated);
  const dispatch = useAppDispatch();

  const user = useAppSelector(selectUser);
  const status = useAppSelector(selectUserStatus);
  const error = useAppSelector(selectUserError);
  const hasUser = useAppSelector(selectHasUser);

  const handleGetInfo = async () => {
    if (!isAuthenticated) {
      navigate('/login');
      return;
    }

    dispatch(setLoading());

    try {
      const data = await fetchUserFromApi();
      dispatch(setUser(data));
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to load user';
      dispatch(setFailed(message));
    }
  };

  return (
    <section className={styles.section}>
      <h3 className={styles.title}>User Profile (Service_Base)</h3>

      <div className={styles.actions}>
        <Button
          type="primary"
          icon={<UserOutlined />}
          loading={status === 'loading'}
          onClick={() => void handleGetInfo()}
        >
          Get Info User
        </Button>
        {hasUser && (
          <Button
            type="default"
            icon={<ArrowRightOutlined />}
            onClick={() => navigate('/user')}
          >
            Xem trang User (/user)
          </Button>
        )}
      </div>

      {!isAuthenticated && (
        <Alert
          type="info"
          showIcon
          message="ÄÄƒng nháº­p SSO trÆ°á»›c khi gá»i API User Info (Bearer token)."
          style={{ marginBottom: 12 }}
        />
      )}

      {status === 'loading' && (
        <div className={styles.summary}>
          <Spin tip="Äang láº¥y thÃ´ng tin user..." />
        </div>
      )}

      {status === 'failed' && error && (
        <Alert type="error" showIcon message={error} style={{ marginBottom: 12 }} />
      )}

      {hasUser && user && (
        <div className={styles.summary}>
          <Descriptions bordered size="small" column={1}>
            <Descriptions.Item label="Há» tÃªn">{user.fullName}</Descriptions.Item>
            <Descriptions.Item label="Tuá»•i">{user.age}</Descriptions.Item>
            <Descriptions.Item label="Phone">{user.phone}</Descriptions.Item>
            <Descriptions.Item label="Äá»‹a chá»‰">{user.address}</Descriptions.Item>
          </Descriptions>
          <Space style={{ marginTop: 12 }}>
            <span style={{ color: '#64748b', fontSize: 13 }}>
              API gá»i ngay trong page â†’ dispatch(setUser) â†’ /user Ä‘á»c store.
            </span>
          </Space>
        </div>
      )}
    </section>
  );
}


