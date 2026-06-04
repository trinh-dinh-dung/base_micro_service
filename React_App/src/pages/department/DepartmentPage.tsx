import { useEffect, useState } from 'react';
import {
  Alert,
  Button,
  Drawer,
  Form,
  Input,
  Modal,
  Popconfirm,
  Select,
  Space,
  Switch,
  Table,
  Tag,
  Typography,
  type TableColumnsType,
} from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined, ReloadOutlined, HistoryOutlined } from '@ant-design/icons';
import axios from 'axios';
import { useAppDispatch, useAppSelector } from '../../app/hooks';
import {
  fetchDepartmentsApi,
  fetchDepartmentAuditLogsApi,
  createDepartmentApi,
  updateDepartmentApi,
  deleteDepartmentApi,
  setLoading,
  setDepartments,
  setFailed,
  selectDepartments,
  selectDepartmentStatus,
  selectDepartmentError,
  type Department,
  type DepartmentAuditLog,
  type DepartmentRequest,
} from '../../features/department';
import { SyncAuditMetadataButton } from '../../shared/ui/SyncAuditMetadataButton';
import styles from './DepartmentPage.module.scss';

type ModalMode = 'create' | 'edit';
const PAGE_CODE = 'DEPARTMENT_MANAGEMENT';
const PAGE_NAME_MAP: Record<string, string> = {
  DEPARTMENT_MANAGEMENT: 'Quan ly phong ban',
};

