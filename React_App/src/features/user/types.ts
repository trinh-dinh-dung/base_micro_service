/** Domain User — dùng chéo nhiều page (Home preview, /user detail). */
export interface User {
  userName: string;
  fullName: string;
  age: number;
  phone: string;
  address: string;
}

export type UserLoadStatus = 'idle' | 'loading' | 'succeeded' | 'failed';

export interface UserState {
  data: User | null;
  status: UserLoadStatus;
  error: string | null;
}
