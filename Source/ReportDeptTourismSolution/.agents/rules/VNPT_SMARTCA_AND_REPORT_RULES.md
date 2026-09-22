# 🔏 QUY CHUẨN KÝ SỐ VNPT SMARTCA & XỬ LÝ BÁO CÁO (REPORT & DIGITAL SIGN RULES)

> **Áp dụng cho:** Phân hệ Báo cáo Thống kê (`Modules.ReportModule`), Ký số điện tử (`CenIT.Libs.VNPTSmartCA`), và Quản lý Mẫu Báo cáo / RDLC Reports.  
> **Mục tiêu:** Đảm bảo an toàn luồng dữ liệu ký số, tính toàn vẹn của tệp báo cáo điện tử và định dạng file xuất bản.

---

## 1. QUY CHUẨN TÍCH HỢP KÝ SỐ TỪ XA (VNPT SMARTCA)

### 1.1. Luồng ký số tiêu chuẩn (Signing Workflow)
1. **Khởi tạo giao dịch ký:** Client gọi action ký số tương ứng trong Controller (`AccountController` hoặc `ReportController`).
2. **Xác thực chứng thư số:** Kiểm tra tính hợp lệ của token và chứng thư qua `VNPTSmartCA.Providers.SmartCAProvider`.
3. **Ký Hash tài liệu:** Sử dụng `VnptHashSignatures` để tạo chữ ký điện tử trên file PDF/Báo cáo.
4. **Lưu trữ tài liệu đã ký:**
   - Tệp tin đã ký số **BẮT BUỘC** lưu trữ tại thư mục an toàn: `/Contents/Modules/Report/ReportSignedDocs/`.
   - Tên tệp phải kèm mã định danh duy nhất (UUID hoặc Timestamp) để chống trùng lặp và ghi đè trái phép.

### 1.2. Xử lý lỗi & Timeout trong Ký số
- Giao tiếp ký số từ xa phụ thuộc hạ tầng mạng và thiết bị di động (app SmartCA) của người dùng:
  - Bắt buộc xử lý timeout tối thiểu 60s cho bước phê duyệt ký trên mobile app.
  - Khi người dùng từ chối hoặc hết hạn giao dịch, phải giải phóng session và trả về thông báo lỗi rõ ràng qua `AppProcessor.Messagor`.

---

## 2. QUY CHUẨN XUẤT BẢN & MẪU BÁO CÁO THỐNG KÊ (REPORT ENGINE)

### 2.1. Quản lý Mẫu Báo cáo Excel
- Toàn bộ tệp Excel mẫu (`.xlsx`) lưu trữ tại: `/Contents/Modules/Report/Templates/`.
- Khi đọc/ghi file Excel qua thư viện `OfficeOpenXml` (EPPlus):
  - Phải kiểm tra cấu trúc sheet và tiêu đề cột trước khi đọc dữ liệu vào hệ thống.
  - Sử dụng block `using (var package = new ExcelPackage(fileStream))` để đảm bảo giải phóng bộ nhớ và tránh lock file.

### 2.2. Báo cáo RDLC & Microsoft ReportViewer
- Các project báo cáo chuyên biệt:
  - `CenIT.ReportTourism.Reports.UocKetQuaHoatDongKinhDoanh`
  - `CenIT.ReportTourism.Reports.ThongKeQuocTichKhachDuLich`
- Tệp RDLC và Designer `.cs` phải được biên dịch và copy tự động sang `WebApp/Contents/Modules/Report/Reports/` qua PostBuildEvent.
- Không chỉnh sửa file `*.Designer.cs` bằng tay; dùng Visual Studio Report Designer để cập nhật Dataset và layout.
