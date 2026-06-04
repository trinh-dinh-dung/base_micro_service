import { configureStore } from '@reduxjs/toolkit';
import { authReducer } from '../features/auth';
import { userReducer } from '../features/user';
import { departmentReducer } from '../features/department';
import { homeServiceHealthReducer } from '../pages/home/model';

export const store = configureStore({
  reducer: {
    /** Global — SSO token mirror */
    auth: authReducer,
    /** Domain ngang — User (Home + /user) */
    user: userReducer,
    /** Phòng ban CRUD */
    department: departmentReducer,
    /** Chỉ Home — ping microservices */
    homeServiceHealth: homeServiceHealthReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
