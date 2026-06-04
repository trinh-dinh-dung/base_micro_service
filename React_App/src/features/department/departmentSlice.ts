import { createSlice, type PayloadAction } from '@reduxjs/toolkit';
import type { Department, DepartmentState } from './types';
import type { RootState } from '../../app/store';

const initialState: DepartmentState = {
  list: [],
  status: 'idle',
  error: null,
};

const departmentSlice = createSlice({
  name: 'department',
  initialState,
  reducers: {
    setLoading(state) {
      state.status = 'loading';
      state.error = null;
    },
    setDepartments(state, action: PayloadAction<Department[]>) {
      state.list = action.payload;
      state.status = 'succeeded';
      state.error = null;
    },
    setFailed(state, action: PayloadAction<string>) {
      state.status = 'failed';
      state.error = action.payload;
    },
  },
});

export const { setLoading, setDepartments, setFailed } = departmentSlice.actions;
export default departmentSlice.reducer;

// Selectors
export const selectDepartments = (state: RootState) => state.department.list;
export const selectDepartmentStatus = (state: RootState) => state.department.status;
export const selectDepartmentError = (state: RootState) => state.department.error;
