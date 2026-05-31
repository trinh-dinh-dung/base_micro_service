/**
 * Feature NGANG — domain User (chỉ state + hàm API).
 * Page tự gọi fetchUserFromApi() và dispatch setUser / setFailed.
 */
export { default as userReducer } from './userSlice';
export { setLoading, setUser, setFailed, clearUser } from './userSlice';
export { fetchUserFromApi } from './userApi';
export {
  selectUser,
  selectUserStatus,
  selectUserError,
  selectHasUser,
} from './userSelectors';
export type { User, UserState, UserLoadStatus } from './types';
