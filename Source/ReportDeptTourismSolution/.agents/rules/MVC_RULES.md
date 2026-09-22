# 📘 BỘ QUY TẮC PHÁT TRIỂN ỨNG DỤNG MVC (CENIT REPORT TOURISM - MASTER INDEX)

> **Tài liệu tham chiếu chuẩn:** Phân hệ Doanh nghiệp (`Cate/Enterprise`) & Quản lý Lưu trú (`Cate/Accommodation`).  
> **Áp dụng cho:** Toàn bộ lập trình viên và AI Agent khi xây dựng View, Controller, JavaScript, Data Model trong hệ thống CenIT ReportTourism.  
> **Nguyên tắc cốt lõi:** Kế thừa chuẩn `Cate/Enterprise`, mô hình Form dùng chung (`_Entity.cshtml` + `_Form.cshtml`), xác thực kép (`@Html.ValidationMessageFor` inline cho lỗi trường + Toastr cho kết quả hệ thống), 100% `@Html.*` helper, tuân thủ giao diện AdminLTE 2.3.11 + Bootstrap 3 + skin-vnpt, 100% UTF-8 with BOM và đồng bộ các tầng module.

---

## 📑 DANH MỤC CÁC QUY CHUẨN CHUYÊN ĐỀ (SUB-RULES)

Để việc tra cứu và tuân thủ đạt hiệu quả cao nhất, bộ quy tắc MVC được tổ chức thành các chuyên đề chuyên sâu:

