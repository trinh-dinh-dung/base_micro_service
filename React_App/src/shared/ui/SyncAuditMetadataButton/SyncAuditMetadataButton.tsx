import { useState } from 'react';
import { Button, message } from 'antd';
import { SyncOutlined } from '@ant-design/icons';
import { getAuditMetadataByPage } from '../../auditMetadata/metadataRegistry';
import { syncAuditMetadataApi } from '../../api/auditMetadataApi';

interface SyncAuditMetadataButtonProps {
  pageCode: string;
}

export default function SyncAuditMetadataButton({ pageCode }: SyncAuditMetadataButtonProps) {
  const [loading, setLoading] = useState(false);

  const handleSync = async () => {
    try {
      setLoading(true);
      const items = getAuditMetadataByPage(pageCode);
      if (items.length === 0) {
        message.warning('Khong tim thay metadata trong locale VI/EN cho page nay');
        return;
      }

      const changed = await syncAuditMetadataApi(pageCode, items);
      message.success(`Dong bo metadata thanh cong (${changed})`);
    } catch (err: unknown) {
      message.error(err instanceof Error ? err.message : 'Dong bo metadata that bai');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Button icon={<SyncOutlined />} onClick={handleSync} loading={loading}>
      Dong bo metadata
    </Button>
  );
}
