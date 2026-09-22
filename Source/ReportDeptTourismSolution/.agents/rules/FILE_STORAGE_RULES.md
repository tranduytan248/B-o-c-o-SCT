# 📁 Quy chuẩn Quản lý & Lưu trữ File trên Hệ thống (FILE_STORAGE_RULES)

Tài liệu này quy định bắt buộc cho việc upload, lưu trữ và bảo mật tệp tin trên hệ thống CenIT TOC CRM:

---

## 1. Vị trí Lưu trữ Thống nhất (Directory Structure)
- **CẤM LƯU FILE TỰ DO:** Tuyệt đối không lưu file bừa bãi vào root `/Contents/` hoặc các thư mục không được phân định rõ ràng.
- **Thư mục cơ sở:** Toàn bộ tệp tin tải lên bởi người dùng PHẢI được lưu trữ tại:
  ```
  /Contents/Uploads/[Tên_Module]/[yyyyMM]/
  ```
  Ví dụ cho module DigitalSales:
  - Đường dẫn gốc: `/Contents/Uploads/DigitalSales`
  - Đường dẫn con theo tháng: `/Contents/Uploads/DigitalSales/202609/`
- Tự động tạo thư mục: Hệ thống phải tự động kiểm tra `Directory.Exists(physicalPath)` và tạo mới nếu chưa tồn tại trước khi lưu file.

---

## 2. Quy tắc Đặt tên File An toàn (Safe File Naming)
- Không lưu nguyên bản tên file từ người dùng tải lên (tránh trùng lặp tên, ký tự đặc biệt, dấu cách, tiếng Việt có dấu).
- Quy tắc định danh file an toàn:
  ```csharp
  var originalName = Path.GetFileNameWithoutExtension(file.FileName);
  var ext = Path.GetExtension(file.FileName);
  var safeName = UtilString.ConvertToUnSign(originalName) + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
  ```
- Kết quả lưu CSDL: Chỉ lưu **đường dẫn tương đối** dạng `/Contents/Uploads/DigitalSales/202609/filename.ext`, tuyệt đối không lưu đường dẫn tuyệt đối ổ đĩa cứng (như `D:\SVN\...`).

---

## 3. Bảo mật & Chặn File Độc hại (File Security & Antivirus Prevention)
- **DANH SÁCH EXTENSION CẤM TUYỆT ĐỐI (Blacklist Extensions):**
  - Mọi controller/service xử lý upload file PHẢI chặn các extension thực thi nguy hiểm:
    ```csharp
    var forbiddenExts = new[] { 
        ".exe", ".dll", ".bat", ".cmd", ".vbs", ".ps1", 
        ".sh", ".com", ".msi", ".vbe", ".jse", ".wsf", 
        ".wsh", ".scr", ".pif", ".jar", ".app", ".gadget" 
    };
    ```
  - Nếu phát hiện extension cấm, BẮT BUỘC ném ngoại lệ hoặc trả về mã lỗi bảo mật với thông báo từ `Sys_Messages` (`DigitalSales_Msg_InvalidFileFormat`).
- **Giới hạn kích thước file:** Kiểm tra dung lượng file tải lên không được vượt quá cấu hình tối đa của hệ thống (mặc định 50MB cho văn bản đính kèm).
