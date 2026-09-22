# 🪟 QUY CHUẨN MODAL, AJAX & THÔNG BÁO (MVC MODAL & AJAX RULES)

> **Tài liệu tham chiếu chuẩn:** Framework `_Form.cshtml`, `Enterprise.js`, `Accommodation.js`.  
> **Áp dụng cho:** Toàn bộ popup modal, request AJAX và hệ thống thông báo Toastr trong CenIT ReportTourism.

---

## 1. QUY CHUẨN KÍCH THƯỚC MODAL (DATA_WIDTH STANDARDS)

### 1.1. Ba kích thước chuẩn tuyệt đối
Toàn bộ Modal form trong hệ thống CenIT ReportTourism phải sử dụng 1 trong 3 kích thước chuẩn:
- **`data_width = "1024px"` (Modal Lớn - Chuẩn mặc định cho Form nghiệp vụ):**
  - Áp dụng cho: Form Doanh nghiệp, Lưu trú, Lữ hành, Báo cáo thống kê.
  - Bố cục: Thường chia 2 cột (`col-md-6`), chứa nhiều trường thông tin, file đính kèm, bảng con.
- **`data_width = "800px"` (Modal Trung bình):**
  - Áp dụng cho: Form cấu hình, danh mục đơn giản, phân quyền.
- **`data_width = "600px"` (Modal Nhỏ):**
  - Áp dụng cho: Xác nhận xóa, kích hoạt/hủy kích hoạt, đổi mật khẩu, thao tác đơn giản (1 - 3 trường).

### 1.2. Khai báo nút mở Modal bằng `@Html.Button` Helper
```razor
@Html.Button(true, "AddEnterprise", Url.Action("Add", new { area = "Cate" }),
             "<i class='fa fa-plus'></i>",
             AppProcessor.Messagor.GetMessage("Button_Add"),
             new { @class = "btn btn-sm btn-success", data_width = "1024px" })
```

---

## 2. QUY CHUẨN LAYOUT `_Form.cshtml` & `ajaxForm`

### 2.1. Cấu trúc Layout `_Form.cshtml`
Mọi Partial View Modal form (`_Add.cshtml`, `_Edit.cshtml`) đều phải kế thừa layout `_Form.cshtml`:
```razor
@{
    ViewBag.Title = "...";
    Layout = "~/Views/Shared/_Form.cshtml";
}
@section FormType { primary }      <!-- Màu sắc header: primary, info, success, warning, danger -->
@section FormIcon { <i class="..."></i> }
@section FormBody {
    <!-- Form HTML và nhúng Content PartialView -->
}
```

### 2.2. Xử lý Submit Form bằng `ajaxForm`
Đối với các form có upload file hoặc nội dung CKEditor, bắt buộc dùng `ajaxForm` (jquery.form plugin tích hợp sẵn trong Ace Admin):
```razor
@using (Html.BeginForm("Add", "Customer", new { area = "Cate" }, FormMethod.Post, new { encType = "multipart/form-data", id = "AddCustomer", @class = "form-horizontal" }))
{
    <div id="bodyForm">
        @Html.Partial("_Customer", Model)
    </div>
}

<script type="text/javascript">
    $('form#AddCustomer').ajaxForm({
        beforeSubmit: function() {
            // Đồng bộ CKEditor nếu có
            if (typeof CKEDITOR !== 'undefined') {
                for (var inst in CKEDITOR.instances) {
                    try { CKEDITOR.instances[inst].updateElement(); } catch(e) {}
                }
            }
            return true;
        },
        success: function (response) {
            Customer_OnProcessSuccess(response, "AddCustomer");
        }
    });
</script>
```

---

## 3. QUY CHUẨN CALLBACK `_OnProcessSuccess(response, formId)`

Mỗi module JavaScript bắt buộc phải có hàm callback nhận kết quả submit từ `ajaxForm`:

