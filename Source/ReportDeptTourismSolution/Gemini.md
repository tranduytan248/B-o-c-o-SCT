# 📘 BỘ QUY TẮC PHÁT TRIỂN HỆ THỐNG BÁO CÁO THỐNG KÊ DU LỊCH (CENIT REPORT TOURISM)

> **Dự án:** Hệ thống Quản lý & Báo cáo Thống kê Du lịch (Báo cáo Sở Du lịch / Sở Công Thương) - `ReportDeptTourismSolution.sln`  
> **Áp dụng cho:** Toàn bộ lập trình viên và AI Agent khi xây dựng, chỉnh sửa Model, Biz, Cache, Controller, View, JavaScript, Report và Stored Procedure trong giải pháp.  
> **Nguyên tắc cốt lõi:**  
> 1. Bắt buộc 100% tệp tin định dạng **UTF-8 with BOM**.  
> 2. Tuân thủ nghiêm ngặt **C# 6.0 (.NET Framework 4.5.2)** do gói `Microsoft.Net.Compilers 1.0.0` kiểm soát: **CẤM** dùng cú pháp C# 7.0+ (như inline `out var`, `out int _`, tuple deconstruction, pattern matching).  
> 3. Chuẩn hóa giao diện theo **AdminLTE 2.3.11 + Bootstrap 3.3.6 + skin-vnpt** (`skin-vnpt.css`), icon Font Awesome 4.7.0.  
> 4. Quy chuẩn Form dùng chung (`_Entity.cshtml` + vỏ bọc `_Add.cshtml`, `_Edit.cshtml` kế thừa `~/Views/Shared/_Form.cshtml`), submit bằng `ajaxForm`, thông báo qua `AppProcessor.Messagor`.  
> 5. Kiến trúc đa tầng khép kín: `WebApp / Areas` -> `Controller` -> `CacheLayer` -> `Biz` -> `Plugable.SQLStoreProcedure` (`*_StoredProcedures.xml`) -> SQL Server.  
> 6. Tích hợp giải pháp ký số từ xa **VNPT SmartCA** (`CenIT.Libs.VNPTSmartCA`) cho các biểu mẫu và báo cáo số liệu.

---

## 1. TỔNG QUAN KIẾN TRÚC HỆ THỐNG (SYSTEM ARCHITECTURE)

Solution `ReportDeptTourismSolution.sln` gồm 18 projects được phân bổ rõ ràng theo các tầng trách nhiệm:

```
ReportDeptTourismSolution/
├── Core Framework & Plugable:
│   ├── TSFramework.Plugable           # Interface mở rộng Plugable Providers
│   ├── TSFramework.Core               # Tiện ích nền tảng, Caching Base, Utility, Security
│   ├── TSFramework.App                # Attributes, Processors, MVC View Helpers, Model Bindings
│   ├── Plugable.SQLStoreProcedure     # Data Provider thực thi SQL Stored Procedures
│   └── Plugable.SQLProcedureAuthority # Provider phân quyền chức năng hệ thống
│
├── Business & Data Access:
│   ├── CenIT.ReportTourism.Models     # Data Models: Cate, Report, Sys
│   ├── CenIT.ReportTourism.Biz        # Business Logic, gọi Stored Procedures qua ProcedureProvider
│   ├── CenIT.ReportTourism.Caches     # Cache Layer kế thừa CacheLayer, quản lý Memory Cache
│   ├── CenIT.ReportTourism.Core       # Hằng số hệ thống, Application Context, Helpers
│   └── CenIT.ReportTourism.AppMembership # Quản lý xác thực người dùng, Membership Provider
│
├── Modules & UI Areas (ASP.NET MVC 5):
│   ├── CenIT.ReportTourism.Modules.CateModule   # Phân hệ Quản lý Danh mục & Doanh nghiệp (Area: Cate)
│   ├── CenIT.ReportTourism.Modules.ReportModule # Phân hệ Báo cáo Thống kê & Ký số (Area: Report)
│   ├── CenIT.ReportTourism.Modules.SysModule    # Phân hệ Quản trị Hệ thống, Phân quyền, Cấu hình (Area: Sys)
│   └── CenIT.ReportTourism.WebApp               # Host Application, Xác thực, Controllers dùng chung
│
├── Reports Engine & Quartz Background Jobs:
│   ├── CenIT.ReportTourism.Reports.UocKetQuaHoatDongKinhDoanh # Báo cáo Ước KQ HĐ Kinh doanh (RDLC)
│   ├── CenIT.ReportTourism.Reports.ThongKeQuocTichKhachDuLich # Báo cáo Thống kê Quốc tịch Khách (RDLC)
│   ├── CenIT.ReportTourism.Jobs.NotifyEnterprise              # Quartz Job gửi thông báo nhắc báo cáo
│   └── CenIT.ReportTourism.Jobs.RefreshSite                   # Quartz Job làm tươi hệ thống định kỳ
│
└── Tích hợp dịch vụ Ký số:
    └── CenIT.Libs.VNPTSmartCA        # Thư viện tích hợp dịch vụ Ký số từ xa VNPT SmartCA
```

