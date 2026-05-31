/** API gọi trực tiếp tại page → dispatch vào features/user slice. */
import { Alert, Button, Descriptions, Space, Spin } from 'antd';
import { UserOutlined, ArrowRightOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { useAuth } from 'react-oidc-context';
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
  const auth = useAuth();
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const user = useAppSelector(selectUser);
  const status = useAppSelector(selectUserStatus);
  const error = useAppSelector(selectUserError);
  const hasUser = useAppSelector(selectHasUser);

  const handleGetInfo = async () => {
    if (!auth.isAuthenticated) {
      auth.signinRedirect();
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

      {!auth.isAuthenticated && (
        <Alert
          type="info"
          showIcon
          message="Đăng nhập SSO trước khi gọi API User Info (Bearer token)."
          style={{ marginBottom: 12 }}
        />
      )}

      {status === 'loading' && (
        <div className={styles.summary}>
          <Spin tip="Đang lấy thông tin user..." />
        </div>
      )}

      {status === 'failed' && error && (
        <Alert type="error" showIcon message={error} style={{ marginBottom: 12 }} />
      )}

      {hasUser && user && (
        <div className={styles.summary}>
          <Descriptions bordered size="small" column={1}>
            <Descriptions.Item label="Họ tên">{user.fullName}</Descriptions.Item>
            <Descriptions.Item label="Tuổi">{user.age}</Descriptions.Item>
            <Descriptions.Item label="Phone">{user.phone}</Descriptions.Item>
            <Descriptions.Item label="Địa chỉ">{user.address}</Descriptions.Item>
          </Descriptions>
          <Space style={{ marginTop: 12 }}>
            <span style={{ color: '#64748b', fontSize: 13 }}>
              API gọi ngay trong page → dispatch(setUser) → /user đọc store.
            </span>
          </Space>
        </div>
      )}
    </section>
  );
}
