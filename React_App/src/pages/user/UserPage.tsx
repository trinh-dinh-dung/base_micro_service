/**
 * Trang /user — đọc store; gọi API trực tiếp tại page khi Refresh.
 */
import { Alert, Button, Card, Descriptions, Empty, Tag } from 'antd';
import { ArrowLeftOutlined, ReloadOutlined } from '@ant-design/icons';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from 'react-oidc-context';
import { useAppDispatch, useAppSelector } from '../../app/hooks';
import {
  fetchUserFromApi,
  selectHasUser,
  selectUser,
  selectUserError,
  selectUserStatus,
  setFailed,
  setLoading,
  setUser,
} from '../../features/user';
import styles from './UserPage.module.scss';

export default function UserPage() {
  const auth = useAuth();
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const user = useAppSelector(selectUser);
  const status = useAppSelector(selectUserStatus);
  const error = useAppSelector(selectUserError);
  const hasUser = useAppSelector(selectHasUser);

  const handleRefetch = async () => {
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
    <div className={styles.app}>
      <main className={styles.main}>
        <div className={styles.headerRow}>
          <h1 className={styles.title}>User Profile</h1>
          <Button icon={<ArrowLeftOutlined />} onClick={() => navigate('/')}>
            Về Home
          </Button>
        </div>

        <Alert
          type="success"
          showIcon
          message="API gọi trực tiếp trong UserPage"
          description="fetchUserFromApi() → dispatch(setUser) — state chia sẻ với Home."
          style={{ marginBottom: 20 }}
        />

        {!hasUser && status !== 'loading' && (
          <Card>
            <Empty description="Chưa có dữ liệu. Về Home và bấm Get Info User.">
              <Link to="/">
                <Button type="primary">Về Home</Button>
              </Link>
              <Button
                style={{ marginLeft: 8 }}
                icon={<ReloadOutlined />}
                onClick={() => void handleRefetch()}
              >
                Gọi API ngay
              </Button>
            </Empty>
          </Card>
        )}

        {status === 'loading' && <Card loading title="Đang tải..." />}

        {status === 'failed' && error && (
          <Alert type="error" showIcon message={error} style={{ marginBottom: 16 }} />
        )}

        {hasUser && user && (
          <Card
            title={
              <>
                {user.fullName} <Tag color="blue">{user.userName}</Tag>
              </>
            }
            extra={
              <Button
                size="small"
                icon={<ReloadOutlined />}
                onClick={() => void handleRefetch()}
              >
                Refresh API
              </Button>
            }
          >
            <Descriptions bordered column={1}>
              <Descriptions.Item label="Username">{user.userName}</Descriptions.Item>
              <Descriptions.Item label="Họ tên">{user.fullName}</Descriptions.Item>
              <Descriptions.Item label="Tuổi">{user.age}</Descriptions.Item>
              <Descriptions.Item label="Số điện thoại">{user.phone}</Descriptions.Item>
              <Descriptions.Item label="Địa chỉ">{user.address}</Descriptions.Item>
            </Descriptions>
          </Card>
        )}
      </main>
    </div>
  );
}