```javascript
function ModuleName_OnProcessSuccess(response, formId) {
    var $modal = $("#ModalContent #modal_" + formId);
    if ($modal.length === 0) {
        $modal = $("#modal_" + formId);
    }

    // 0. Luôn phục hồi trạng thái nút Lưu
    var $btnSave = $modal.find("#btnSave, .modal-footer #btnSave, button[type='submit']");
    $btnSave.prop("disabled", false).html('<i class="fa fa-save"></i> Lưu');

    // TRƯỜNG HỢP 1: Response là JSON (Thành công hoặc Lỗi hệ thống)
    if (response.status !== undefined) {
        // 1. Thực thi script Toastr từ server trả về hoặc hàm executeResponseMessage
        executeResponseMessage(response.message, response.status ? "Thao tác thành công!" : "Thao tác thất bại!", response.status);

        // 2. Nếu thành công: đóng modal và reload / điều hướng
        if (response.status === true) {
            var isKeepOpen = $modal.find("#chkNotDismissModal").is(":checked");
            if (isKeepOpen) {
                var urlAction = $modal.find("form").attr("action");
                $modal.find("#bodyForm").load(urlAction);
            } else {
                $modal.modal("hide");
                $(".modal-backdrop").remove();
                $("body").removeClass("modal-open").css("padding-right", "");
            }

            // Chuyển trang hoặc reload bảng độc lập, KHÔNG lệ thuộc duy nhất vào hidden.bs.modal
            if (response.id) {
                setTimeout(function () {
                    window.location.href = _detailUrl + "/" + response.id;
                }, 300);
            } else if (typeof refreshDataTable === "function") {
                refreshDataTable();
            }
        }
    } 
    // TRƯỜNG HỢP 2: Response là HTML PartialView (Validation thất bại)
    else {
        // Nạp lại HTML vào #bodyForm -> Các thẻ @Html.ValidationMessageFor sẽ tự động hiện chữ đỏ
        $modal.find("#bodyForm").html(response);
        
        // Tái khởi tạo các plugin giao diện sau khi HTML bị thay thế
        _initModalPlugins($modal);

        // BẮT BUỘC BẬT TOASTR CẢNH BÁO: Không bao giờ để form im lặng khi có lỗi
        var $firstError = $modal.find(".text-danger:visible").first();
        var warnMsg = ($firstError.length && $firstError.text().trim())
            ? $firstError.text().trim()
            : "Vui lòng kiểm tra và nhập đầy đủ các trường bắt buộc (*)!";
        executeResponseMessage(warnMsg, warnMsg, false);

        // Re-bind lại nút Submit để người dùng tiếp tục thao tác
        $modal.find("#btnSave, .modal-footer #btnSave").off("click").on("click", function (e) {
            e.preventDefault();
            $("form#" + formId).submit();
        });
    }
}
```

---

## 4. QUY CHUẨN THÔNG BÁO TOASTR, SUBMIT BUTTON & CHỐNG KẸT MODAL BACKDROP

### 4.1. Nguyên tắc An toàn khi đóng Modal và hiển thị Thông báo (Safe Modal Lifecycle)
- **CẤM:** Khóa chết logic hiển thị Toastr (`executeResponseMessage`) hoặc lệnh điều hướng (`window.location.href`) **duy nhất bên trong sự kiện `hidden.bs.modal`**. Nếu sự kiện này không kích hoạt (do modal nạp động hoặc animation bị kẹt), người dùng sẽ không nhận được thông báo và trang bị "đóng băng".
- **CHUẨN:** 
  1. Hiển thị thông báo Toastr NGAY LẬP TỨC khi nhận được phản hồi thành công từ server.
  2. Gọi đóng modal và dọn dẹp backdrop: `$modal.modal('hide'); $('.modal-backdrop').remove(); $('body').removeClass('modal-open');`.
  3. Điều hướng hoặc reload bảng với độ trễ ngắn (`setTimeout(..., 300)`).

### 4.2. Trạng thái Nút Submit Form (`#btnSave`)
- Mọi thao tác submit form từ Modal PHẢI có phản hồi xúc giác trực quan:
  - Khi bắt đầu submit (`beforeSubmit`): Chuyển nút `#btnSave` sang trạng thái `disabled` và icon quay: `<i class="fa fa-spinner fa-spin mr-1"></i> Đang lưu...`.
  - Khi hoàn tất hoặc lỗi: Phục hồi lại trạng thái nút.

### 4.3. Chống kẹt Backdrop khi mở Modal lồng nhau (Nested Modals)
Khi mở modal con (như Modal Tra cứu Khách hàng) từ trong modal cha (Thêm mới Cơ hội):
```javascript
$('#modalChildLookup').on('hidden.bs.modal', function () {
    if ($('#modalParent.show').length > 0) {
        $('body').addClass('modal-open');
    }
});
```
