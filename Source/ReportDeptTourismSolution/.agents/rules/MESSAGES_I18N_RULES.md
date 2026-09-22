# 🌐 Quy chuẩn Đa ngôn ngữ & Quản lý Text Hệ thống (MESSAGES_I18N_RULES)

Tài liệu này quy định bắt buộc cho toàn bộ các lập trình viên và tác nhân AI khi viết mã trên hệ thống CenIT TOC CRM. Mọi text hiển thị cho người dùng, thông báo, nhãn trường đều PHẢI tuân thủ 100% các nguyên tắc sau:

---

## 1. Nguyên tắc Tuyệt đối về Quản lý Text (No Hardcoded UI Strings)
- **CẤM HARDCODE TIẾNG VIỆT HOẶC TIẾNG ANH TRONG CODE `.cs`:**
  - Tuyệt đối KHÔNG gán chuỗi text trực tiếp vào các biến thông báo, tiêu đề, lỗi validation:
    - ❌ *Sai:* `ViewBag.Title = "Cơ hội kinh doanh SPDV Số";`
    - ❌ *Sai:* `return Json(new { status = false, message = "Bạn không có quyền thực hiện!" });`
    - ❌ *Sai:* `ModelState.AddModelError("ShortName", "Vui lòng nhập tên viết tắt!");`
  - BẮT BUỘC lấy text thông qua bộ máy thông báo tập trung `Sys_Messages`:
    - ✅ *Đúng:* `ViewBag.Title = AppProcessor.Messagor.GetMessage("DigitalSales_Title");`
    - ✅ *Đúng:* `return Json(new { status = false, message = AppProcessor.Messagor.GetMessage("DigitalSales_Msg_NoPermission") });`
    - ✅ *Đúng:* `ModelState.AddModelError("ShortName", AppProcessor.Messagor.GetMessage("DigitalSales_Msg_ShortNameRequired"));`

---

## 2. Quy chuẩn Bảng `Sys_Messages` trong CSDL SQL Server
- Toàn bộ message keys được quản lý trong bảng `Sys_Messages`:
  - `LangCode`: Mã ngôn ngữ (`'vi-VN'`, `'en-US'`). Mặc định bắt buộc có `'vi-VN'`.
  - `LabelKey`: Khóa định danh (`VARCHAR(100)`), quy tắc đặt tên theo tiền tố module:
    - Tiêu đề màn hình: `[Module]_Title` (VD: `DigitalSales_Title`)
    - Loại hình/Thuộc tính: `[Module]_[Category]_[Item]` (VD: `DigitalSales_BusinessType_Opportunity`)
    - Thông báo hành động: `[Module]_Msg_[Action][Result]` (VD: `DigitalSales_Msg_SaveSuccess`, `DigitalSales_Msg_DeleteFail`)
    - Xác nhận thao tác: `[Module]_Msg_DeleteConfirm`, `[Module]_Msg_LockViewOnly`
    - Ràng buộc nghiệp vụ: `[Module]_Msg_[RuleName]` (VD: `DigitalSales_Msg_ReqProductBeforeProject`)
  - `Message`: Nội dung thông báo hiển thị cho người dùng (`NVARCHAR(1000)`).
- Hỗ trợ tham số động: Dùng cú pháp `{0}`, `{1}` chuẩn của .NET và gọi qua `string.Format(AppProcessor.Messagor.GetMessage("..."), arg0, arg1)`.

---

## 3. Quy chuẩn An toàn Model Binding & DisplayName (Anti-Null DisplayName)
- Khi sử dụng thuộc tính `[CustomDisplayName("LabelKey")]` hoặc `[DisplayName("LabelKey")]` trên các Model/ViewModel:
  - Bắt buộc kiểm tra `LabelKey` đã tồn tại trong `Sys_Messages`.
  - Hàm resolver tuyệt đối KHÔNG được trả về `null`. Nếu không tìm thấy key, phải trả về chính `LabelKey` thay vì để `null`, nhằm tránh lỗi sập `ArgumentNullException: Value cannot be null. Parameter name: value` khi ASP.NET MVC binding model.

---

## 4. Quy chuẩn Controller & Service Layer
- Mọi Controller kế thừa `AppController` phải triển khai helper nhất quán:
  ```csharp
  private string GetAppMessage(string labelKey, string defaultMessage = null)
  {
      var msg = AppProcessor.Messagor.GetMessage(labelKey);
      return !string.IsNullOrEmpty(msg) ? msg : (defaultMessage ?? labelKey);
  }
  ```
- Tuyệt đối không fallback về chuỗi tiếng Việt hardcoded trực tiếp trong file mã nguồn. Nếu cần fallback, chỉ fallback về chính `labelKey` để phát hiện thiếu message trong CSDL khi chạy test.
