import { gatewayClient } from './gatewayClient';

const BASE = '/api/base/api/department-service/department';

interface ApiResponse<T> {
  success?: boolean;
  isSuccess?: boolean;
  data: T;
  message?: string;
}

export interface AuditMetadataSyncItem {
  fieldName: string;
  labelVi: string;
  labelEn: string;
  translationKey: string;
}

function isApiSuccess<T>(response: ApiResponse<T>): boolean {
  if (typeof response.isSuccess === 'boolean') return response.isSuccess;
  if (typeof response.success === 'boolean') return response.success;
  return false;
}

export async function syncAuditMetadataApi(pageCode: string, items: AuditMetadataSyncItem[]): Promise<number> {
  const response = await gatewayClient.post<ApiResponse<number>>(`${BASE}/sync-audit-metadata`, {
    pageCode,
    items,
  }, {
    headers: { page_code: pageCode },
  });

  if (!isApiSuccess(response.data)) {
    throw new Error(response.data.message ?? 'Sync audit metadata failed');
  }

  return response.data.data ?? 0;
}
