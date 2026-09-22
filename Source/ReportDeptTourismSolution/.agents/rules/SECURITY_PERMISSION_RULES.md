# 🛡️ Quy chuẩn Phân quyền 2 Tầng & Bảo vệ Dữ liệu (SECURITY_PERMISSION_RULES)

Tài liệu này quy định kiến trúc phân quyền 2 tầng cho toàn bộ các module trong hệ thống CenIT TOC CRM, đặc biệt là các module quản lý giao dịch và hồ sơ kinh doanh:

---

## 1. Kiến trúc Phân quyền 2 Tầng (Two-Tier Authorization)

### Tầng 1: Phân quyền Hệ thống (System ActionType Authorization)
- Kiểm soát quyền truy cập chức năng chung qua `ActionTypeAttribute`:
  - `[ActionType(Type = EnumActionType.View)]`: Cho phép xem danh sách, màn hình chi tiết.
  - `[ActionType(Type = EnumActionType.Add)]`: Cho phép mở modal và thêm mới hồ sơ.
  - `[ActionType(Type = EnumActionType.Edit)]`: Cho phép cập nhật thông tin hồ sơ.
  - `[ActionType(Type = EnumActionType.Delete)]`: Cho phép xóa hồ sơ.
- Cơ chế kiểm tra qua Core Authorization: `AppProcessor.Author.IsAllow(HttpContext, User.UserName, Area, Controller, Action)`.

### Tầng 2: Phân quyền Bản ghi (Record-Level Authorization)
- Mỗi bản ghi có chủ sở hữu, người phụ trách và danh sách thành viên liên quan.
- **Điều kiện được phép Thao tác (Sửa / Xóa / Cập nhật tiến trình):**
  Một người dùng `U` chỉ được phép can thiệp vào bản ghi `R` khi thỏa mãn ÍT NHẤT một trong các điều kiện sau:
  1. `U` là Quản trị hệ thống (`IsUserQTHT == true`: User role `QTHT` hoặc RoleID 1).
  2. `U` là Người tạo bản ghi (`R.CreatedBy == U.UserName`).
  3. `U` là Nhân sự chủ trì (`R.AssignedEmployeeID == U.UserId`).
  4. `U` là Thành viên tham gia có quyền cập nhật trạng thái (`Member.IsAM == true`).

---

## 2. Quy chuẩn Hiển thị Thao tác trên Giao diện (UI Actions Rendering)
- Trên bảng dữ liệu DataTable và màn hình Chi tiết:
  - Chỉ hiển thị nút **Edit** và **Xóa** đối với người có đủ quyền (kết hợp cả Tầng 1 và Tầng 2).
  - Bản ghi đã khóa (`IsLocked == true` hoặc trạng thái hoàn thành cuối cùng):
    - Ẩn nút Xóa.
    - Nút Edit chuyển sang chế độ Xem (View Only), hiển thị icon khóa `fa-lock`.
- Trên các Tab Chi tiết (Sản phẩm, Thành viên, Tiến trình):
  - Kiểm tra `HasDetailPermission` trước khi hiển thị các nút "Thêm mới", "Chỉnh sửa", "Xóa".
  - Nếu không có quyền, hiển thị cảnh báo đẹp mắt từ `Sys_Messages` thay vì làm sập giao diện.

---

## 3. Quy chuẩn Bảo vệ Tầng Controller (Server-Side Gatekeeping)
- **CẤM CHỈ PHÂN QUYỀN TRÊN GIAO DIỆN:** Toàn bộ action POST/PUT/DELETE trên Controller BẮT BUỘC phải xác thực lại quyền trước khi ghi CSDL:
  ```csharp
  if (!HasDetailPermission(recordId, User.UserName))
  {
      return Json(new { 
          status = false, 
          message = AppProcessor.Messagor.GetMessage("DigitalSales_Msg_NoPermission") 
      });
  }
  ```
- Kiểm tra các ràng buộc chuyển đổi trạng thái nghiệp vụ (State Machine Gatekeeper): Không cho phép chuyển từ "Cơ hội" sang "Dự án" nếu chưa có ít nhất 1 sản phẩm dịch vụ và 1 thành viên tham gia.