export default function DepartmentPage() {
  const dispatch = useAppDispatch();
  const list = useAppSelector(selectDepartments);
  const status = useAppSelector(selectDepartmentStatus);
  const error = useAppSelector(selectDepartmentError);

  const [modalOpen, setModalOpen] = useState(false);
  const [modalMode, setModalMode] = useState<ModalMode>('create');
  const [submitting, setSubmitting] = useState(false);
  const [auditOpen, setAuditOpen] = useState(false);
  const [auditLoading, setAuditLoading] = useState(false);
  const [auditLogs, setAuditLogs] = useState<DepartmentAuditLog[]>([]);
  const [auditTargetName, setAuditTargetName] = useState('');
  const [auditLang] = useState<'vi' | 'en'>('vi');
  const [form] = Form.useForm<DepartmentRequest>();

  const pageName = (pageCode: string | null) => PAGE_NAME_MAP[pageCode ?? ''] ?? (pageCode || '-');

  const load = async (signal?: AbortSignal) => {
    dispatch(setLoading());
    try {
      const data = await fetchDepartmentsApi(null, signal);
      dispatch(setDepartments(data));
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.code === 'ERR_CANCELED') {
        return;
      }
      dispatch(setFailed(err instanceof Error ? err.message : 'Lỗi tải danh sách phòng ban'));
    }
  };

  useEffect(() => {
    const controller = new AbortController();
    load(controller.signal);
    return () => controller.abort();
  }, []);

  const openCreate = () => {
    form.resetFields();
    setModalMode('create');
    setModalOpen(true);
  };

  const openEdit = (record: Department) => {
    form.setFieldsValue({
      departmentId: record.departmentId,
      departmentName: record.departmentName,
      departmentCode: record.departmentCode,
      parentId: record.parentId ?? undefined,
      note: record.note ?? undefined,
      isActive: record.isActive,
      isDelete: record.isDelete,
    });
    setModalMode('edit');
    setModalOpen(true);
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      setSubmitting(true);
      if (modalMode === 'create') {
        await createDepartmentApi(values);
      } else {
        await updateDepartmentApi(values);
      }
      setModalOpen(false);
      load();
    } catch (err: unknown) {
      // Form validation error or API error — Ant Design handles form errors inline
      if (err instanceof Error) {
        Modal.error({ title: 'Lỗi', content: err.message });
      }
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (departmentId: string) => {
    try {
      await deleteDepartmentApi(departmentId);
      load();
    } catch (err: unknown) {
      Modal.error({ title: 'Lỗi xóa', content: err instanceof Error ? err.message : 'Xóa thất bại' });
    }
  };

  const openAudit = async (record: Department) => {
    setAuditTargetName(record.departmentName);
    setAuditOpen(true);
    setAuditLoading(true);
    try {
      const controller = new AbortController();
      const data = await fetchDepartmentAuditLogsApi(record.departmentId, 50, auditLang, controller.signal);
      setAuditLogs(data);
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.code === 'ERR_CANCELED') {
        return;
      }
      setAuditLogs([]);
      Modal.error({ title: 'Lỗi tải audit', content: err instanceof Error ? err.message : 'Không tải được lịch sử thay đổi' });
    } finally {
      setAuditLoading(false);
    }
  };

  const parentOptions = list.map((d) => ({
    value: d.departmentId,
    label: `${d.departmentCode} — ${d.departmentName}`,
  }));

  const columns: TableColumnsType<Department> = [
    {
      title: 'Mã phòng ban',
      dataIndex: 'departmentCode',
      width: 140,
      sorter: (a, b) => a.departmentCode.localeCompare(b.departmentCode),
    },
    {
      title: 'Tên phòng ban',
      dataIndex: 'departmentName',
      sorter: (a, b) => a.departmentName.localeCompare(b.departmentName),
    },
    {
      title: 'Cấp',
      dataIndex: 'level',
      width: 70,
      render: (level: number) => (
        <Tag className={styles.levelTag} color={level === 1 ? 'blue' : level === 2 ? 'cyan' : 'default'}>
          Cấp {level}
        </Tag>
      ),
    },
    {
      title: 'Ghi chú',
      dataIndex: 'note',
      ellipsis: true,
    },
    {
      title: 'Kích hoạt',
      dataIndex: 'isActive',
      width: 100,
      render: (v: boolean) => <Tag color={v ? 'success' : 'default'}>{v ? 'Hoạt động' : 'Dừng'}</Tag>,
    },
    {
      title: 'Hành động',
      width: 180,
      render: (_, record) => (
        <Space>
          <Button
            size="small"
            icon={<HistoryOutlined />}
            onClick={() => openAudit(record)}
          />
          <Button
            size="small"
            icon={<EditOutlined />}
            onClick={() => openEdit(record)}
          />
          <Popconfirm
            title="Xác nhận xóa?"
            description={`Xóa phòng ban "${record.departmentName}"?`}
            onConfirm={() => handleDelete(record.departmentId)}
            okText="Xóa"
            cancelText="Hủy"
            okButtonProps={{ danger: true }}
          >
            <Button size="small" danger icon={<DeleteOutlined />} />
          </Popconfirm>
        </Space>
      ),
    },
  ];

  const auditColumns: TableColumnsType<DepartmentAuditLog> = [
    {
      title: 'Trang',
      dataIndex: 'pageCode',
      width: 180,
      render: (value: string | null) => pageName(value),
    },
    {
      title: 'Thời gian',
      dataIndex: 'changedAtUtc',
      width: 190,
      render: (value: string) => value ? new Date(value).toLocaleString('vi-VN') : '-',
    },
    {
      title: 'Hành động',
      dataIndex: 'action',
      width: 120,
      render: (value: string) => <Tag color={value === 'Added' ? 'green' : value === 'Modified' ? 'blue' : 'volcano'}>{value || '-'}</Tag>,
    },
    {
      title: 'Field',
      width: 220,
      render: (_, record) => record.fieldLabel || record.fieldName || '-',
    },
    {
      title: 'Gia tri thay doi',
      render: (_, record) => (
        <div className={styles.changeCell}>
          <Typography.Text delete type="secondary" className={styles.oldValue}>
            {record.oldValue || '-'}
          </Typography.Text>
          <span className={styles.changeArrow}>{'->'}</span>
          <Typography.Text strong className={styles.newValue}>
            {record.newValue || '-'}
          </Typography.Text>
        </div>
      ),
    },
  ];

  return (
    <div>
      <div className={styles.toolbar}>
        <h2 className={styles.title}>Quản lý Phòng ban</h2>
        <Space>
          <Button icon={<ReloadOutlined />} onClick={() => load()} loading={status === 'loading'}>
            Tải lại
          </Button>
          <SyncAuditMetadataButton pageCode={PAGE_CODE} />
          <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>
            Thêm phòng ban
          </Button>
        </Space>
      </div>

      {error && <Alert type="error" message={error} showIcon style={{ marginBottom: 16 }} />}

      <Table<Department>
        rowKey="departmentId"
        columns={columns}
        dataSource={list}
        loading={status === 'loading'}
        pagination={{ pageSize: 20, showSizeChanger: true }}
        size="small"
        bordered
      />

      <Modal
        open={modalOpen}
        title={modalMode === 'create' ? 'Thêm phòng ban mới' : 'Cập nhật phòng ban'}
        okText={modalMode === 'create' ? 'Tạo' : 'Lưu'}
        cancelText="Hủy"
        onOk={handleSubmit}
        onCancel={() => setModalOpen(false)}
        confirmLoading={submitting}
        destroyOnHidden
      >
        <Form form={form} layout="vertical" autoComplete="off">
          <Form.Item name="departmentId" hidden>
            <Input />
          </Form.Item>

          <Form.Item
            label="Mã phòng ban"
            name="departmentCode"
            rules={[{ required: true, message: 'Vui lòng nhập mã phòng ban' }]}
          >
            <Input placeholder="VD: IT-001" disabled={modalMode === 'edit'} />
          </Form.Item>

          <Form.Item
            label="Tên phòng ban"
            name="departmentName"
            rules={[{ required: true, message: 'Vui lòng nhập tên phòng ban' }]}
          >
            <Input placeholder="VD: Phòng Công nghệ thông tin" />
          </Form.Item>

          <Form.Item label="Phòng ban cha" name="parentId">
            <Select
              allowClear
              placeholder="Chọn phòng ban cha (nếu có)"
              options={parentOptions}
              showSearch
              optionFilterProp="label"
            />
          </Form.Item>

          <Form.Item label="Ghi chú" name="note">
            <Input.TextArea rows={2} placeholder="Ghi chú..." />
          </Form.Item>

          {modalMode === 'edit' && (
            <Form.Item label="Kích hoạt" name="isActive" valuePropName="checked">
              <Switch checkedChildren="Hoạt động" unCheckedChildren="Dừng" />
            </Form.Item>
          )}
        </Form>
      </Modal>

      <Drawer
        open={auditOpen}
        title={`Lịch sử thay đổi: ${auditTargetName || 'Phòng ban'}`}
        placement="right"
        className={styles.auditDrawer}
        extra={<Tag color="blue">{auditLogs.length} ban ghi</Tag>}
        onClose={() => setAuditOpen(false)}
        width={980}
        destroyOnHidden
      >
        <Table<DepartmentAuditLog>
          rowKey="id"
          columns={auditColumns}
          dataSource={auditLogs}
          loading={auditLoading}
          pagination={{ pageSize: 8 }}
          size="small"
          scroll={{ x: 920 }}
          className={styles.auditTable}
        />
      </Drawer>
    </div>
  );
}
