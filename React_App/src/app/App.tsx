import { Route, Routes } from "react-router-dom";
import HomePage from "../pages/home/HomePage";
import CallbackPage from "../pages/callback/CallbackPage";
import UserPage from "../pages/user/UserPage";
import DepartmentPage from "../pages/department/DepartmentPage";
import AdminLayout from "../shared/ui/AdminLayout/AdminLayout";

export default function App() {
  return (
    <Routes>
      <Route path="/callback" element={<CallbackPage />} />
      <Route element={<AdminLayout />}>
        <Route path="/" element={<HomePage />} />
        <Route path="/user" element={<UserPage />} />
        <Route path="/department" element={<DepartmentPage />} />
      </Route>
    </Routes>
  );
}
