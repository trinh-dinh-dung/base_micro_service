import { useState } from 'react';
import { Layout, Menu, theme, Button } from 'antd';
import { HomeOutlined, UserOutlined, BankOutlined, LogoutOutlined } from '@ant-design/icons';
import { Outlet, useLocation, useNavigate } from 'react-router-dom';
import { useAppDispatch } from '../../../app/hooks';
import { clearAuth } from '../../../features/auth/authSlice';
import styles from './AdminLayout.module.scss';

const { Header, Sider, Content } = Layout;

const menuItems = [
  {
    key: '/',
    icon: <HomeOutlined />,
    label: 'Home',
  },
  {
    key: '/user',
    icon: <UserOutlined />,
    label: 'User',
  },
  {
    key: '/department',
    icon: <BankOutlined />,
    label: 'Phòng ban',
  },
];

export default function AdminLayout() {
  const [collapsed, setCollapsed] = useState(false);
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const location = useLocation();
  const { token } = theme.useToken();

  const handleLogout = () => {
    dispatch(clearAuth());
    navigate('/login', { replace: true });
  };

  const selectedKey = menuItems.find((item) => location.pathname.startsWith(item.key === '/' ? '/' : item.key))?.key ?? '/';

  return (
    <Layout className={styles.layout}>
      <Sider
        collapsible
        collapsed={collapsed}
        onCollapse={setCollapsed}
        className={styles.sider}
        theme="dark"
      >
        <div className={styles.logo}>
          {collapsed ? 'MS' : 'Microservice'}
        </div>
        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={[selectedKey]}
          items={menuItems}
          onClick={({ key }) => navigate(key)}
        />
      </Sider>

      <Layout>
        <Header
          className={styles.header}
          style={{ background: token.colorBgContainer }}
        >
          <span className={styles.headerTitle}>Admin Panel</span>
          <Button
            type="text"
            icon={<LogoutOutlined />}
            onClick={handleLogout}
            style={{ marginLeft: 'auto' }}
          >
            Đăng xuất
          </Button>
        </Header>

        <Content
          className={styles.content}
          style={{ background: token.colorBgLayout }}
        >
          <div
            className={styles.contentInner}
            style={{ background: token.colorBgContainer }}
          >
            <Outlet />
          </div>
        </Content>
      </Layout>
    </Layout>
  );
}
