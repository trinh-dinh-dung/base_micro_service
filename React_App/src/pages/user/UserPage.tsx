/**
 * Trang /user â€” Ä‘á»c store; gá»i API trá»±c tiáº¿p táº¡i page khi Refresh.
 */
import { Alert, Button, Card, Descriptions, Empty, Tag } from 'antd';
import { ArrowLeftOutlined, ReloadOutlined } from '@ant-design/icons';
import { Link, useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../app/hooks';
import { selectIsAuthenticated } from '../../features/auth/authSelectors';
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
import { SyncAuditMetadataButton } from '../../shared/ui/SyncAuditMetadataButton';
import styles from './UserPage.module.scss';

const PAGE_CODE = 'USER_PROFILE';

export default function UserPage() {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const isAuthenticated = useAppSelector(selectIsAuthenticated);

  const user = useAppSelector(selectUser);
  const status = useAppSelector(selectUserStatus);
  const error = useAppSelector(selectUserError);
  const hasUser = useAppSelector(selectHasUser);

  const handleRefetch = async () => {
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
    <div className={styles.app}>
      <main className={styles.main}>
        <div className={styles.headerRow}>
          <h1 className={styles.title}>User Profile</h1>
          <div>
            <SyncAuditMetadataButton pageCode={PAGE_CODE} />
            <Button icon={<ArrowLeftOutlined />} onClick={() => navigate('/')} style={{ marginLeft: 8 }}>
              Vá» Home
            </Button>
          </div>
        </div>

        <Alert
          type="success"
          showIcon
          message="API gá»i trá»±c tiáº¿p trong UserPage"
          description="fetchUserFromApi() â†’ dispatch(setUser) â€” state chia sáº» vá»›i Home."
          style={{ marginBottom: 20 }}
        />

        {!hasUser && status !== 'loading' && (
          <Card>
            <Empty description="ChÆ°a cÃ³ dá»¯ liá»‡u. Vá» Home vÃ  báº¥m Get Info User.">
              <Link to="/">
                <Button type="primary">Vá» Home</Button>
              </Link>
              <Button
                style={{ marginLeft: 8 }}
                icon={<ReloadOutlined />}
                onClick={() => void handleRefetch()}
              >
                Gá»i API ngay
              </Button>
            </Empty>
          </Card>
        )}

        {status === 'loading' && <Card loading title="Äang táº£i..." />}

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
              <Descriptions.Item label="Há» tÃªn">{user.fullName}</Descriptions.Item>
              <Descriptions.Item label="Tuá»•i">{user.age}</Descriptions.Item>
              <Descriptions.Item label="Sá»‘ Ä‘iá»‡n thoáº¡i">{user.phone}</Descriptions.Item>
              <Descriptions.Item label="Äá»‹a chá»‰">{user.address}</Descriptions.Item>
            </Descriptions>
          </Card>
        )}
      </main>
    </div>
  );
}

