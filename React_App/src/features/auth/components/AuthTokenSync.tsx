/**
 * AuthTokenSync đã không còn cần thiết sau khi chuyển sang IAM flow.
 * Token được khởi tạo trực tiếp từ localStorage trong authSlice.
 * File này được giữ lại để tương thích ngược nếu có import cũ.
 */
export default function AuthTokenSync() {
  return null;
}

