import { createAsyncThunk } from '@reduxjs/toolkit';
import axios from 'axios';
import { gatewayClient } from '../../../shared/api/gatewayClient';
import { SERVICE_ENDPOINTS, type ServiceKey } from './types';

export const pingService = createAsyncThunk<
  { service: ServiceKey; data: unknown },
  ServiceKey,
  { rejectValue: string }
>('homeServiceHealth/pingService', async (service, { rejectWithValue }) => {
  try {
    const res = await gatewayClient.get(SERVICE_ENDPOINTS[service]);
    return { service, data: res.data };
  } catch (err: unknown) {
    const msg = axios.isAxiosError(err)
      ? `${err.response?.status ?? 'Network'}: ${JSON.stringify(err.response?.data ?? err.message)}`
      : err instanceof Error
        ? err.message
        : String(err);
    return rejectWithValue(msg);
  }
});