---

## 2. QUY TẮC C# 6.0 & BIÊN DỊCH MSBUILD (COMPILER CONSTRAINTS)

> [!WARNING]
> **CẢNH BÁO QUAN TRỌNG:** Dự án sử dụng bộ biên dịch `Microsoft.Net.Compilers 1.0.0` (Roslyn C# 6.0). Cú pháp C# 7.0 trở lên sẽ gây lỗi biên dịch `CS1525: Invalid expression term` hoặc `CS1003: Syntax error`.

### 2.1. Quy tắc tham số `out`
- **❌ CẤM TUYỆT ĐỐI:**
  ```csharp
  // SAI - C# 7.0 inline out variable
  var data = _enterpriseCache.Get(userName, typeIds, wardIds, out var total, dataSearch);
  var lstWards = _wardCache.GetByProvinceId(provinceId, out int total);
  var lstDistricts = _districtCache.Get(provinceId, out int _);
  ```
- **✅ BẮT BUỘC:** Khai báo biến trước khi gọi hàm:
  ```csharp
  // ĐÚNG - C# 6.0 standard
  int total;
  var data = _enterpriseCache.Get(userName, typeIds, wardIds, out total, dataSearch);

  int totalWards;
  var lstWards = _wardCache.GetByProvinceId(provinceId, out totalWards);
  ```

### 2.2. Các cú pháp hiện đại khác bị cấm
- Không dùng cú pháp deconstruction: `var (x, y) = GetCoords();`
- Không dùng pattern matching: `if (obj is CateWardModel ward)` (dùng `var ward = obj as CateWardModel; if (ward != null)`).
- Không dùng local functions bên trong method.

### 2.3. Lệnh biên dịch chuẩn từ dòng lệnh
Sử dụng MSBuild Visual Studio 2022 Community:
```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" "d:\SVN\Bao-cao-SCT\Source\ReportDeptTourismSolution\ReportDeptTourismSolution.sln" -p:Configuration=Debug -v:minimal
```

---

## 3. QUY TẮC ENCODING & FONT TIẾNG VIỆT (UTF-8 WITH BOM)

- **100% tệp tin mã nguồn:** `.cshtml`, `.js`, `.cs`, `.sql`, `.xml`, `.config`, `.md` **BẮT BUỘC** lưu dưới dạng **UTF-8 with BOM** (`0xEF, 0xBB, 0xBF`).
- **Nghiêm cấm tuyệt đối:** Lưu file dạng ANSI hoặc UTF-8 No BOM, tránh làm hỏng các ký tự tiếng Việt có dấu khi Razor Engine biên dịch trên máy chủ IIS.

---

## 4. QUY CHUẨN GIAO DIỆN (ADMINLTE 2.3.11 + BOOTSTRAP 3 + SKIN-VNPT)

Hệ thống sử dụng bộ giao diện quản trị **AdminLTE 2.3.11** kết hợp **Bootstrap 3.3.6** và skin nhận diện **skin-vnpt**.

### 4.1. Cấu trúc trang danh sách chuẩn (`Index.cshtml`)
```razor
@using TSFramework.App.Processors
@{
    ViewBag.Title = AppProcessor.Messagor.GetMessage("Enterprise_Title");
}

@section Title
{
    @ViewBag.Title
}

@section Action
{
    <div class="col-md-12 text-center">
        @Html.Button(true, "AddEnterprise", Url.Action("Add", new { area = "Cate" }), 
                     "<i class='fa fa-plus'></i>", 
                     AppProcessor.Messagor.GetMessage("Button_Add"), 
                     new { @class = "btn btn-sm btn-success", data_width = "1024px" })

        @Html.Button(true, "ImportEnterprise", Url.Action("Import", new { area = "Cate" }), 
                     "<i class='fa fa-cloud-upload'></i>", 
                     AppProcessor.Messagor.GetMessage("Button_Import"), 
                     new { @class = "btn btn-sm btn-primary", data_width = "1024px" })
    </div>
}

<div class="row">
    <div class="col-sm-12">
        @Html.Partial("_Search")
    </div>
    <div class="col-sm-12">
        <div class="table-responsive">
            <table id="DSEnterprise" class="table table-bordered table-striped" width="100%">
                <thead>
                    <tr>
                        <th>@AppProcessor.Messagor.GetMessage("Column_No")</th>
                        <th>@AppProcessor.Messagor.GetMessage("Enterprise_Label_BusinessName")</th>
                        <th>@AppProcessor.Messagor.GetMessage("Enterprise_Label_TaxCode")</th>
                        <th>@AppProcessor.Messagor.GetMessage("Column_Action")</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
</div>

@section CustomJS
{
    <script src="@Url.Content("~/Areas/Cate/Views/Enterprise/Enterprise.js?v=" + DateTime.Now.Ticks)"></script>
    @Html.RenderScripts(ScriptPosition.BodyEnd)
}
```

### 4.2. Cấu trúc Khung Tìm kiếm (`_Search.cshtml`)
Khung tìm kiếm sử dụng cấu trúc AdminLTE Box:
```razor
@using TSFramework.App.Processors
@model CenIT.ReportTourism.Modules.CateModule.Areas.Cate.Models.EnterpriseSearchModel

<div class="box box-primary">
    <div class="box-header with-border">
        <h3 class="box-title">@AppProcessor.Messagor.GetMessage("Condition_Title")</h3>
        <div class="box-tools pull-right">
            <button type="button" class="btn btn-box-tool" data-widget="collapse">
                <i class="fa fa-minus"></i>
            </button>
        </div>
    </div>
    <div class="box-body with-border">
        <div class="row form-horizontal" id="Search">
            <div class="col-md-6">
                <div class="form-group">
                    @Html.TitleFor(m => m.ListTypeBusinessId, new { @class = "control-label col-md-4" })
                    <div class="col-md-8">
                        @Html.DropDownListFor(m => m.ListTypeBusinessId, new SelectList(Model.ListTypeBusiness, "Value", "Text"), "", 
                            new { multiple = true, @class = "col-md-12 chosen-select", data_placeholder = AppProcessor.Messagor.GetMessage("Combobox_Select") })
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="form-group">
                    @Html.TitleFor(m => m.ListDistrictId, new { @class = "control-label col-md-4" })
                    <div class="col-md-8">
                        @Html.DropDownListFor(m => m.ListDistrictId, new SelectList(Model.ListDistricts, "Value", "Text"), "", 
                            new { multiple = true, @class = "col-md-12 chosen-select", data_placeholder = AppProcessor.Messagor.GetMessage("Combobox_Select") })
                    </div>
                </div>
            </div>
        </div>
        <div class="box-footer">
            <div class="col-md-12 text-center">
                <button type="button" class="btn btn-primary" onclick="_tableEnterprise.ajax.reload(null, false);">
                    <i class="fa fa-search-plus"></i>&nbsp;@AppProcessor.Messagor.GetMessage("Button_Search")
                </button>
            </div>
        </div>
    </div>
</div>
```

---

## 5. QUY CHUẨN FORM CONTROLS & SHARED FORM PATTERN

### 5.1. Mô hình Form 3 tệp
- `_[Entity].cshtml`: Chứa toàn bộ nội dung form, controls nhập liệu, dùng chung cho cả Add và Edit.
- `_Add.cshtml`: Vỏ bọc form thêm mới, kế thừa layout `~/Views/Shared/_Form.cshtml`.
- `_Edit.cshtml`: Vỏ bọc form cập nhật, kế thừa layout `~/Views/Shared/_Form.cshtml`.

### 5.2. Mẫu vỏ bọc Modal Form (`_Add.cshtml` / `_Edit.cshtml`)
```razor
@using TSFramework.App.Processors
@{
    ViewBag.Title = string.Format(AppProcessor.Messagor.GetMessage("Modal_Title_Add"), AppProcessor.Messagor.GetMessage("Enterprise_Title"));
    Layout = "~/Views/Shared/_Form.cshtml";
}

@section FormType {
    bg-primary
}

@section FormIcon {
    <i class="fa fa-plus"></i>
}

@section FormBody {
    @using (Html.BeginForm("Add", "Enterprise", new { area = "Cate" }, FormMethod.Post, 
        new { encType = "multipart/form-data", id = "AddEnterprise", @class = "form-horizontal" }))
    {
        <div id="bodyForm">
            @Html.Partial("_Enterprise", Model)
        </div>
    }

    <script type="text/javascript">
        $('form#AddEnterprise').ajaxForm({
            success: function(response) {
                if (response.status != undefined) {
                    $("#ModalContent #modal_AddEnterprise").modal("hide");
                    $("#ModalContent #modal_AddEnterprise").on('hidden.bs.modal', function() {
                        if (response.status != undefined) {
                            eval(response.message);
                            _tableEnterprise.ajax.reload(null, false);
                            response.status = undefined;
                        }
                    });
                } else {
                    $("#ModalContent #modal_AddEnterprise #bodyForm").html(response);
                }
            }
        });
    </script>
}
```

### 5.3. Bảng quy chuẩn Controls `@Html.*`
| Thành phần | ❌ Không dùng | ✅ Bắt buộc dùng | Ghi chú |
| :--- | :--- | :--- | :--- |
| **Nhãn trường** | `<label>...</label>` | `@Html.TitleFor(m => m.FieldName, new { @class = "control-label col-md-3" })` | Tự động đọc nhãn từ `[CustomDisplayName]` |
| **Ô nhập văn bản** | `<input type="text"...>` | `@Html.TextBoxFor(m => m.FieldName, new { @class = "form-control" })` | Hỗ trợ model binding |
| **Dropdown đơn** | `<select>...</select>` | `@Html.DropDownListFor(m => m.FieldId, new SelectList(Model.ListItems, "Value", "Text"), "-- Chọn --", new { @class = "form-control" })` | Chuẩn danh mục |
| **Dropdown đa chọn** | `<select multiple...>` | `@Html.DropDownListFor(m => m.ListIds, new SelectList(...), "", new { multiple = true, @class = "chosen-select" })` | Tích hợp Select2 / Chosen |
| **Ô nhập nhiều dòng** | `<textarea>...</textarea>`| `@Html.TextAreaFor(m => m.Description, new { @class = "form-control", rows = 3 })` | Mô tả, ghi chú |
| **Validation Error** | Tự viết span | `@Html.ValidationMessageFor(m => m.FieldName, "", new { @class = "text-danger" })` | Hiển thị lỗi xác thực model |
| **Chống CSRF** | Bỏ qua token | `@Html.AntiForgeryToken()` | Bắt buộc trong form POST |

---

## 6. KIẾN TRÚC TẦNG DỮ LIỆU & CACHE (CACHE & BIZ PATTERN)

### 6.1. Cache Layer (`CenIT.ReportTourism.Caches`)
- Mọi thao tác truy vấn dữ liệu danh mục phải đi qua tầng `CacheLayer`.
- Sử dụng `MasterCacheKeyArray` để quản lý việc hủy cache (invalidation) khi có thêm/sửa/xóa:
```csharp
[DataObject]
public class CateDistrictCache : CacheLayer
{
    private CateDistrictBiz _districtApi;
    private CateDistrictBiz Api => _districtApi ?? (_districtApi = new CateDistrictBiz());

    protected override string[] MasterCacheKeyArray =>
        new[] { "DistrictsCache", "ProvincesCache", "CENIT.APP.Cache" };

    [DataObjectMethod(DataObjectMethodType.Select, true)]
    public List<CateDistrictModel> GetAll(int? provinceId = null)
    {
        var rawKey = $"AllDistricts-{provinceId}";
        if (GetCacheItem(rawKey) is List<CateDistrictModel> items) return items;

        items = Api.GetAll(provinceId);
        AddCacheItem(rawKey, items);
        return items;
    }

    [DataObjectMethod(DataObjectMethodType.Update, false)]
    public int? Save(CateDistrictModel model)
    {
        var id = Api.Save(model);
        if (id > 0) InvalidateCache();
        return id;
    }
}
```

### 6.2. Biz Layer (`CenIT.ReportTourism.Biz`)
- Thực thi thông qua `AppProcessor.ProcedureProvider.ExecuteTypedList<T>` hoặc `ExecuteScalarObject<T>`.
- Khóa thủ tục được định nghĩa trong `App_Data/Modules/*_StoredProcedures.xml`.

---

## 7. TÍCH HỢP KÝ SỐ VNPT SMARTCA

Phân hệ báo cáo tích hợp trực tiếp Ký số từ xa **VNPT SmartCA** thông qua project `CenIT.Libs.VNPTSmartCA`:
1. **Xác thực SmartCA:** `AccountController.LoginSmartCA` kết nối gateway SmartCA lấy access token.
2. **Ký số Báo cáo Du lịch:** Sau khi số liệu báo cáo được tổng hợp, cán bộ phụ trách thực hiện ký số xác nhận báo cáo.
3. **Quản lý tài liệu đã ký:** File báo cáo sau khi ký được lưu trữ an toàn trong thư mục `/Contents/Modules/Report/ReportSignedDocs/`.

---

## 8. QUY TRÌNH KIỂM THỬ 5 TẦNG TRƯỚC KHI BÀN GIAO

Mỗi thay đổi mã nguồn trước khi bàn giao PHẢI đáp ứng:
1. **Tầng 1 (Build):** Biên dịch toàn bộ solution qua MSBuild đạt `0 Error(s)`.
2. **Tầng 2 (Encoding & Mirroring):** 100% tệp tin UTF-8 with BOM; PostBuildEvent robocopy sang `WebApp` thành công.
3. **Tầng 3 (DOM ID Collision):** Các trường nhập liệu trong Modal Add/Edit không trùng ID với trang cha.
4. **Tầng 4 (Sys_Messages Coverage):** Mọi key `AppProcessor.Messagor.GetMessage("...")` đều tồn tại trong DB `Sys_Messages`.
5. **Tầng 5 (Clean Code):** Không hardcode chuỗi tiếng Việt vào mã C#, không viết thẻ `<style>` nội tuyến trong `.cshtml`.

---

## 9. QUY TẮC BẮT BUỘC KHI COMMIT & PUSH CODE (ANTI-BIN/OBJ POLLUTION)

> [!CAUTION]
> **NGHIÊM CẤM PUSH CÁC FILE TRONG THƯ MỤC BIN VÀ OBJ LÊN GIT:**
> 1. **CẤM TUYỆT ĐỐI** commit và push các tệp tin trong thư mục `bin/` và `obj/` (`*.dll`, `*.pdb`, `*.cache`, `*.FileListAbsolute.txt`...).
> 2. **CẤM DÙNG `git add .` HOẶC `git add -A` BỪA BÃI:**
>    - Thao tác này sẽ vô tình đưa hàng trăm MB file binary biên dịch vào Git repository, gây phình to repo và conflict nhánh.
>    - **BẮT BUỘC** `git add` đích danh từng file mã nguồn cần commit:
>      ```powershell
>      # Ví dụ: Chỉ add file mã nguồn cụ thể
>      git add Path/To/File.cs Path/To/View.cshtml Gemini.md Memory.md
>      ```
> 3. **KIỂM TRA BẮT BUỘC TRƯỚC KHI COMMIT:**
>    - Luôn chạy lệnh `git status` hoặc `git diff --cached --name-status` để rà soát toàn bộ danh sách file đã stage.
>    - Nếu phát hiện bất kỳ file nào nằm trong `bin/` hoặc `obj/`, phải lập tức unstage bằng `git reset HEAD <file>`.
> 4. **CẤU HÌNH LOẠI TRỪ TRONG `.gitignore`:**
>    - Duy trì tệp `.gitignore` chuẩn cho .NET Framework để tự động bỏ qua toàn bộ `[Bb]in/`, `[Oo]bj/`, `packages/`, `*.suo`, `*.user`, `build.log`.
