import { createSlice, type PayloadAction } from '@reduxjs/toolkit';
import type { User, UserState } from './types';

const initialState: UserState = {
  data: null,
  status: 'idle',
  error: null,
};

const userSlice = createSlice({
  name: 'user',
  initialState,
  reducers: {
    setLoading(state) {
      state.status = 'loading';
      state.error = null;
    },
    setUser(state, action: PayloadAction<User>) {
      state.data = action.payload;
      state.status = 'succeeded';
      state.error = null;
    },
    setFailed(state, action: PayloadAction<string>) {
      state.status = 'failed';
      state.error = action.payload;
    },
    clearUser(state) {
      state.data = null;
      state.status = 'idle';
      state.error = null;
    },
  },
});

export const { setLoading, setUser, setFailed, clearUser } = userSlice.actions;
export default userSlice.reducer;
