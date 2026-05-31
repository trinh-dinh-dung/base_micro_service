/** State chỉ dùng trên trang Home — ping health microservices. */
export type ServiceKey = 'base' | 'upload';
export type ServiceStatus = 'idle' | 'loading' | 'ok' | 'error';

export interface ServiceHealthState {
  status: ServiceStatus;
  data?: unknown;
  error?: string;
}

export interface HomeServiceHealthState {
  base: ServiceHealthState;
  upload: ServiceHealthState;
}

export const SERVICE_ENDPOINTS: Record<ServiceKey, string> = {
  base: '/api/base/api/FileBridge/files?entityType=health&entityId=ping',
  upload: '/api/upload/api/FileUpload/list',
};

export const SERVICE_META: Record<
  ServiceKey,
  { name: string; description: string; port: number }
> = {
  base: {
    name: 'Service_Base',
    description: 'Core business API (Clean Architecture)',
    port: 5000,
  },
  upload: {
    name: 'Service_Upload',
    description: 'File upload/download microservice',
    port: 5001,
  },
};
