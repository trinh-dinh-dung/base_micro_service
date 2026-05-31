import { createSlice } from '@reduxjs/toolkit';
import { pingService } from './serviceHealthThunks';
import type { HomeServiceHealthState, ServiceKey } from './types';

const idle = { status: 'idle' as const };

const initialState: HomeServiceHealthState = {
  base: { ...idle },
  upload: { ...idle },
};

const serviceHealthSlice = createSlice({
  name: 'homeServiceHealth',
  initialState,
  reducers: {
    resetService(state, action: { payload: ServiceKey }) {
      state[action.payload] = { status: 'idle' };
    },
    resetAll(state) {
      state.base = { status: 'idle' };
      state.upload = { status: 'idle' };
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(pingService.pending, (state, action) => {
        state[action.meta.arg] = { status: 'loading' };
      })
      .addCase(pingService.fulfilled, (state, action) => {
        const { service, data } = action.payload;
        state[service] = { status: 'ok', data };
      })
      .addCase(pingService.rejected, (state, action) => {
        state[action.meta.arg] = {
          status: 'error',
          error: action.payload ?? action.error.message ?? 'Unknown error',
        };
      });
  },
});

export const { resetService, resetAll } = serviceHealthSlice.actions;
export default serviceHealthSlice.reducer;
