import axios from 'axios';
import { gatewayClient } from '../../shared/api/gatewayClient';
import type { Department, DepartmentAuditLog, DepartmentRequest } from './types';

const PAGE_CODE = 'DEPARTMENT_MANAGEMENT';
const BASE = '/api/base/api/department-service/department';

interface ApiResponse<T> {
  success?: boolean;
  isSuccess?: boolean;
  data: T;
  message?: string;
}

interface RawDepartment {
  DepartmentId?: string;
  DepartmentName?: string;
  DepartmentCode?: string;
  ParentId?: string | null;
  Note?: string | null;
  IsActive?: boolean;
  IsDelete?: boolean;
  Level?: number;
  departmentId?: string;
  departmentName?: string;
  departmentCode?: string;
  parentId?: string | null;
  note?: string | null;
  isActive?: boolean;
  isDelete?: boolean;
  level?: number;
}

interface RawDepartmentAuditLog {
  Id?: string;
  Action?: string;
  EntityKeys?: string | null;
  FieldName?: string | null;
  FieldLabel?: string | null;
  OldValue?: string | null;
  NewValue?: string | null;
  PageCode?: string | null;
  RequestMethod?: string | null;
  RequestPath?: string | null;
  UserName?: string | null;
  TraceId?: string | null;
  StatusCode?: number | null;
  ChangedAtUtc?: string;
  id?: string;
  action?: string;
  entityKeys?: string | null;
  fieldName?: string | null;
  fieldLabel?: string | null;
  oldValue?: string | null;
  newValue?: string | null;
  pageCode?: string | null;
  requestMethod?: string | null;
  requestPath?: string | null;
  userName?: string | null;
  traceId?: string | null;
  statusCode?: number | null;
  changedAtUtc?: string;
}

function headers() {
  return { page_code: PAGE_CODE };
}

function isApiSuccess<T>(response: ApiResponse<T>): boolean {
  if (typeof response.isSuccess === 'boolean') return response.isSuccess;
  if (typeof response.success === 'boolean') return response.success;
  return false;
}

function normalizeParentId(parentId?: string | null): string | null {
  if (!parentId) return null;
  return parentId === '00000000-0000-0000-0000-000000000000' ? null : parentId;
}

function normalizeDepartment(item: RawDepartment): Department {
  return {
    departmentId: item.departmentId ?? item.DepartmentId ?? '',
    departmentName: item.departmentName ?? item.DepartmentName ?? '',
    departmentCode: item.departmentCode ?? item.DepartmentCode ?? '',
    parentId: normalizeParentId(item.parentId ?? item.ParentId ?? null),
    note: item.note ?? item.Note ?? null,
    isActive: item.isActive ?? item.IsActive ?? true,
    isDelete: item.isDelete ?? item.IsDelete ?? false,
    level: item.level ?? item.Level,
  };
}

function normalizeDepartmentAuditLog(item: RawDepartmentAuditLog): DepartmentAuditLog {
  return {
    id: item.id ?? item.Id ?? '',
    action: item.action ?? item.Action ?? '',
    entityKeys: item.entityKeys ?? item.EntityKeys ?? null,
    fieldName: item.fieldName ?? item.FieldName ?? null,
    fieldLabel: item.fieldLabel ?? item.FieldLabel ?? null,
    oldValue: item.oldValue ?? item.OldValue ?? null,
    newValue: item.newValue ?? item.NewValue ?? null,
    pageCode: item.pageCode ?? item.PageCode ?? null,
    requestMethod: item.requestMethod ?? item.RequestMethod ?? null,
    requestPath: item.requestPath ?? item.RequestPath ?? null,
    userName: item.userName ?? item.UserName ?? null,
    traceId: item.traceId ?? item.TraceId ?? null,
    statusCode: item.statusCode ?? item.StatusCode ?? null,
    changedAtUtc: item.changedAtUtc ?? item.ChangedAtUtc ?? '',
  };
}

export async function fetchDepartmentsApi(parentId?: string | null, signal?: AbortSignal): Promise<Department[]> {
  try {
    const params = parentId ? { parentId } : {};
    const res = await gatewayClient.get<ApiResponse<RawDepartment[]>>(
      `${BASE}/get-list-department-by-parent-id`,
      { params, headers: headers(), signal }
    );
    if (!isApiSuccess(res.data)) throw new Error(res.data.message ?? 'Failed to fetch departments');
    return (res.data.data ?? []).map(normalizeDepartment);
  } catch (err: unknown) {
    if (axios.isAxiosError(err)) throw new Error(`${err.response?.status ?? 'Network'}: ${JSON.stringify(err.response?.data ?? err.message)}`);
    if (err instanceof Error) throw err;
    throw new Error(String(err));
  }
}

export async function createDepartmentApi(req: DepartmentRequest): Promise<boolean> {
  try {
    const res = await gatewayClient.post<ApiResponse<boolean>>(`${BASE}/create`, req, { headers: headers() });
    if (!isApiSuccess(res.data)) throw new Error(res.data.message ?? 'Failed to create department');
    return res.data.data;
  } catch (err: unknown) {
    if (axios.isAxiosError(err)) throw new Error(`${err.response?.status ?? 'Network'}: ${JSON.stringify(err.response?.data ?? err.message)}`);
    if (err instanceof Error) throw err;
    throw new Error(String(err));
  }
}

export async function updateDepartmentApi(req: DepartmentRequest): Promise<boolean> {
  try {
    const res = await gatewayClient.post<ApiResponse<boolean>>(`${BASE}/update`, req, { headers: headers() });
    if (!isApiSuccess(res.data)) throw new Error(res.data.message ?? 'Failed to update department');
    return res.data.data;
  } catch (err: unknown) {
    if (axios.isAxiosError(err)) throw new Error(`${err.response?.status ?? 'Network'}: ${JSON.stringify(err.response?.data ?? err.message)}`);
    if (err instanceof Error) throw err;
    throw new Error(String(err));
  }
}

export async function deleteDepartmentApi(departmentId: string): Promise<boolean> {
  try {
    const res = await gatewayClient.get<ApiResponse<boolean>>(`${BASE}/delete`, {
      params: { departmentId },
      headers: headers(),
    });
    if (!isApiSuccess(res.data)) throw new Error(res.data.message ?? 'Failed to delete department');
    return res.data.data;
  } catch (err: unknown) {
    if (axios.isAxiosError(err)) throw new Error(`${err.response?.status ?? 'Network'}: ${JSON.stringify(err.response?.data ?? err.message)}`);
    if (err instanceof Error) throw err;
    throw new Error(String(err));
  }
}

export async function fetchDepartmentAuditLogsApi(departmentId: string, limit = 20, lang = 'vi', signal?: AbortSignal): Promise<DepartmentAuditLog[]> {
  try {
    const res = await gatewayClient.get<ApiResponse<RawDepartmentAuditLog[]>>(`${BASE}/get-audit-logs`, {
      params: { departmentId, limit, lang },
      headers: headers(),
      signal,
    });
    if (!isApiSuccess(res.data)) throw new Error(res.data.message ?? 'Failed to fetch department audit logs');
    return (res.data.data ?? []).map(normalizeDepartmentAuditLog);
  } catch (err: unknown) {
    if (axios.isAxiosError(err)) throw new Error(`${err.response?.status ?? 'Network'}: ${JSON.stringify(err.response?.data ?? err.message)}`);
    if (err instanceof Error) throw err;
    throw new Error(String(err));
  }
}
