# 📋 QUY CHUẨN FORM & NHẬP LIỆU (MVC FORM RULES)

> **Tài liệu tham chiếu chuẩn:** Module Doanh nghiệp (`Cate/Enterprise`) & Quản lý Lưu trú (`Cate/Accommodation`).  
> **Áp dụng cho:** Toàn bộ form nhập liệu, thêm mới, cập nhật trong giải pháp CenIT ReportTourism.

---

## 1. MÔ HÌNH FORM DÙNG CHUNG (SHARED FORM PATTERN)

### 1.1. Nguyên tắc cốt lõi: Single Source of Truth
- **TUYỆT ĐỐI KHÔNG** sao chép toàn bộ giao diện form thành 2 file riêng biệt `_Add.cshtml` và `_Edit.cshtml` lớn hàng trăm dòng.
- Việc nhân bản mã nguồn dẫn tới:
  - Hiện tượng "lệch pha": sửa ở `_Add` nhưng quên ở `_Edit`.
  - Dễ gây xung đột ID DOM hoặc sai lệch nhãn đa ngữ.
  - Tốn gấp đôi công sức bảo trì và kiểm thử.

### 1.2. Cấu trúc 3 tệp chuẩn CenIT ReportTourism
Một chức năng quản lý thực thể (Entity) chuẩn mực bao gồm đúng 3 tệp View:

```
Areas/Cate/Views/[EntityName]/
├── _[EntityName].cshtml    <-- Chứa 100% Giao diện Content Form dùng chung
├── _Add.cshtml             <-- Vỏ bọc modal Thêm mới (kế thừa _Form.cshtml)
└── _Edit.cshtml            <-- Vỏ bọc modal Cập nhật (kế thừa _Form.cshtml)
```

#### A. Tệp Content dùng chung: `_[EntityName].cshtml`
- Chứa toàn bộ các dòng `row`, thẻ `box`, các ô `@Html.TextBoxFor`, `@Html.DropDownListFor`, `@Html.ValidationMessageFor`, file upload.
- Với các trường chỉ hiển thị khi Cập nhật:
  ```razor
  @if (Model.EnterpriseId > 0)
  {
      <!-- Các trường chỉ áp dụng cho Edit -->
  }
  ```

#### B. Tệp Vỏ bọc Thêm mới: `_Add.cshtml` (Khoảng 25 - 30 dòng)
```razor
@using TSFramework.App.Processors
@model CenIT.ReportTourism.Models.Cate.CateEnterpriseModel
@{
    ViewBag.Title = string.Format(AppProcessor.Messagor.GetMessage("Modal_Title_Add"), AppProcessor.Messagor.GetMessage("Enterprise_Title"));
    Layout = "~/Views/Shared/_Form.cshtml";
}
@section FormType { bg-primary }
@section FormIcon { <i class="fa fa-plus"></i> }
@section FormBody {
    @using (Html.BeginForm("Add", "Enterprise", new { area = "Cate" }, FormMethod.Post, new { encType = "multipart/form-data", id = "AddEnterprise", @class = "form-horizontal" }))
    {
        <div id="bodyForm">
            @Html.Partial("_Customer", Model)
        </div>
    }

    <script type="text/javascript">
        $('form#AddCustomer').ajaxForm({
            success: function (response) {
                Customer_OnProcessSuccess(response, "AddCustomer");
            }
        });
    </script>
}
```

#### C. Tệp Vỏ bọc Cập nhật: `_Edit.cshtml` (Khoảng 25 - 30 dòng)
```razor
@using TSFramework.Libs.Processors
@model Core.Cate.Models.RM_CustomerModel
@{
    ViewBag.Title = string.Format(AppProcessor.Messagor.GetMessage("Modal_Title_Edit"), AppProcessor.Messagor.GetMessage("Customer_Title"));
    Layout = "~/Views/Shared/_Form.cshtml";
}
@section FormType { primary }
@section FormIcon { <i class="far fa-edit"></i> }
@section FormBody {
    @using (Html.BeginForm("Edit", "Customer", new { area = "Cate" }, FormMethod.Post, new { encType = "multipart/form-data", id = "EditCustomer", @class = "form-horizontal" }))
    {
        <div id="bodyForm">
            @Html.Partial("_Customer", Model)
        </div>
    }

    <script type="text/javascript">
        $('form#EditCustomer').ajaxForm({
            success: function (response) {
                Customer_OnProcessSuccess(response, "EditCustomer");
            }
        });
    </script>
}
```

---

## 2. QUY CHUẨN XÁC THỰC DỮ LIỆU KÉP (DUAL-LAYER VALIDATION)

### 2.1. Phân định rõ 2 tầng thông báo lỗi
1. **Lỗi xác thực trường nhập liệu (Field-Level Validation):**
   - Bắt buộc hiển thị trực tiếp dòng chữ đỏ chỉ điểm ngay dưới chân control nhập liệu qua thẻ:
     ```razor
     @Html.ValidationMessageFor(model => model.FieldName, "", new { @class = "text-danger text-85" })
     ```
   - **NGHIÊM CẤM** việc bắn popup/Toastr chung chung khi người dùng thiếu các trường bắt buộc trên Form. Người dùng phải nhìn thấy lỗi đỏ ngay tại ô bị sai.
2. **Lỗi cấp hệ thống hoặc Kết quả hoàn thành (System Action Message):**
   - Chỉ dùng Toastr / Popup cho kết quả thực thi (Thành công, Thất bại khi ghi DB, Trùng mã trong DB, Mất kết nối DB).

