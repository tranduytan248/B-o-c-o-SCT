---
name: mvc-ui-engineering
description: Chuyên gia xây dựng và chuẩn hóa giao diện người dùng ASP.NET MVC 5 (CenIT ReportTourism). Áp dụng triết lý frontend-ui-engineering từ Addy Osmani vào hệ thống Razor Views, Form Controls (@Html.*), DataTable AdminLTE 2.3.11, quy chuẩn kích thước Modal (data_width 1024px/800px/600px), và xử lý phản hồi AJAX JavaScript khép kín. Sử dụng khi tạo mới hoặc chỉnh sửa View, Partial View, Modal, hoặc xử lý tương tác Form trên hệ thống Báo cáo Du lịch.
---

# 🎨 Skill: MVC UI Engineering — Xây Dựng Giao Diện Chuẩn CenIT ReportTourism

> **Cảm hứng & Triết lý:** Kế thừa nguyên lý `frontend-ui-engineering` từ [Addy Osmani's Agent Skills](https://github.com/addyosmani/agent-skills), kết hợp chuẩn mực phân hệ chuẩn `Cate/Enterprise` của CenIT ReportTourism.  
> **Mục tiêu:** Xây dựng giao diện web đẳng cấp, sạch sẽ, đạt chuẩn phân tầng, tuyệt đối không tạo mã nguồn cẩu thả, không dùng thẻ HTML thuần cho controls, và quản lý vòng đời Modal & AJAX không một kẽ hở.

---

## 🎯 Khi Nào Kích Hoạt Skill Này?

- Tạo mới hoặc cập nhật View (`Index.cshtml`), Partial View (`_Search.cshtml`, `_Add.cshtml`, `_Edit.cshtml`, `_Delete.cshtml`, `_[Entity]_View.cshtml`).
- Thiết kế bố cục Modal, thiết lập kích thước chuẩn `data_width`.
- Xử lý Form nhập liệu, validation client/server, binding dữ liệu với C# Model.
- Viết hoặc sửa đổi JavaScript tương tác bảng dữ liệu DataTable hoặc callback Form (`_OnProcessSuccess`).
- Refactor hoặc tối ưu trải nghiệm tương tác (UX) trên giao diện Web quản trị.

---

## 🏗️ Kiến Trúc View Phân Tầng CenIT TOC

Mỗi phân hệ (Entity) trên Web tuân thủ cấu trúc 6 file View và 1 file JS đồng bộ:

```
Areas/[AreaName]/Views/[Entity]/
├── Index.cshtml             # Màn hình chính: PageTitle, PageAction (@Html.Button data_width), DataTable
├── _Search.cshtml           # Khối tìm kiếm: Form lọc, @Html.DropDownListFor, @Html.TextBoxFor
├── _Add.cshtml              # Modal Thêm mới: Layout = "~/Views/Shared/_Form.cshtml", ajaxForm submit
├── _Edit.cshtml             # Modal Chỉnh sửa: Layout = "~/Views/Shared/_Form.cshtml", nạp dữ liệu cũ
├── _Delete.cshtml           # Modal Xác nhận Xóa: Layout = "~/Views/Shared/_FormConfirm.cshtml"
├── _[Entity]_View.cshtml    # Partial View ruột chứa form controls (được bọc trong <div id="bodyForm">)
└── [Entity].js              # JavaScript điều khiển DataTable, Search, và [Entity]_OnProcessSuccess
```

---

## 💎 Quy Tắc Bất Di Bất Dịch Về Form Controls

### 1. Cấm Tuyệt Đối Thẻ HTML Thuần
**Không bao giờ** viết `<label>`, `<input>`, `<select>`, `<textarea>` trực tiếp khi hệ thống đã có `@Html.*` tương ứng:
- ✅ Nhãn trường bắt buộc có sao đỏ: `@Html.TitleFor(m => m.FieldName, new { @class = "col-sm-2 col-form-label text-sm-right pr-0 font-bold" })`
- ✅ Ô nhập text: `@Html.TextBoxFor(m => m.FieldName, new { @class = "form-control", placeholder = "..." })`
- ✅ Dropdown chọn: `@Html.DropDownListFor(m => m.FieldID, new SelectList(Model.ListOptions, "Value", "Text"), "-- Chọn --", new { @class = "form-control" })`
- ✅ Nhập mô tả nhiều dòng: `@Html.TextAreaFor(m => m.Description, new { @class = "form-control", rows = 3, placeholder = "..." })`
- ✅ Đính kèm tệp tin: `@Html.FileFor(m => m.DinhKemFile, ".*", true, new { @class = "form-control", placeholder = "Đính kèm tệp tin" })`
- ✅ Thông báo lỗi xác thực: `@Html.ValidationMessageFor(m => m.FieldName, "", new { @class = "text-danger" })`
- ✅ Khóa bí mật CSRF: `@Html.AntiForgeryToken()` ở đầu mỗi Form.

### 2. Tiêu Chuẩn Kích Thước Modal Chuẩn (`data_width`)
- **`data_width = "1024px"`**: Áp dụng cho Form lớn, bố cục 2 cột, nhiều tab, bảng con, đính kèm file (Thêm/Sửa phân hệ chính).
- **`data_width = "800px"`**: Áp dụng cho Form trung bình 1 cột, 6 - 10 trường nhập liệu.
- **`data_width = "600px"`**: Áp dụng cho Modal xác nhận Xóa (`_Delete.cshtml`) hoặc cập nhật nhanh trạng thái.

---

## ⚡ Quy Chuẩn JavaScript Callback Toàn Diện (`_OnProcessSuccess`)

Hàm callback `[Entity]_OnProcessSuccess(response, formId)` phải xử lý **toàn diện 3 kịch bản**, không được bỏ sót bất kỳ trường hợp nào:

```javascript
function [Entity]_OnProcessSuccess(response, formId) {
    // KỊCH BẢN 1: Server trả về JSON (Có response.status) -> Thành công hoặc Lỗi nghiệp vụ
    if (response.status != undefined) {
        // 1.1. Người dùng đánh dấu tích "Tiếp tục tạo mới / Không đóng modal" (#chkNotDismissModal)
        if ($("#ModalContent #modal_" + formId + " #chkNotDismissModal").is(":checked")) {
            eval(response.message); // Kích hoạt Toast thông báo
            if (typeof _table[Entity] !== "undefined" && _table[Entity]) {
                _table[Entity].ajax.reload(null, false); // Reload dữ liệu tại trang hiện tại
            }
            response.status = undefined;
            // Tải lại nội dung form rỗng để tiếp tục nhập
            var urlAction = $("#ModalContent #modal_" + formId + " form").attr("action");
            $("#ModalContent #modal_" + formId + " #modal-content").load(urlAction, function () {
                if (typeof _initElement === "function") _initElement();
            });
        } 
        // 1.2. Người dùng không tích "Tiếp tục tạo mới" -> Đóng modal chuẩn
        else {
            // Nếu là lỗi nghiệp vụ (status = false, errorCode = 1): Báo lỗi và giữ nguyên modal để sửa
            if (!response.status && response.errorCode == 1) {
                eval(response.message);
                response.status = undefined;
            } else {
                $("#ModalContent #modal_" + formId).modal("hide");
                // ĐỢI MODAL ẨN HOÀN TOÀN MỚI HIỂN THỊ TOAST VÀ RELOAD BẢNG (Chống kẹt backdrop)
                $("#ModalContent #modal_" + formId).on("hidden.bs.modal", function () {
                    if (response.status != undefined) {
                        eval(response.message);
                        if (typeof _table[Entity] !== "undefined" && _table[Entity]) {
                            _table[Entity].ajax.reload(null, false);
                        }
                        if (typeof reload[Entity]Detail === "function") reload[Entity]Detail();
                        response.status = undefined;
                    }
                });
            }
        }
    } 
    // KỊCH BẢN 2: Server trả về HTML Partial View (Do ModelState.IsValid == false):
    // Thay thế toàn bộ HTML vào thẻ #bodyForm để hiển thị lỗi validation đỏ mà KHÔNG làm mất dữ liệu đã nhập!
    else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}
```

---

## 🚫 Bảng Chống Bao Biện (Anti-Rationalization Table)

| Lý lẽ AI hay bao biện để làm ẩu | Sự thật & Hậu quả kỹ thuật | Kỷ luật bắt buộc thi hành |
| :--- | :--- | :--- |
| *"Dùng `<input>` hay `<label>` thuần viết nhanh hơn, đỡ phải nhớ cú pháp `@Html.*`."* | Mất hoàn toàn Model Binding, mất dấu sao đỏ `(*)` tự động từ Model, mất tự động hiển thị lỗi validation đỏ. | **BẮT BUỘC 100% dùng `@Html.*`**. Chỉ dùng thẻ HTML thuần khi cả source code không có helper tương đương. |
| *"Không cần gán `data_width`, để modal tự co giãn theo trình duyệt."* | Modal bị co rúm, form 2 cột bị méo mó, chữ dài làm biến dạng form. | **BẮT BUỘC chỉ định `data_width` rõ ràng: `1024px`, `800px`, hoặc `600px`**. |
| *"Bỏ qua thẻ bọc `<div id="bodyForm">` trong modal Add/Edit."* | Khi nhập sai thông tin, server trả về lỗi validation thì Javascript không biết nhét vào đâu, làm form đơ cứng hoặc tải lại trang gây mất trắng dữ liệu người dùng. | **BẮT BUỘC bọc Partial View ruột trong `<div id="bodyForm">`**. |
| *"Gọi `eval(response.message)` ngay trước khi `modal('hide')` cho tiện."* | Xung đột luồng giao diện làm Bootstrap Modal bị treo lớp backdrop mờ đen che toàn bộ màn hình, bắt người dùng phải reload trang. | **BẮT BUỘC lồng `eval(response.message)` trong sự kiện `hidden.bs.modal`**. |
| *"Không cần kiểm tra `#chkNotDismissModal`, chỉ cần đóng modal là xong."* | Phá vỡ tính năng thêm liên tiếp nhiều bản ghi của CenIT TOC, gây ức chế cho nhân viên nghiệp vụ khi phải mở lại modal nhiều lần. | **BẮT BUỘC hỗ trợ nhánh `#chkNotDismissModal` trong `_OnProcessSuccess`**. |
| *"Lưu file UTF-8 không BOM cũng chạy tốt trên máy cá nhân."* | Khi chạy trên IIS / ASP.NET MVC Razor production, các ký tự tiếng Việt có dấu sẽ bị biến thành ``, `?` hoặc lỗi biên dịch. | **BẮT BUỘC 100% file `.cshtml`, `.js`, `.cs` phải lưu định dạng UTF-8 with BOM (`0xEF, 0xBB, 0xBF`)**. |

---

## 📋 Checklist Kiểm Thử View & Giao Diện Trước Khi Bàn Giao

- [ ] **Encoding:** File được lưu đúng định dạng **UTF-8 with BOM**.
- [ ] **Form Controls:** 100% sử dụng `@Html.TitleFor`, `@Html.TextBoxFor`, `@Html.DropDownListFor`, `@Html.TextAreaFor`, `@Html.FileFor`. Không còn thẻ HTML thuần nào trong các ô nhập liệu.
- [ ] **Validation:** Có `@Html.ValidationMessageFor` màu đỏ dưới mọi ô nhập liệu bắt buộc.
- [ ] **Modal Width:** Nút mở modal có chỉ định `data_width = "1024px"` (form lớn), `800px` (form vừa) hoặc `600px` (modal xóa).
- [ ] **Form Layout:** Kế thừa đúng `Layout = "~/Views/Shared/_Form.cshtml"` hoặc `_FormConfirm.cshtml`.
- [ ] **Thẻ Body Form:** Có thẻ `<div id="bodyForm">` bọc PartialView ruột form.
- [ ] **JS OnProcessSuccess:** Cài đặt đủ 3 nhánh: (1) Giữ modal thêm tiếp, (2) Đóng modal và hiển thị toast trong `hidden.bs.modal`, (3) Đè HTML vào `#bodyForm` khi lỗi validation.
- [ ] **DataTable:** Thead dùng `sticky-nav`, cột số thứ tự tự động tăng theo phân trang, thao tác `reload(null, false)`.
