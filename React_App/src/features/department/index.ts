export type { Department, DepartmentRequest, DepartmentState, DepartmentAuditLog } from './types';
export {
  fetchDepartmentsApi,
  createDepartmentApi,
  updateDepartmentApi,
  deleteDepartmentApi,
  fetchDepartmentAuditLogsApi,
} from './departmentApi';
export {
  setLoading,
  setDepartments,
  setFailed,
  selectDepartments,
  selectDepartmentStatus,
  selectDepartmentError,
} from './departmentSlice';
export { default as departmentReducer } from './departmentSlice';