### 2.2. Luồng xử lý Controller Action chuẩn
Khi Controller nhận request POST:
```csharp
[AjaxOnly]
[HttpPost]
[ValidateInput(false)]
public ActionResult Add(RM_CustomerModel model, HttpPostedFileBase fileUpload)
{
    // 1. Kiểm tra nghiệp vụ & ghi nhận lỗi vào ModelState
    if (model.CustomerID <= 0)
    {
        ModelState.AddModelError("CustomerID", GetAppMessage("Msg_CustomerRequired", "Vui lòng chọn khách hàng!"));
    }

    // 2. Nếu ModelState có lỗi -> Trả về PartialView Form để render lỗi inline
    if (!ModelState.IsValid)
    {
        PrepareDropdowns(model);
        return PartialView("_Customer", model); // Trả về HTML PartialView chứa các lỗi đỏ
    }

    // 3. Thực thi lưu dữ liệu
    var result = _service.Save(model);
    if (result > 0)
    {
        return Json(new { status = true, id = result, message = CreateMessage("Thêm mới thành công!", EnumProcessType.Add, EnumMsgIcon.Success) });
    }

    return Json(new { status = false, message = CreateMessage("Lưu thất bại!", EnumProcessType.Add, EnumMsgIcon.Error) });
}
```

### 2.3. Luồng xử lý JavaScript Callback `_OnProcessSuccess`
```javascript
function Customer_OnProcessSuccess(response, formId) {
    if (response.status !== undefined) {
        // Response là JSON -> Thực thi thông báo Toastr
        eval(response.message);
        if (response.status === true) {
            $("#modal_" + formId).modal("hide");
            refreshDataTable();
        }
    } else {
        // Response là HTML PartialView (do ModelState không hợp lệ)
        var $body = $("#modal_" + formId + " #bodyForm");
        $body.html(response);
        // Tự động các thẻ @Html.ValidationMessageFor hiển thị dòng chữ đỏ!
        // Tái kích hoạt lại các plugin giao diện:
        _initFormPlugins($body);
    }
}
```

---

## 3. QUY CHUẨN FORM CONTROLS: BẮT BUỘC 100% @Html.* HELPERS

> **TUYỆT ĐỐI HẠN CHẾ SỬ DỤNG CÁC THẺ MẶC ĐỊNH HTML** (`<label>`, `<input>`, `<select>`, `<textarea>`).

| Thành phần | ✅ BẮT BUỘC DÙNG (`@Html.*` Helper) | Ghi chú & Lợi ích |
| :--- | :--- | :--- |
| **Nhãn trường (Label)** | `@Html.TitleFor(m => m.PropertyName, new { @class = "font-bold text-secondary-d2 mb-1 d-block text-90" })` | Tự động lấy tên hiển thị và dấu sao đỏ `(*)` bắt buộc từ `[Required]` |
| **Ô nhập văn bản** | `@Html.TextBoxFor(m => m.PropertyName, new { @class = "form-control form-control-sm", placeholder = "..." })` | 2-way binding, tự phục hồi giá trị khi submit lại |
| **Danh mục chọn đơn** | `@Html.DropDownListFor(m => m.FieldID, Model.ListItems, "-- Chọn --", new { @class = "form-control form-control-sm select2" })` | Chuẩn dropdown CenIT TOC |
| **Văn bản nhiều dòng** | `@Html.TextAreaFor(m => m.Note, new { @class = "form-control form-control-sm", rows = 4 })` | Chuẩn bị cho CKEditor hoặc textarea thường |
| **Ô ẩn (Hidden ID)** | `@Html.HiddenFor(m => m.FieldID, new { id = "FieldID_Form" })` | Lưu ID bảo mật |
| **Thông báo lỗi** | `@Html.ValidationMessageFor(m => m.PropertyName, "", new { @class = "text-danger text-85" })` | Hiển thị lỗi đỏ inline |
| **Bảo mật Form** | `@Html.AntiForgeryToken()` | Bắt buộc đặt ở đầu mỗi form |

---

## 4. QUY CHUẨN CHỐNG XUNG ĐỘT DOM ID (ANTI-DOM ID COLLISION)

1. **Nguyên tắc phân biệt ID:**
   - Khi form modal được nạp vào cùng trang với `_Search.cshtml`, cấm đặt trùng ID các control giữa Search card và Modal (như `CustomerID`, `DepartmentID`, `StatusID`).
   - Các ID trong form modal phải có hậu tố ngữ cảnh: `id="CustomerID_Form"` hoặc query scoped:
     ```javascript
     $form.find('input[name="CustomerID"]').val(c.id);
     ```
2. **Pre-submit Validation Client:**
   - Kiểm tra nhanh tại client trước khi post để hỗ trợ tức thời cho người dùng.

---

## 5. QUY CHUẨN CKEDITOR & AN TOÀN HTML REQUEST VALIDATION

1. **Model C# bắt buộc có `[AllowHtml]`:**
   ```csharp
   [AllowHtml]
   public string Note { get; set; }
   ```
2. **Cấu hình `requestValidationMode="2.0"` trong `Web.config`:**
   ```xml
   <httpRuntime targetFramework="4.8" requestValidationMode="2.0" maxRequestLength="1048576" />
   ```
3. **Đồng bộ nội dung trước khi Submit:**
   - Trong sự kiện submit form, duyệt qua các instances của CKEditor và gọi `updateElement()` để đồng bộ iframe vào textarea trước khi `ajaxForm` serialize.