1. 📋 [**MVC_FORM_RULES.md**](file:///d:/SVN/Bao-cao-SCT/Source/ReportDeptTourismSolution/.agents/rules/MVC_FORM_RULES.md):
   - **Mô hình Form dùng chung (Shared Form Pattern):** Gom 100% markup vào `_Entity.cshtml`, tối giản `_Add.cshtml` và `_Edit.cshtml` (~25 dòng) kế thừa `_Form.cshtml`.
   - **Quy chuẩn Xác thực dữ liệu kép (Dual-Layer Validation):** Bắt buộc dùng `@Html.ValidationMessageFor` inline hiển thị chữ đỏ dưới ô nhập liệu khi `!ModelState.IsValid`. Cấm dùng Toastr báo lỗi thay thế cho lỗi trường nhập liệu.
   - **Quy chuẩn Form Controls:** Bắt buộc 100% `@Html.*` helper, nghiêm cấm thẻ HTML thuần (`<label>`, `<input>`, `<select>`).
   - **Chống xung đột DOM ID:** Context suffixing, scoped selectors.

2. 🪟 [**MVC_MODAL_AJAX_RULES.md**](file:///d:/SVN/Bao-cao-SCT/Source/ReportDeptTourismSolution/.agents/rules/MVC_MODAL_AJAX_RULES.md):
   - **Quy chuẩn kích thước Modal:** Chuẩn hóa 3 kích thước: `1024px` (Form nghiệp vụ lớn), `800px` (Trung bình), `600px` (Nhỏ).
   - **Layout `_Form.cshtml` & `ajaxForm`:** Kế thừa header/body/footer và submit form multipart có file đính kèm.
   - **Framework Callback:** Nhận JSON (Toastr + đóng modal) hoặc HTML (render lại form kèm lỗi validation inline).
   - **Vòng đời Toastr & Chống kẹt Backdrop:** Kích hoạt qua `hidden.bs.modal`.

3. 🧪 [**TESTING.md**](file:///d:/SVN/Bao-cao-SCT/Source/ReportDeptTourismSolution/.agents/rules/TESTING.md):
   - Kiểm thử Form Submit: xác minh trả về `PartialView` khi thiếu dữ liệu và `{ status: true }` khi thành công.
   - Kiểm thử Model Binding DisplayName an toàn (Anti-Null DisplayName).
   - Kiểm thử không có lỗi Console, mã phản hồi 200 OK.

4. 🔏 [**VNPT_SMARTCA_AND_REPORT_RULES.md**](file:///d:/SVN/Bao-cao-SCT/Source/ReportDeptTourismSolution/.agents/rules/VNPT_SMARTCA_AND_REPORT_RULES.md):
   - Quy chuẩn tích hợp ký số từ xa VNPT SmartCA và xuất bản báo cáo RDLC/Excel.

---

## 🏛️ TỔNG HỢP NGUYÊN TẮC CỐT LÕI BẮT BUỘC (CORE PILLARS)

### 1. Định dạng File: UTF-8 with BOM & Module Mirroring
- **100% file `.cshtml`, `.js`, `.cs`, `.sql`, `.xml`, `.md`, `.config`** BẮT BUỘC lưu dưới dạng **UTF-8 with BOM** (`0xEF, 0xBB, 0xBF`). Cấm tuyệt đối ANSI hoặc UTF-8 No BOM gây lỗi hiển thị tiếng Việt.
- **Module Mirroring:** Mọi chỉnh sửa ở tầng View/JS/CSS các phân hệ module (`CateModule`, `ReportModule`, `SysModule`) tự động đồng bộ sang `CenIT.ReportTourism.WebApp` qua PostBuildEvent robocopy khi biên dịch.

### 2. Mô hình Form dùng chung & Tái sử dụng giao diện
- **TUYỆT ĐỐI KHÔNG** tách rời nội dung form Thêm mới và Chỉnh sửa thành 2 file riêng biệt gây trùng lặp và phân mảnh mã nguồn.
- Tạo `_[Entity].cshtml` làm nội dung dùng chung duy nhất (Single Source of Truth).
- `_Add.cshtml` và `_Edit.cshtml` chỉ là các vỏ bọc form kết nối tới Layout `~/Views/Shared/_Form.cshtml`.

### 3. Quy chuẩn Xác thực kép (Dual-Layer Validation)
- Lỗi trường nhập liệu: Bắt buộc render inline qua `@Html.ValidationMessageFor` màu đỏ dưới chân ô nhập liệu.
- Controller: Nếu `!ModelState.IsValid`, trả về `PartialView("_[Entity]", model)` để giao diện tự hiển thị lỗi trực quan.
- Toastr Popup: Chỉ dùng cho thông báo kết quả chung sau khi thực thi nghiệp vụ xong.

### 4. Quy chuẩn Giao diện Danh sách & DataTable Ace Admin v4
- Chiều cao các control tìm kiếm (`_Search.cshtml`) chuẩn hóa đúng **32px** (`form-control-sm` hoặc class tương đương).
- **CẤM** nhúng Select2 vào thanh tìm kiếm ở trang danh sách nếu làm vỡ chiều cao 32px; ưu tiên dropdown chuẩn CenIT TOC.
- Cột thao tác trong DataTable phải sử dụng các icon và button helper chuẩn (`_renderButton`, `data_width = "1024px"`).

### 5. Quy chuẩn Cache-Busting cho Scripts
- Mọi script tag nhúng file `.js` tự viết phải có cache-busting timestamp:
  ```razor
  @section BottomScript {
      <script src="~/Areas/Cate/Views/DigitalSales/DigitalSales.js?v=@DateTime.Now.Ticks"></script>
  }
  ```
---

## 2. QUY CHUẨN FORM CONTROLS: @Html.* HELPERS vs THẺ HTML THUẦN

### 2.1. Nguyên tắc Vàng
> **TUYỆT ĐỐI HẠN CHẾ SỬ DỤNG CÁC THẺ MẶC ĐỊNH HTML** (`<label>`, `<input>`, `<select>`, `<textarea>`) cho các Form Controls.  
> **CHỈ ĐƯỢC PHÉP** dùng thẻ HTML thuần khi cả source code dự án **HOÀN TOÀN KHÔNG CÓ** `@Html.*` helper tương ứng phù hợp.

### 2.2. Bảng đối chiếu quy chuẩn (DOs and DON'Ts)

| Thành phần giao diện | ❌ BỊ CẤM (Thẻ HTML thuần) | ✅ BẮT BUỘC DÙNG (`@Html.*` Helper) | Ghi chú & Cú pháp mẫu CenIT TOC |
| :--- | :--- | :--- | :--- |
| **Nhãn trường (Label)** | `<label>Tên cơ hội (*)</label>` | `@Html.TitleFor(m => m.OpportunityName, new { @class = "col-sm-2 col-form-label text-sm-right pr-0 font-bold" })` | Tự động lấy tên hiển thị và dấu sao đỏ `(*)` bắt buộc từ thuộc tính `[Required]` trong Model |
| **Ô nhập văn bản** | `<input type="text" name="..." />` | `@Html.TextBoxFor(m => m.OpportunityName, new { @class = "form-control", placeholder = "..." })` | Tự động binding dữ liệu 2 chiều, giữ lại giá trị khi validation lỗi |
| **Ô nhập số / tỷ lệ %** | `<input type="number" ... />` | `@Html.TextBoxFor(m => m.ClosingProbability, new { @class = "form-control", type = "number", min = "0", max = "100", step = "1", placeholder = "0 – 100", data_format = "percent" })` | Hỗ trợ thuộc tính HTML5 thông qua anonymous object |
| **Ô nhập ngày giờ** | `<input type="datetime" ... />` | `@Html.TextBoxFor(m => m.ExchangeDate, "{0:dd/MM/yyyy HH:mm}", new { id = "ExchangeDate", @class = "form-control datetimepicker", placeholder = "dd/mm/yyyy hh:mm" })` | Định dạng ngày giờ chuẩn kèm class `datetimepicker` |
| **Ô ẩn (Hidden ID)** | `<input type="hidden" name="ID" />` | `@Html.HiddenFor(m => m.BusinessOpportunityID)` | Giữ ID khóa chính, ID khóa ngoại, token an toàn |
| **Danh mục chọn đơn** | `<select name="..."><option>...</select>` | `@Html.DropDownListFor(m => m.ContactPerson_ID, new SelectList(Model.ListContactPerson, "Value", "Text"), "-- Chọn người liên hệ --", new { @class = "form-control" })` | Dropdown chọn 1 giá trị chuẩn CenIT TOC |
| **Danh mục chọn nhiều (Multiple)** | `<select multiple ...>` | `@Html.DropDownListFor(m => m.lst_SP, new SelectList(Model.SPDichVus, "pID", "DisplayName"), "", new { @class = "form-control", multiple = true })` | Tích hợp Select2 / đa chọn |
| **Văn bản nhiều dòng** | `<textarea name="...">...</textarea>` | `@Html.TextAreaFor(m => m.Description, new { @class = "form-control", rows = 3, placeholder = "..." })` | Nhập mô tả, ghi chú |
| **Đính kèm tệp tin** | `<input type="file" name="..." />` | `@Html.FileFor(m => m.DinhKemFile, ".*", true, new { @class = "form-control", placeholder = "Đính kèm tệp tin" })` | Helper chuyên dụng tải file, cấu hình đuôi mở rộng và đa file |
| **Hộp kiểm (Checkbox)** | `<input type="checkbox" name="..." />` | `@Html.CheckBoxFor(m => m.IsActive, new { @class = "..." })` | Trạng thái kích hoạt, cờ boolean |
| **Thông báo lỗi xác thực** | `<span class="text-danger">Lỗi...</span>` | `@Html.ValidationMessageFor(m => m.OpportunityName, "", new { @class = "text-danger" })` | Bắt buộc đặt ngay dưới control nhập liệu |
| **Bảo mật Form** | `<input name="__RequestVerificationToken"...>` | `@Html.AntiForgeryToken()` | Bắt buộc đặt ở đầu mỗi Form |
| **Nút mở Modal / Thao tác** | `<button onclick="...">Thêm</button>` | `@Html.Button(true, "AddBusinessOpportunity", Url.Action("Add", new { area = "Cate" }), "<i class='fa fa-plus'></i>", AppProcessor.Messagor.GetMessage("Button_Add"), new { @class = "btn btn-light-blue ...", data_width = "1024px" })` | Helper tạo nút phân quyền và cấu hình kích thước Modal |

---

## 3. QUY CHUẨN MASTER LAYOUT, PAGE TITLE & BREADCRUMB

### 3.1. Cơ chế hoạt động của Layout CenIT TOC
File Layout dùng chung `Views/Shared/_PageContent.cshtml` đã dựng sẵn khung Page Header như sau:
```html
<div class="page-header pb-0" id="PageTitle">
    <div class="page-title text-primary-d2" style="font-size:1rem !important">
        @RenderSection("PageTitle", false)
    </div>
    <div class="page-tools d-inline-flex" id="PageAction">
        @RenderSection("PageAction", false)
    </div>
</div>
```
Khi trang hoàn tất tải tài nguyên (DOM ready), script hệ thống `BE-ConfigBreadcrumb.js` tự động phân tích URL và đè toàn bộ nội dung của thẻ `div.page-title` thành thanh điều hướng Breadcrumb:
```javascript
$('div.page-title').html('<i class="fa fa-home blue"></i> <a href="/">Trang chủ</a> &raquo; ...');
```

### 3.2. Quy tắc Bắt buộc thi hành
1. **`@section PageTitle` CHỈ ĐƯỢC CHỨA `@ViewBag.Title` hoặc chuỗi văn bản đơn giản**:
   ```razor
   @section PageTitle {
       @ViewBag.Title
   }
   ```
2. **NGHIÊM CẤM TUYỆT ĐỐI**:
   - Không lồng `<div class="page-header">`, `<h1>`, `.page-title` hoặc cấu trúc HTML phức tạp vào `@section PageTitle`.
   - **Hậu quả nếu vi phạm:** Sẽ sinh ra mã HTML lồng nhau bất hợp lệ, phá vỡ flexbox layout. Khi `BE-ConfigBreadcrumb.js` chạy, nó sẽ xóa sạch toàn bộ nội dung trong `div.page-title`, gây ra hiện tượng nhấp nháy màn hình (FOUC) và mất sạch tiêu đề.
3. **Vị trí cho Badge thông tin & Nút hành động đầu trang**:
   - Nếu màn hình cần nhãn phân hệ, thông tin phiên bản hoặc các nút hành động, **BẮT BUỘC** đặt vào `@section PageAction`:
   ```razor
   @section PageAction {
       <span class="badge badge-lg bgc-primary-l3 text-primary-d2 border-1 brc-primary-m3">
           <i class="fa fa-sitemap mr-1"></i> Phân hệ Kinh doanh SPDV Số
       </span>
       @Html.Button(true, "AddOpportunity", ...)
   }
   ```

---

## 4. QUY CHUẨN BỘ LỌC TÌM KIẾM (SEARCH BOX & FILTER FORM)

### 4.1. Thẻ Card bọc Bộ lọc Tìm kiếm
Bắt buộc dùng cú pháp chuẩn Ace Admin với class `.Search.card.bcard.border-0.shadow-sm.radius-0`:
```razor
<div class="Search card bcard border-0 shadow-sm radius-0" id="Search[Entity]">
    <div class="card-body p-3">
        <!-- Lưới form tìm kiếm -->
    </div>
</div>
```

### 4.2. Chuẩn hóa Chiều cao Control (32px) & CẤM TUYỆT ĐỐI SELECT2 trong Search Card
- Toàn bộ Form Controls trong Card tìm kiếm phải có chiều cao đồng nhất **32px** (chuẩn của `.form-control` trong Ace Admin).
- **CẤM TUYỆT ĐỐI dùng Select2 cho các combobox trong Card Tìm kiếm**:
  - *Nguyên nhân:* Select2 sinh ra thẻ `span.select2-container` động với chiều cao mặc định khác biệt (28px hoặc 38px), khiến giao diện bị lỗi "input to, input nhỏ", lệch hàng nghiêm trọng so với TextBox và DatePicker.
  - *Quy tắc:* Trong `_Search.cshtml`, toàn bộ Dropdown bắt buộc dùng `@Html.DropDownListFor` hoặc thẻ `<select class="form-control">` thuần, tuyệt đối không gọi `$('select').select2()`.

### 4.3. Bố cục Nhóm Nút Hành động Tìm kiếm
- Nhóm nút Tìm kiếm và Làm mới (Bỏ lọc) phải đặt ở **hàng cuối cùng, góc dưới bên trái** (hoặc `text-left`) của Search Card.
- **Nút Tìm kiếm:** `.btn.btn-primary` kèm icon `<i class="fa fa-search mr-1"></i> Tìm kiếm`.
- **Nút Làm mới (Bỏ lọc):** `.btn.btn-secondary` hoặc `.btn-default` kèm icon `<i class="fa fa-undo mr-1"></i> Làm mới`.
- Đảm bảo khoảng cách (`mr-2`, `px-3`), không để nút dính chùm hoặc lệch sang phải mất cân đối.

### 4.4. Cấu hình Chọn Ngày tháng (DatePicker)
- Trường chọn ngày tháng trong Search Card phải có class `datepicker` hoặc `datetimepicker` và placeholder định dạng `dd/MM/yyyy`.
- Ví dụ chuẩn tham chiếu từ `Cate/Project`:
```razor
<div class="input-group">
    <div class="input-group-prepend">
        <span class="input-group-text"><i class="fa fa-calendar-alt"></i></span>
    </div>
    @Html.TextBoxFor(m => m.FromDate, "{0:dd/MM/yyyy}", new { @class = "form-control datepicker", placeholder = "Từ ngày (dd/mm/yyyy)" })
</div>
```

---

## 5. QUY CHUẨN DESIGN TOKENS ACE ADMIN V4 (CẤM TỰ VIẾT CSS RIÊNG)

Hệ thống CenIT TOC CRM xây dựng trên nền tảng **Ace Admin v4 (Bootstrap 4)**. Hệ thống đã cung cấp đầy đủ hệ thống Design Tokens thông qua class utilities.

### 5.1. Những điều CẤM TUYỆT ĐỐI (DON'Ts)
- ❌ **KHÔNG** tự viết các class CSS custom cho card (`.workflow-card`, `.my-card`, `.box-item`,...).
- ❌ **KHÔNG** tự ý hardcode mã màu hex trong mã HTML/CSS (`#0288d1`, `#f59e0b`, `#ef4444`, `#8b5cf6`, `#f7f9fb`, `#e2e8f0`...).
- ❌ **KHÔNG** tự ý viết inline styles đè màu viền: `style="border-left-color: ... !important"`.
- ❌ **KHÔNG** dùng nút bo tròn viên thuốc (`border-radius: 20px` hoặc `btn-pill`) lệch chuẩn.
- ❌ **KHÔNG** dùng Bootstrap `.btn-group-toggle` để làm tab điều hướng.

### 5.2. Chuẩn quy chiếu Design Tokens Ace Admin (DOs)
- **Thẻ Card:**
  - Card thường: `.card.bcard.border-1.brc-[color]-m3.shadow-sm`
  - Card dữ liệu / bảng: `.card.dcard`
- **Card Header & Title:**
  - Header: `.card-header.bgc-[color]-d1.text-white`
  - Tiêu đề: `<h5 class="card-title text-100 text-white font-bolder mb-0"><i class="fa ... mr-1"></i> Tiêu đề</h5>`
  - Toolbar nút bấm: `<div class="card-toolbar"><button class="btn btn-xs btn-white btn-h-light-[color] ... font-bolder border-0">`
- **Tab Điều hướng (Nav Tabs):**
  - Dùng chuẩn Ace Nav Tabs:
    ```html
    <ul class="nav nav-tabs nav-tabs-simple border-0" role="tablist">
        <li class="nav-item">
            <a class="nav-link active font-bold py-2 text-90 text-blue-d2" data-toggle="tab" href="#tab1">
                <i class="fa fa-lightbulb text-orange-d1 mr-1"></i> Cơ hội
            </a>
        </li>
        <li class="nav-item">
            <a class="nav-link font-bold py-2 text-90 text-blue-d2" data-toggle="tab" href="#tab2">
                <i class="fa fa-project-diagram text-purple-d1 mr-1"></i> Dự án
            </a>
        </li>
    </ul>
    ```
- **Danh sách List Group & Trạng thái Active:**
  - Item chuẩn: `.list-group-item.list-group-item-action.px-3.py-2.cursor-pointer.border-1.brc-grey-l2.border-l-4.mb-2.radius-1.bgc-h-blue-l4`
  - Highlight mục đang chọn (Active): `.active.bgc-[color]-l3.text-[color]-d2.shadow-sm.font-bold`
- **Huy hiệu (Badges):**
  - Badge trạng thái: `.badge.bgc-[color]-l2.text-[color]-d2.border-1.brc-[color]-m3`
  - Badge số lượng: `.badge.badge-sm.badge-pill.text-white.bgc-[color]-d1`

---

## 6. QUY CHUẨN MÀN HÌNH NHIỀU CỘT (WORKFLOW / MASTER-DETAIL)

Đối với các màn hình quản trị quy trình, cây danh mục, phân cấp dữ liệu (ví dụ: `DigitalSalesWorkflow`):

1. **Bố cục Lưới chuẩn 3 cột:**
   - Sử dụng `.col-xl-4.col-lg-4.col-md-12.mb-3` cho mỗi cột.
2. **Phân cấp Màu sắc Ngữ nghĩa (Semantic Color Hierarchy):**
   - **Cột 1 (Cấp gốc / Trạng thái):** Card màu chính `bgc-primary-d1` (Xanh CenIT).
   - **Cột 2 (Cấp trung gian / Quy trình):** Card màu bổ trợ `bgc-blue-d1` (Xanh dương).
   - **Cột 3 (Cấp chi tiết / Tiến trình):** Card màu hoàn tất `bgc-success-d1` (Xanh lá).
3. **Khung Cuộn Danh sách (Scroll Container):**
   - Thiết lập chiều cao cuộn mượt mà: `overflow-y: auto; max-height: calc(100vh - 250px); min-height: 420px;`.
4. **Trạng thái Trống (Empty State) & Khóa Nút Thao tác:**
   - Cột phụ thuộc khi chưa có mục cha được chọn phải hiển thị thông báo hướng dẫn kèm icon mờ `opacity-50`:
     `<div class="text-center py-5 text-secondary-m2"><i class="fa fa-arrow-left text-160 mb-2 opacity-50"></i><p class="mb-0 text-90">Vui lòng chọn mục ở cột trước</p></div>`
   - Nút Thêm mới ở cột phụ thuộc **BẮT BUỘC** bị khóa (`class="disabled" disabled`) cho đến khi một mục cha hợp lệ được kích hoạt.
5. **Nút Thao tác trên từng dòng (Action Buttons):**
   - Nút Sửa: `.btn.btn-xs.btn-outline-primary.btn-h-primary.btn-a-primary.border-0.mr-1.p-1` kèm icon `<i class="fa fa-pencil-alt text-90"></i>`.
   - Nút Xóa: `.btn.btn-xs.btn-outline-danger.btn-h-danger.btn-a-danger.border-0.p-1` kèm icon `<i class="fa fa-trash-alt text-90"></i>`.

---

## 7. QUY CHUẨN CẤU TRÚC VIEW VÀ KÍCH THƯỚC MODAL

### 7.1. Kích thước Modal chuẩn (`data_width`)
Khi cấu hình nút mở modal tại View chính (`Index.cshtml`), thuộc tính `data_width` quyết định độ rộng chuẩn của Modal Bootstrap:

1. **`data_width = "1024px"` (Form lớn / Phức tạp / 2 cột)**:
   - **Áp dụng cho:** Màn hình Thêm mới/Chỉnh sửa có nhiều trường dữ liệu, bố cục chia 2 cột, có đính kèm file, bảng con, hoặc phân tab (Ví dụ: `RM_BusinessOpportunity`, `Project`, `Contract`, `DigitalSales`).
2. **`data_width = "800px"` hoặc `"700px"` (Form trung bình / 1-2 cột)**:
   - **Áp dụng cho:** Màn hình danh mục vừa phải, 4 – 10 trường nhập liệu, modal cấu hình quy trình.
3. **`data_width = "600px"` hoặc `"500px"` (Form nhỏ / Xác nhận / Đổi trạng thái)**:
   - **Áp dụng cho:** Modal xác nhận Xóa (`_Delete.cshtml`), cập nhật nhanh trạng thái.
4. **`modal-fs` (Full screen modal)**:
   - **Áp dụng cho:** Modal lịch sử cập nhật trạng thái có phân đôi giao diện (Form xác nhận 1 bên, timeline lịch sử bên kia như `_UpdateStatus.cshtml`).

---

### 7.2. Cấu trúc Modal Thêm mới / Chỉnh sửa (`_Add.cshtml`, `_Edit.cshtml`)

```cshtml
@using TSFramework.Libs.Processors
@{
    ViewBag.Title = string.Format(AppProcessor.Messagor.GetMessage("Modal_Title_Add"), "Cơ hội kinh doanh");
    Layout = "~/Views/Shared/_Form.cshtml"; // BẮT BUỘC kế thừa Layout _Form.cshtml
}

@section FormType {
    success  @* 'success' cho Thêm mới, 'primary' cho Chỉnh sửa *@
}

@section FormIcon {
    <i class="fas fa-plus"></i>
}

@section FormBody {
    @using (Html.BeginForm("Add", "RM_BusinessOpportunity", new { area = "Cate" }, FormMethod.Post, 
        new { encType = "multipart/form-data", id = "AddBusinessOpportunity", @class = "form-horizontal" }))
    {
        <div id="bodyForm">
            @* BẮT BUỘC: Bọc Partial View ruột trong <div id="bodyForm"> để thay thế khi validation lỗi *@
            @Html.Partial("_AddCHKD_View")
        </div>
    }

    <script type="text/javascript">
        $('form#AddBusinessOpportunity').ajaxForm({
            success: function (response) {
                BusinessOpportunity_OnProcessSuccess(response, "AddBusinessOpportunity");
            }
        });
    </script>
}
```

---

## 8. QUY CHUẨN JAVASCRIPT: XỬ LÝ PHẢN HỒI & VÒNG ĐỜI MODAL

### 8.1. Quy tắc Vàng chống Kẹt Backdrop & Đồng bộ Toastr
> **TUYỆT ĐỐI KHÔNG KÍCH HOẠT TOASTR HOẶC RELOAD DỮ LIỆU KHI MODAL ĐANG TẮT DỞ DANG.**  
> Khi đóng modal, **BẮT BUỘC** đợi sự kiện `hidden.bs.modal` kích hoạt xong mới gọi `eval(response.message)` và reload dữ liệu.  
> Nếu kích hoạt ngay, lớp mờ đen (`.modal-backdrop`) sẽ bị mồ côi (orphaned) và khóa cứng màn hình người dùng!

### 8.2. Hàm Callback Chuẩn `[Entity]_OnProcessSuccess`
```javascript
function BusinessOpportunity_OnProcessSuccess(response, formId) {
    if (response && response.projectId) {
        window.open("/Cate/ProjectOverview/Index/" + response.projectId, "_blank");
    }

    // 1. TRƯỜNG HỢP SERVER TRẢ VỀ JSON:
    if (response.status != undefined) {
        // KỊCH BẢN A: Người dùng chọn "Tiếp tục tạo mới" (#chkNotDismissModal)
        if ($("#ModalContent #modal_" + formId + " #chkNotDismissModal").is(":checked")) {
            eval(response.message);
            if (typeof _tableBusinessOpportunity !== "undefined" && _tableBusinessOpportunity) {
                _tableBusinessOpportunity.ajax.reload(null, false);
            }
            response.status = undefined;
            
            var urlAction = $("#ModalContent #modal_" + formId + " form").attr("action");
            $("#ModalContent #modal_" + formId + " #modal-content").load(urlAction, function () {
                if (typeof _initElement === "function") _initElement();
            });
        } 
        // KỊCH BẢN B: Đóng modal chuẩn mực
        else {
            if (!response.status && response.errorCode == 1) {
                eval(response.message); // Báo lỗi ngay và GIỮ NGUYÊN modal
                response.status = undefined;
            } else {
                $("#ModalContent #modal_" + formId).modal("hide");
                
                // ĐỢI MODAL ẨN XONG HOÀN TOÀN MỚI KÍCH HOẠT THÔNG BÁO VÀ RELOAD
                $("#ModalContent #modal_" + formId).one("hidden.bs.modal", function () {
                    if (response.status != undefined) {
                        eval(response.message);
                        if (typeof _tableBusinessOpportunity !== "undefined" && _tableBusinessOpportunity) {
                            _tableBusinessOpportunity.ajax.reload(null, false);
                        }
                        response.status = undefined;
                    }
                });
            }
        }
    } 
    // 2. TRƯỜNG HỢP SERVER TRẢ VỀ HTML (Validation thất bại):
    // Thay thế toàn bộ HTML vào #bodyForm để hiển thị lỗi đỏ ValidationMessageFor
    else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}
```

### 8.2.1. Bắt buộc khởi tạo lại validation sau khi thay PartialView ruột

Khi controller trả HTML do `ModelState` không hợp lệ, callback phải thay đúng nội dung `#bodyForm`, giữ modal mở và khởi tạo lại các control động cùng unobtrusive validation:

```javascript
function renderValidationResponse(response) {
    if (typeof response !== "string") return false; // JSON nghiệp vụ

    var $modal = $(".modal.show").last();
    var $bodyForm = $modal.find("#bodyForm");
    if (!$bodyForm.length) return false;

    $bodyForm.html(response);
    if (typeof _initElement === "function") _initElement();

    var $form = $bodyForm.closest("form");
    if ($.validator && $.validator.unobtrusive && $form.length) {
        $form.removeData("validator").removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse($form);
    }
    return true;
}
```

- Callback **BẮT BUỘC** kiểm tra và xử lý response HTML trước khi đọc `response.status`.
- Response HTML validation **KHÔNG ĐƯỢC** đóng modal, reload bảng hoặc gọi `eval`.
- Response JSON có `status` chỉ dùng cho kết quả xử lý nghiệp vụ sau khi `ModelState` đã hợp lệ.

### 8.3. Cấm tuyệt đối hàm `alert()` của JavaScript
- Thay vì gọi `alert("...")`, bắt buộc dùng thư viện Toastr: `toastr.warning("...")`, `toastr.error("...")` hoặc `toastr.success("...")`.

---

## 9. QUY CHUẨN CONTROLLER (APPCONTROLLER & ACTIONS)

### 9.1. Kế thừa & Attributes
- Controller **BẮT BUỘC** kế thừa từ `AppController` (`Modules.Cate` / `Core.Sys.BaseApp`).
- Các Action tương tác Ajax / Modal **BẮT BUỘC** trang trí các Attribute:
  - `[AjaxOnly]`: Chỉ cho phép gọi qua Ajax.
  - `[ActionType(Type = EnumActionType.Create / Edit / Delete / View)]`: Kiểm soát phân quyền chức năng.
  - `[HttpPost]` / `[HttpGet]`.
  - `[ValidateAntiForgeryToken]` hoặc `[ValidateInput(false)]` (khi có CKEditor/HTML input).

### 9.2. Action Thêm mới (`Add` POST)
```csharp
[AjaxOnly]
[HttpPost]
[ValidateAntiForgeryToken]
[ActionType(Type = EnumActionType.Create)]
public ActionResult Add(RM_BusinessOpportunityModel model)
{
    if (!ModelState.IsValid)
    {
        model.ListContactPerson = GetContactPersons(model.CustomerID);
        // BẮT BUỘC: Trả về PartialView ruột để Javascript thế vào #bodyForm
        return PartialView("_AddCHKD_View", model);
    }

    var result = _businessOpportunityCache.Save(model, User.UserName);

    string response;
    if (result == 0)
        response = CreateMessage($"Cơ hội kinh doanh [{model.OpportunityName}]", EnumProcessType.Add, EnumMsgIcon.Error);
    else if (result == -9)
        response = CreateMessage($"Cơ hội kinh doanh [{model.OpportunityName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
    else
        response = CreateMessage($"Cơ hội kinh doanh [{model.OpportunityName}]", EnumProcessType.Add, EnumMsgIcon.Success);

    return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
}
```

### 9.3. Quy tắc bắt buộc khi `ModelState` không hợp lệ

1. Modal Add/Edit/Save **BẮT BUỘC** tách thành hai lớp:
   - Partial wrapper: chứa `Layout = "~/Views/Shared/_Form.cshtml"`, `Ajax.BeginForm` và `<div id="bodyForm">@Html.Partial("_EntityForm", Model)</div>`.
   - Partial ruột `_EntityForm`: chứa anti-forgery token, hidden field, control nhập liệu và `ValidationMessageFor`.
2. Partial ruột **BẮT BUỘC** dùng chung cho cả thêm mới và cập nhật; không nhân đôi markup field giữa hai modal.
3. POST action **BẮT BUỘC** kiểm tra `ModelState.IsValid` trước khi gọi Cache/Biz/Stored Procedure.
4. Khi `ModelState` không hợp lệ, controller phải phục hồi toàn bộ dữ liệu phụ cần render (dropdown, tên bản ghi cha, danh sách lựa chọn...) và trả `PartialView("_EntityForm", model)`.
5. **CẤM** trả JSON “lưu thất bại” cho lỗi validation field, vì người dùng sẽ không biết trường nào sai và dữ liệu nhập có thể bị mất.
6. POST action dùng anti-forgery token phải có cả `@Html.AntiForgeryToken()` trong partial ruột và `[ValidateAntiForgeryToken]` tại controller.
7. Client validation chỉ hỗ trợ trải nghiệm; validation tại controller qua `ModelState` luôn là lớp xác thực cuối cùng và không được bỏ qua.

---

## 10. QUY CHUẨN RÀNG BUỘC APP_MESSAGE CHO VIEW VÀ CONTROLLER (STRICT LOCALIZATION & MESSAGING)

### 10.1. Nguyên tắc cốt lõi
> **TUYỆT ĐỐI NGHIÊM CẤM** hardcode chuỗi ký tự văn bản thuần (plain text), nhãn tiêu đề (labels), chú thích (placeholders/tooltips) trực tiếp trên View (`.cshtml`) và các chuỗi thông báo phản hồi (messages, alert, validation, error/success response) bên trong Controller (`.cs`).  
> **100% CÁC THÔNG TIN TEXT VÀ MESSAGE BẮT BUỘC PHẢI KHAI BÁO TRONG HỆ THỐNG `App_Message`** (bảng cơ sở dữ liệu `Sys_Messages` với `LangCode = 'vi-VN'`).

### 10.2. Quy tắc khai báo và sử dụng trên View (.cshtml)
1. **Tiêu đề trang & Header:**
   - Dùng `@AppProcessor.Messagor.GetMessage("LabelKey")`:
     ```razor
     @{
         ViewBag.Title = AppProcessor.Messagor.GetMessage("DigitalSales_Title");
     }
     @section PageTitle {
         @ViewBag.Title
     }
     ```
2. **Nút bấm thao tác & Tiêu đề Form:**
   - Sử dụng các key chuẩn cho nút:
     ```razor
     @Html.Button(true, "Add", Url.Action("Add", ...), "<i class='fa fa-plus'></i>", AppProcessor.Messagor.GetMessage("Button_Add"), new { ... })
     ```
3. **Nhãn trường nhập liệu (Labels):**
   - Bắt buộc dùng `@Html.TitleFor(m => m.FieldName)` để tự động đọc nhãn từ thuộc tính `[CustomDisplayName("LabelKey")]` trong Model.
   - Nếu bắt buộc viết nhãn tùy chỉnh ngoài Form Control, phải dùng `@AppProcessor.Messagor.GetMessage("LabelKey")`.
4. **Chú thích, Placeholder, Cột bảng dữ liệu DataTable:**
   - Tiêu đề cột `<th>`: `@AppProcessor.Messagor.GetMessage("[Module]_[Field]_Header")` hoặc khai báo danh mục nhãn tương ứng.

### 10.2.1. Metadata Model là nguồn duy nhất cho tiêu đề trường Form
1. Mọi property được hiển thị trên Form **BẮT BUỘC** khai báo `CustomDisplayName` bằng `LabelKey` tồn tại trong bảng `Sys_Messages`; **NGHIÊM CẤM** truyền trực tiếp chuỗi tiếng Việt hoặc chuỗi hiển thị vào attribute:
   ```csharp
   // Đúng
   [CustomRequired]
   [CustomDisplayName("CustomerType_Label_Code")]
   public string CustomerTypeCode { get; set; }

   // Sai: hard-code nội dung hiển thị trong Model
   [CustomDisplayName("Mã loại khách hàng")]
   public string CustomerTypeCode { get; set; }
   ```
2. Label của control có model binding **BẮT BUỘC** dùng `@Html.TitleFor`; không viết lại cùng nội dung bằng `<label>` hoặc `GetMessage` trực tiếp trên View:
   ```razor
   @Html.TitleFor(model => model.CustomerTypeCode,
       new { @class = "col-sm-3 col-form-label text-sm-right pr-0 font-bold" })
   ```
3. `TitleFor` lấy nội dung từ `[CustomDisplayName("LabelKey")]` và tự hiển thị dấu bắt buộc theo `[CustomRequired]`. Không tự nối `(*)` hoặc `<span class="text-danger">*</span>` trong label.
4. Placeholder, `data-placeholder` và option hướng dẫn không phải label nên **BẮT BUỘC** lấy trực tiếp từ `AppProcessor.Messagor.GetMessage("LabelKey")`:
   ```razor
   @Html.TextBoxFor(model => model.CustomerTypeCode, new
   {
       @class = "form-control",
       placeholder = AppProcessor.Messagor.GetMessage("CustomerType_Code_Placeholder")
   })
   ```
5. Khi thêm hoặc đổi `LabelKey`, **BẮT BUỘC** kèm script SQL idempotent cập nhật `Sys_Messages` cho `LangCode = 'vi-VN'`; không được hoàn thành thay đổi nếu code tham chiếu key chưa tồn tại trong DB mục tiêu.

### 10.3. Quy tắc khai báo và sử dụng trong Controller (.cs)
1. **Định nghĩa tiêu đề phân hệ (`_title`):**
   ```csharp
   private readonly string _title = AppProcessor.Messagor.GetMessage("DigitalSales_Title");
   ```
2. **Hàm helper đọc Message an toàn (bắt buộc có):**
   ```csharp
   private string GetAppMessage(string labelKey, string defaultMessage = "")
   {
       var msg = AppProcessor.Messagor.GetMessage(labelKey);
       return !string.IsNullOrEmpty(msg) ? msg : defaultMessage;
   }
   ```
3. **Thông báo trả về cho Client (JSON / Toastr / AJAX Response):**
   - **Thành công:**
     ```csharp
     return Json(new
     {
         status = true,
         code = 1,
         message = GetAppMessage("DigitalSales_Msg_ChangeStatusSuccess", "Chuyển trạng thái thành công!")
     });
     ```
   - **Ràng buộc nghiệp vụ / Cảnh báo:**
     ```csharp
     return Json(new
     {
         status = false,
         code = -3,
         message = GetAppMessage("DigitalSales_Msg_ReqProductBeforeProject", "RÀNG BUỘC CHUYỂN DỰ ÁN: Chưa có Sản phẩm / Dịch vụ số đính kèm!")
     });
     ```
   - **Kiểm tra phân quyền:**
     ```csharp
     return Json(new
     {
         status = false,
         message = GetAppMessage("DigitalSales_Msg_NoPermission", "Bạn không có quyền thao tác trên hồ sơ này!")
     });
     ```
   - **Thông báo CRUD chuẩn:** Sử dụng `CreateMessage(_title, EnumProcessType.[Add/Edit/Delete], EnumMsgIcon.[Success/Error])`.

### 10.4. Quy chuẩn đặt tên LabelKey trong Sys_Messages
| Loại thông điệp | Quy tắc đặt LabelKey | Ví dụ |
| :--- | :--- | :--- |
| **Tiêu đề phân hệ / chức năng** | `[Module]_[Feature]_Title` | `DigitalSales_Title`, `Project_Title` |
| **Nhãn trường thông tin** | `[Module]_[Field]_Label` | `DigitalSales_Customer_Label`, `User_Label_FullName` |
| **Tiêu đề cột bảng** | `[Module]_[Column]_Header` | `DigitalSales_Col_Status`, `DigitalSales_Col_Revenue` |
| **Nút bấm thao tác** | `Button_[Action]` | `Button_Save`, `Button_Cancel`, `Button_Add`, `Button_Delete` |
| **Thông báo nghiệp vụ thành công** | `[Module]_Msg_[Action]Success` | `DigitalSales_Msg_ChangeStatusSuccess`, `DigitalSales_Msg_SaveProductSuccess` |
| **Thông báo nghiệp vụ thất bại / Lỗi** | `[Module]_Msg_[Action]Fail` | `DigitalSales_Msg_ChangeStatusFail`, `DigitalSales_Msg_SaveMemberFail` |
| **Ràng buộc điều kiện (Gatekeeper)** | `[Module]_Msg_Req[Condition]` | `DigitalSales_Msg_ReqProductBeforeProject`, `DigitalSales_Msg_ReqMemberBeforeProject` |
| **Xác thực / Bắt buộc nhập** | `[Module]_Msg_[Field]Required` | `DigitalSales_Msg_CustomerRequired`, `DigitalSales_Msg_TitleRequired` |
| **Phân quyền truy cập** | `[Module]_Msg_NoPermission` | `DigitalSales_Msg_NoPermission` |

---

## 11. QUY TẮC ĐỒNG BỘ 3 NƠI (TRIPLE MIRRORING) & BIÊN DỊCH BẮT BUỘC

Mỗi khi tạo mới hoặc chỉnh sửa file:
1. **Source phát triển:** `CenIT.ReportTourism.Modules.CateModule\...` (hoặc `ReportModule`, `SysModule`)
2. **Gói triển khai:** `publish_source\...`
3. **Website WebApp:** `CenIT.ReportTourism.WebApp\...`

**Quy trình chuẩn thi hành:**
1. Sau khi sửa View (`.cshtml`) hoặc Script (`.js`), PostBuildEvent sẽ tự động đồng bộ sang WebApp khi build.
2. Nếu sửa code C# (`.cs`), bắt buộc biên dịch với lệnh:
   ```powershell
   & "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" "d:\SVN\Bao-cao-SCT\Source\ReportDeptTourismSolution\ReportDeptTourismSolution.sln" -p:Configuration=Debug -v:minimal
   ```
3. Chạy script kiểm tra và bảo đảm 100% file có **UTF-8 with BOM**.
4. Chạy toàn bộ test suites (`Run-Tests.ps1`, `Run-ManagementTests.ps1`) và chỉ hoàn thành khi đạt **100% PASS**.
5. **Quy định về Git Branch & Upcode Demo:**
   - **CẤM TUYỆT ĐỐI** tự động merge hoặc push code sang nhánh `upcode-demo` trong quá trình phát triển hoặc sửa lỗi thông thường.
   - Mọi commit và push hàng ngày **CHỈ ĐƯỢC PHÉP** thực hiện trên nhánh làm việc hiện tại (`crm_v2`).
   - **CHỈ ĐƯỢC PHÉP** merge hoặc push sang `upcode-demo` KHI VÀ CHỈ KHI người dùng có chỉ định rõ ràng bằng văn bản (ví dụ: *"upcode demo"*, *"đẩy code demo"*, *"deploy demo"*).

---

## 12. BẢNG CHỐNG BAO BIỆN (ANTI-RATIONALIZATION TABLE)

*Theo triết lý kỹ thuật cao cấp từ Addy Osmani's Agent Skills:*

| AI / Lập trình viên bao biện | Thực tế & Hậu quả thực tế | Quy tắc bắt buộc thi hành |
| :--- | :--- | :--- |
| *"Dùng CustomDisplayName với chuỗi text tự do, không cần quan tâm nó có trả về null hay không."* | Khi thuộc tính DisplayName trả về null, DataAnnotationsModelValidator sẽ gán `context.DisplayName = null`, quăng ngoại lệ `ArgumentNullException: Value cannot be null. Parameter name: value` làm sập HTTP 500 ngay tại tầng Model Binding trước khi Action được gọi. | **BẮT BUỘC CustomDisplayName KHÔNG BAO GIỜ ĐƯỢC PHÉP TRẢ VỀ NULL**. Bắt buộc kế thừa `base(resourceName ?? string.Empty)` và có fallback chuỗi hợp lệ. |
| *"Tiện tay merge và push luôn sang nhánh `upcode-demo` cho server demo cập nhật."* | Vi phạm quy trình kiểm soát release, đẩy mã nguồn đang trong giai đoạn dev/sửa lỗi lên môi trường demo mà chưa được người dùng kiểm duyệt. | **CẤM TỰ Ý PUSH SANG UPCODE-DEMO**. Mọi push thông thường chỉ thực hiện trên `crm_v2`. Chỉ tương tác với `upcode-demo` khi người dùng yêu cầu rõ ràng. |
| *"Gõ thẳng chuỗi tiếng Việt vào View hoặc Controller cho tiện, khai báo Sys_Messages mất công."* | Làm mất khả năng đa ngôn ngữ, khó tùy biến nội dung theo từng khách hàng/triển khai, không đồng bộ thông điệp toàn hệ thống, dễ lỗi font mojibake. | **BẮT BUỘC 100% dùng App_Message**. Mọi chuỗi text trên View và thông báo trong Controller phải được khai báo trong `Sys_Messages` và gọi qua `AppProcessor.Messagor.GetMessage`. |
| *"Dùng thẻ `<input>` hoặc `<label>` thuần cho nhanh, viết `@Html.*` rườm rà."* | Làm mất cơ chế Model Binding 2 chiều, mất thông báo validation đỏ khi nhập sai, mất dấu sao đỏ `(*)` bắt buộc. | **BẮT BUỘC 100% dùng `@Html.*`**. Chỉ dùng thẻ HTML thuần khi cả source code không có helper tương ứng. |
| *"Lồng thẻ `<h1>` và `<div class="page-header">` vào `@section PageTitle` cho đẹp và rõ ràng."* | Phá vỡ flexbox layout của `_PageContent.cshtml`. Khi `BE-ConfigBreadcrumb.js` chạy, nó sẽ xóa sạch nội dung, làm giật màn hình (FOUC). | **`@section PageTitle` CHỈ ĐƯỢC CHỨA `@ViewBag.Title`**. Mọi badge, nút thao tác phải đưa vào `@section PageAction`. |
| *"Dùng Select2 cho các combobox trong Search Card cho hiện đại."* | Select2 sinh thẻ dynamic gây lỗi chiều cao không đồng bộ, làm vỡ form "input to, input nhỏ", lệch hàng so với TextBox. | **CẤM SELECT2 TRONG SEARCH CARD**. 100% combobox dùng thẻ select hoặc `@Html.DropDownListFor` chuẩn 32px. |
| *"Tự viết CSS riêng (`.workflow-card`, `#f7f9fb`, `#0288d1`) cho tiện."* | Phá vỡ tính đồng bộ Design System của Ace Admin v4, gây xung đột theme và khó bảo trì. | **BẮT BUỘC DÙNG DESIGN TOKENS ACE ADMIN** (`.card.bcard`, `.bgc-primary-d1`, `.brc-blue-tp1`,...). |
| *"Dùng button group toggle (`.btn-group-toggle`) thay cho nav-tabs."* | Giao diện trông như nút bấm radio thô kệch, không đồng bộ với phong cách tab danh mục của hệ thống. | **BẮT BUỘC DÙNG ACE NAV TABS** (`.nav.nav-tabs.nav-tabs-simple`). |
| *"Dùng `alert()` của Javascript để thông báo cảnh báo/lỗi."* | Tạo hộp thoại trình duyệt xấu xí, ngắt quãng trải nghiệm người dùng CenIT CRM. | **BẮT BUỘC dùng Toastr** (`toastr.warning`, `toastr.error`) hoặc `CreateMessage`. |
| *"Không cần đặt `data_width`, để Modal tự co giãn theo nội dung."* | Modal sẽ bị co rúm trên màn hình lớn hoặc tràn màn hình trên laptop nhỏ, làm vỡ bố cục form 2 cột. | **BẮT BUỘC chỉ định `data_width = "1024px"` (form lớn), `800px`/`700px` (form vừa), `600px`/`500px` (xóa/xác nhận)**. |
| *"Không cần bọc PartialView ruột trong `<div id="bodyForm">`."* | Khi validation thất bại, Javascript không tìm thấy nơi để đè HTML lỗi, form bị đơ hoặc tải lại toàn trang làm mất dữ liệu người dùng đã gõ. | **BẮT BUỘC có `<div id="bodyForm">` bọc PartialView ruột trong `_Add.cshtml` và `_Edit.cshtml`**. |
| *"Kích hoạt Toastr ngay khi nhận phản hồi, không cần đợi sự kiện `hidden.bs.modal`."* | Gây giật lag, xung đột DOM làm modal bị kẹt lớp mờ đen (`modal-backdrop`), người dùng bị khóa chuột không thể thao tác tiếp. | **BẮT BUỘC đặt `eval(response.message)` và `reload(null, false)` bên trong sự kiện `hidden.bs.modal`**. |
| *"Phó thác submit Form Modal cho file JS bên ngoài, không cần đặt `action` cho form."* | File JS ngoài bị lỗi runtime hoặc cache trình duyệt làm mất event listener, form chuyển sang submit native lên `/Cate/[Entity]` không có Action POST gây sập trang HTTP 500 redirect về `/Error/Error`. | **BẮT BUỘC đặt `action="@Url.Action(...)"` trên `<form>` và code logic submit AJAX khép kín (`e.preventDefault()`) ngay trong Partial View**. |
| *"Định nghĩa hàm JS ở file dưới đáy trang rồi gọi trên View giữa trang, không thêm timestamp `?v=...`."* | Gây lỗi `Uncaught ReferenceError: ... is not defined` do DOM gọi hàm trước khi script đáy trang tải, hoặc do trình duyệt client cache file JS cũ. | **BẮT BUỘC định nghĩa hàm ngay trong Partial View gọi nó, gán `window.funcName`, và thêm `?v=@DateTime.Now.Ticks` cho mọi script tag**. |
| *"Không gắn `[AllowHtml]` cho trường nhập liệu có chứa CKEditor."* | Gây lỗi `HttpRequestValidationException` (HTTP 500) khi người dùng lưu nội dung có thẻ HTML `<p>`, `<div>`. | **BẮT BUỘC gắn `[AllowHtml]` cho trường Note/Description trong C# Model và cấu hình `requestValidationMode="2.0"` trong `Web.config`**. |
| *"Chỉ cần biên dịch C# thành công (Build 0 errors) là xong việc, không cần test lại trên giao diện/dữ liệu thực."* | Biên dịch chỉ kiểm tra cú pháp, hoàn toàn không phát hiện lỗi Javascript runtime, lỗi cache trình duyệt, lỗi validation ASP.NET hay lỗi lệch schema SQL. | **CẤM NGHIỆM THU NẾU CHƯA TEST RUNTIME THỰC TẾ**. Bắt buộc chạy script kiểm thử DB/Service và kiểm tra Console Error trước khi bàn giao. |
| *"Chỉ cần sửa ở `Modules.Cate`, không cần chép sang `publish_source` hay `WebApp`."* | Website IIS chạy trên `publish_source` hoặc `WebApp` sẽ không nhận được thay đổi, gây lỗi không tìm thấy file hoặc chạy code cũ. | **BẮT BUỘC ĐỒNG BỘ 3 NƠI (Triple Mirroring)** và biên dịch DLL đầy đủ. |
| *"Lưu file dạng UTF-8 No BOM cũng chạy được."* | IIS và Razor Engine trên Windows Server sẽ bị lỗi phân tích cú pháp ký tự tiếng Việt có dấu, sinh ra lỗi font mojibake trên production. | **BẮT BUỘC 100% file lưu định dạng UTF-8 with BOM (`0xEF, 0xBB, 0xBF`)**. |

---

## 13. BẢNG CHECKLIST KIỂM THỬ TRƯỚC KHI BÀN GIAO (QA CHECKLIST)

Mọi màn hình hoặc tính năng mới trước khi bàn giao phải vượt qua 100% các tiêu chí:

- [ ] **App_Message Compliance:** 100% chuỗi văn bản trên View và thông báo phản hồi trong Controller được định nghĩa trong `Sys_Messages` và gọi qua `AppProcessor.Messagor.GetMessage`, không hardcode chuỗi text trần.
- [ ] **Encoding:** Toàn bộ file liên quan (`.cshtml`, `.js`, `.cs`, `.sql`) là **UTF-8 with BOM**.
- [ ] **Triple Mirroring:** File đã được đồng bộ đầy đủ ở cả 3 nơi (`Modules.*`, `publish_source`, `WebApp`).
- [ ] **DLL Rebuild & Copy:** Đã biên dịch C# DLL thành công 0 lỗi và copy DLL sang `publish_source\bin` và `WebApp\bin`.
- [ ] **Page Title Cleanliness:** `@section PageTitle` chỉ chứa `@ViewBag.Title`, không lồng thẻ `<h1>` hay `.page-header`.
- [ ] **Search Box Standard:** Card có class `.Search.card.bcard.border-0.shadow-sm.radius-0`, controls cao chuẩn 32px, **KHÔNG dùng Select2**, nút Tìm kiếm/Làm mới đặt ở góc dưới bên trái.
- [ ] **Design Tokens Compliance:** Dùng hoàn toàn class Ace Admin v4 (`.card.bcard`, `.bgc-*-d1`, `.nav-tabs-simple`,...). Không có CSS riêng hoặc mã màu hex tùy tiện.
- [ ] **Form Controls:** 100% sử dụng `@Html.TitleFor`, `@Html.TextBoxFor`, `@Html.DropDownListFor`, `@Html.TextAreaFor`, `@Html.FileFor`. Không dùng thẻ HTML thuần cho controls.
- [ ] **Self-contained Form Submit:** `<form>` có thuộc tính `action` rõ ràng, toàn bộ submit AJAX (`e.preventDefault()`) nằm khép kín trong Partial View, không bị submit native gây lỗi 500.
- [ ] **CKEditor HTML Safe:** Các trường HTML có `[AllowHtml]` trong Model, `requestValidationMode="2.0"` trong `Web.config`, và gọi `updateElement()` trước khi serialize.
- [ ] **No ReferenceError:** Không có hàm JS nào bị thiếu hoặc lỗi scope, mọi hàm gọi từ sự kiện trên view đã được định nghĩa và gán `window`.
- [ ] **Cache-Busting:** Toàn bộ thẻ `<script src="...js">` tự viết đều có query timestamp `?v=@DateTime.Now.Ticks`.
- [ ] **Validation:** Đầy đủ `@Html.ValidationMessageFor` màu đỏ dưới các ô nhập liệu bắt buộc.
- [ ] **Modal Width:** Thuộc tính `data_width` đặt đúng chuẩn (`1024px`, `800px`/`700px`, hoặc `600px`/`500px`).
- [ ] **Form Layout & BodyForm:** Kế thừa đúng `Layout = "~/Views/Shared/_Form.cshtml"`, có `<div id="bodyForm">` bọc ruột.
- [ ] **Modal Lifecycle:** Có cơ chế chờ `hidden.bs.modal` trước khi kích hoạt `eval(message)` hoặc reload dữ liệu, ngăn ngừa triệt để lỗi kẹt backdrop đen.
- [ ] **No Alert:** Không dùng hàm `alert()` thuần, thay bằng `toastr` hoặc `CreateMessage`.
- [ ] **Automated Tests:** Bộ unit test / regression test / verification test đạt 100% PASS (3 tầng: Happy Path, Edge Cases, Error Handling theo `TESTING.md`).
- [ ] **Model Binding & Anti-Null DisplayName:** 100% thuộc tính của Model có `DisplayName` hoặc `CustomDisplayName` trả về chuỗi hợp lệ, không trả về null; `ValidationContext.DisplayName` gán thành công không văng `ArgumentNullException`.

---

## 14. QUY CHUẨN FORM MODAL KHÉP KÍN (SELF-CONTAINED AJAX SUBMIT) & PHÒNG CHỐNG LỖI 500 / REDIRECT /Error/Error

### 14.1. Nguyên nhân gốc rễ lỗi sập trang khi lưu Modal
- `<form>` trong Modal không có thuộc tính `action` hợp lệ hoặc phó mặc việc bind event submit cho file script bên ngoài (`.js`).
- Khi script bên ngoài bị lỗi runtime (do cache hoặc lỗi cú pháp) hoặc chưa kịp gắn listener, người dùng bấm nút submit sẽ kích hoạt **native browser form POST** tới URL hiện tại (`/Cate/[Entity]`).
- Do Controller không có Action `[HttpPost] Index()`, IIS / ASP.NET MVC sẽ quăng lỗi 500 và CustomErrors tự động redirect trình duyệt sang trang `/Error/Error`.

### 14.2. Quy tắc bắt buộc thi hành
1. **BẮT BUỘC khai báo `action` và `method="post"` tường minh cho `<form>`:**
   ```razor
   <form id="frmAdd[Entity]" method="post" action="@Url.Action("Add", "[Entity]", new { area = "Cate" })">
   ```
2. **BẮT BUỘC xử lý AJAX Submit KHÉP KÍN (Self-contained) ngay trong Partial View:**
   - Toàn bộ logic submit AJAX (chặn submit native bằng `e.preventDefault()`, hiển thị icon xoay loading, disable nút submit để chống double-click, bắt phản hồi và chuyển hướng) **PHẢI NẰM NGAY TRONG THẺ `<script>` CỦA CHÍNH FILE `_Add.cshtml` HOẶC `_Edit.cshtml`**.
   - CẤM phó thác việc bắt sự kiện submit của form modal cho file JS chung bên ngoài.
   - Cú pháp mẫu chuẩn khép kín trong Modal:
   ```javascript
   $('#frmAddSales').off('submit').on('submit', function (e) {
       e.preventDefault();
       // Đồng bộ CKEditor nếu có
       if (typeof CKEDITOR !== 'undefined') {
           for (var instance in CKEDITOR.instances) {
               CKEDITOR.instances[instance].updateElement();
           }
       }
       var $form = $(this);
       var $btnSubmit = $form.find('button[type="submit"]');
       $btnSubmit.prop('disabled', true).prepend('<i class="fa fa-spinner fa-spin mr-1"></i>');
       
       $.ajax({
           url: $form.attr('action'),
           type: 'POST',
           data: $form.serialize(),
           success: function (res) {
               $btnSubmit.prop('disabled', false).find('i.fa-spinner').remove();
               if (res.status) {
                   var $modal = $form.closest('.modal');
                   if ($modal.length) {
                       $modal.modal('hide');
                   }
                   toastr.success(res.message);
                   if (res.redirectUrl) {
                       window.location.href = res.redirectUrl;
                   } else if (typeof reloadData === 'function') {
                       reloadData();
                   }
               } else {
                   toastr.error(res.message);
               }
           },
           error: function (xhr) {
               $btnSubmit.prop('disabled', false).find('i.fa-spinner').remove();
               toastr.error('Có lỗi xảy ra khi xử lý yêu cầu (' + xhr.status + ')');
           }
       });
   });
   ```

---

## 15. QUY CHUẨN PHẠM VI JAVASCRIPT & CACHE-BUSTING (CHỐNG LỖI ReferenceError)

### 15.1. Nguyên nhân lỗi `ReferenceError: ... is not defined`
- View hoặc Partial View (ví dụ `_Search.cshtml`) nằm ở giữa trang, gọi hàm JS từ sự kiện inline (`onchange="loadStatusesByBusinessType(this.value)"`) trong khi hàm đó lại được viết ở file `.js` tải dưới đáy trang (`@section BottomScript`).
- Trình duyệt người dùng lưu cache file `.js` phiên bản cũ, nên dù file trên server đã có hàm mới thì client vẫn không có hàm đó trong bộ nhớ execution context.

### 15.2. Quy tắc bắt buộc thi hành
1. **Hàm của View/Partial View nào thì định nghĩa ngay trong thẻ `<script>` của chính View đó:**
   - Các hàm phục vụ dropdown liên kết, lọc tìm kiếm, toggle control của `_Search.cshtml` phải được viết ngay bên trong `_Search.cshtml` và gắn tường minh vào `window`:
   ```javascript
   function loadStatusesByBusinessType(businessType, selectedStatusId) { ... }
   window.loadStatusesByBusinessType = loadStatusesByBusinessType;
   ```
2. **BẮT BUỘC Cache-Busting cho mọi thẻ `<script>` nhúng file JS tự viết:**
   ```razor
   @section BottomScript {
       <script src="~/Areas/Cate/Views/DigitalSales/DigitalSales.js?v=@DateTime.Now.Ticks"></script>
   }
   ```
   - CẤM TUYỆT ĐỐI viết `<script src="...file.js"></script>` trần trụi không có query timestamp version.

---

## 16. QUY CHUẨN XỬ LÝ NỘI DUNG HTML CKEDITOR ([AllowHtml] & requestValidationMode="2.0")

### 16.1. Nguyên nhân lỗi `HttpRequestValidationException`
- Khi form submit dữ liệu có chứa thẻ HTML từ WYSIWYG editor (CKEditor, Summernote,...), cơ chế ASP.NET Request Validation mặc định (chế độ 4.5) sẽ quăng ngoại lệ HTTP 500 trước khi dữ liệu chạm tới Controller.

### 16.2. Quy tắc bắt buộc thi hành
1. **Model C# BẮT BUỘC có thuộc tính `[AllowHtml]`:**
   - Mọi thuộc tính model nhận nội dung giàu định dạng (Ghi chú, Nội dung bài viết, Mô tả chi tiết) phải khai báo:
   ```csharp
   [AllowHtml]
   public string Note { get; set; }
   ```
2. **Cấu hình `requestValidationMode="2.0"` trong `Web.config`:**
   - Trong thẻ `<httpRuntime>` của `Web.config`, bắt buộc có thuộc tính `requestValidationMode="2.0"`:
   ```xml
   <httpRuntime targetFramework="4.8" requestValidationMode="2.0" maxRequestLength="1048576" />
   ```
3. **Đồng bộ CKEditor trước khi Serialize Form:**
   - Trước khi gọi `$form.serialize()` hoặc `new FormData()`, bắt buộc duyệt qua các instances của CKEditor và gọi `updateElement()` để đẩy nội dung từ iframe soạn thảo vào thẻ `<textarea>` ẩn tương ứng.

---

## 17. QUY CHUẨN AN TOÀN MODEL BINDING, DISPLAYNAME & VALIDATION CONTEXT

### 17.1. Nguyên nhân gốc rễ lỗi 500 khi Model Binding (`Value cannot be null. Parameter name: value`)
- Khi Model có các thuộc tính sử dụng `[CustomDisplayName("...")]`, nếu class `CustomDisplayNameAttribute` trả về `null` (do không tìm thấy resource key trong DB/Sys_Messages và `DisplayNameValue` trong base class bị null), ASP.NET MVC Model Binding (`DataAnnotationsModelValidator`) sẽ thực thi:
  ```csharp
  ValidationContext context = new ValidationContext(container, null, null);
  context.DisplayName = metadata.GetDisplayName(); // Nhận giá trị null!
  ```
- Setter `ValidationContext.set_DisplayName(value)` trong .NET Framework quăng ngoại lệ nghiêm ngặt:
  ```csharp
  if (value == null) throw new ArgumentNullException("value");
  ```
  Ngoại lệ này sập ngay tại tầng Model Binding trước khi Action được gọi, khiến toàn bộ form submit bị HTTP 500 và redirect sang `/Error/Error`.

### 17.2. Quy tắc bắt buộc thi hành
1. **`DisplayName` KHÔNG BAO GIỜ ĐƯỢC PHÉP TRẢ VỀ NULL:**
   - Mọi attribute kế thừa `DisplayNameAttribute` BẮT BUỘC gọi constructor cơ sở:
     ```csharp
     public CustomDisplayNameAttribute(string resourceName) : base(resourceName ?? string.Empty)
     ```
   - Thuộc tính `DisplayName` phải luôn có giá trị fallback an toàn (Message -> ResourceName -> `string.Empty`), tuyệt đối cấm trả về null.
2. **An toàn trong Constructor của Custom Validation Attributes:**
   - Các attribute như `[CustomRequired]` phải bọc `try-catch` an toàn khi đọc resource để không văng lỗi khi chạy ngoài `HttpContext` hoặc khi `AppProcessor` chưa khởi tạo.
3. **Kiểm thử Metadata trong Automated Verification Test:**
   - Trước khi nghiệm thu màn hình có Form Submit, bắt buộc phải có script reflection duyệt qua 100% properties của Model, xác nhận `GetDisplayName()` không trả về null và `ValidationContext.DisplayName` gán thành công.
