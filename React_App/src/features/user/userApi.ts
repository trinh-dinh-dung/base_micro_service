import axios from 'axios';
import { gatewayClient } from '../../shared/api/gatewayClient';
import type { User } from './types';

interface UserInfoApiResponse {
  success: boolean;
  data: User;
}

/** Gọi API — không dispatch, chỉ trả data hoặc throw message lỗi. */
export async function fetchUserFromApi(): Promise<User> {
  try {
    const res = await gatewayClient.get<UserInfoApiResponse>('/api/base/api/UserInfo/me');
    if (!res.data.success || !res.data.data) {
      throw new Error('Invalid response from server');
    }
    return res.data.data;
  } catch (err: unknown) {
    if (axios.isAxiosError(err)) {
      throw new Error(
        `${err.response?.status ?? 'Network'}: ${JSON.stringify(err.response?.data ?? err.message)}`
      );
    }
    if (err instanceof Error) throw err;
    throw new Error(String(err));
  }
}
