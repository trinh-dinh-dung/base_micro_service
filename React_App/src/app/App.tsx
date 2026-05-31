import { Route, Routes } from "react-router-dom";
import HomePage from "../pages/home/HomePage";
import CallbackPage from "../pages/callback/CallbackPage";
import UserPage from "../pages/user/UserPage";

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route path="/user" element={<UserPage />} />
      <Route path="/callback" element={<CallbackPage />} />
    </Routes>
  );
}
