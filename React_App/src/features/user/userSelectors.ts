import type { RootState } from '../../app/store';

export const selectUser = (state: RootState) => state.user.data;
export const selectUserStatus = (state: RootState) => state.user.status;
export const selectUserError = (state: RootState) => state.user.error;
export const selectHasUser = (state: RootState) => state.user.data !== null;
