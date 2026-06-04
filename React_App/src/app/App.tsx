import { useEffect } from "react";
import { Route, Routes } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import { useAppDispatch } from "./hooks";
import HomePage from "../pages/home/HomePage";
import LoginPage from "../pages/login/LoginPage";
import LoginCallbackPage from "../pages/loginCallback/LoginCallbackPage";
import UserPage from "../pages/user/UserPage";
import DepartmentPage from "../pages/department/DepartmentPage";
import AdminLayout from "../shared/ui/AdminLayout/AdminLayout";
import ProtectedRoute from "../shared/ui/ProtectedRoute/ProtectedRoute";
import { clearAuth } from "../features/auth";

export default function App() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  useEffect(() => {
    const onUnauthorized = () => {
      dispatch(clearAuth());
      navigate("/login", { replace: true });
    };

    window.addEventListener("app:auth:unauthorized", onUnauthorized);
    return () => window.removeEventListener("app:auth:unauthorized", onUnauthorized);
  }, [dispatch, navigate]);

  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/login-callback" element={<LoginCallbackPage />} />
      <Route
        element={
          <ProtectedRoute>
            <AdminLayout />
          </ProtectedRoute>
        }
      >
        <Route path="/" element={<HomePage />} />
        <Route path="/user" element={<UserPage />} />
        <Route path="/department" element={<DepartmentPage />} />
      </Route>
    </Routes>
  );
}
