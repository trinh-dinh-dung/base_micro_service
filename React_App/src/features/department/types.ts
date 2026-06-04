export interface Department {
  departmentId: string;
  departmentName: string;
  departmentCode: string;
  parentId: string | null;
  note: string | null;
  isActive: boolean;
  isDelete: boolean;
  createDate?: string | null;
  updateDate?: string | null;
  level?: number;
}

export interface DepartmentRequest {
  departmentId?: string;
  departmentName: string;
  departmentCode: string;
  parentId?: string | null;
  note?: string | null;
  isActive?: boolean;
  isDelete?: boolean;
}

export interface DepartmentAuditLog {
  id: string;
  action: string;
  entityKeys: string | null;
  fieldName: string | null;
  fieldLabel: string | null;
  oldValue: string | null;
  newValue: string | null;
  pageCode: string | null;
  requestMethod: string | null;
  requestPath: string | null;
  userName: string | null;
  traceId: string | null;
  statusCode: number | null;
  changedAtUtc: string;
}

export type DepartmentLoadStatus = 'idle' | 'loading' | 'succeeded' | 'failed';

export interface DepartmentState {
  list: Department[];
  status: DepartmentLoadStatus;
  error: string | null;
}
