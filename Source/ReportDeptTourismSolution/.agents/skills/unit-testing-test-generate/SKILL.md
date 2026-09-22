---
name: unit-testing-test-generate
description: Chuyên gia xây dựng, tự động sinh và thực thi các bộ Unit Test / Automated Verification Suite cho hệ thống CenIT ReportTourism (.NET Framework 4.5.2 / ASP.NET MVC 5). Bao phủ kiểm thử Model Binding (Anti-Null DisplayName), Business Cache Layer, Controller Action Lifecycle, Phân quyền 2 tầng (Security Permissions), Quản lý File Storage an toàn, và Sys_Messages DB Coverage.
---

# ⚡ Skill: Unit Testing & Automated Verification Cho CenIT ReportTourism

## 🎯 Mục đích & Phạm vi Hoạt động
Skill này quy định quy trình chuẩn mực và cung cấp công cụ tự động để **phân tích mã nguồn, sinh ca kiểm thử và thực thi Unit Test / Automated Verification** trên toàn bộ hệ thống CenIT ReportTourism (.NET Framework 4.5.2 / ASP.NET MVC 5 / SQL Server).

Mọi lập trình viên và tác nhân AI khi hoàn thành một tính năng mới hoặc chỉnh sửa mã nguồn BẮT BUỘC phải sử dụng skill này để sinh và chạy kiểm thử tự động, tự sửa lỗi nếu phát hiện lỗi trước khi bàn giao cho người dùng.

---

## 📐 Cấu Trúc Kiểm Thử Chuẩn: AAA (Arrange - Act - Assert)

Mọi Unit Test được sinh ra bắt buộc tuân theo khuôn mẫu 3 bước:
1. **Arrange (Chuẩn bị)**: Khởi tạo Model, Controller, Fake/Mock DbContext, Cache, HttpContext, User Session.
2. **Act (Hành động)**: Gọi trực tiếp phương thức cần kiểm tra với tham số đầu vào (Happy path, Edge case, Invalid input).
3. **Assert (Xác minh)**: Khẳng định chặt chẽ kết quả trả về (`Assert.AreEqual`, `Assert.IsTrue`), không viết assertion yếu hoặc hình thức.

---

## 🧪 5 Lớp Kiểm Thử Chuyên Sâu Cho CenIT ReportTourism

Khi sinh Unit Test cho bất kỳ module nào trong ReportTourism (như `Enterprise`, `Accommodation`, `Report`...), BẮT BUỘC sinh đầy đủ 5 lớp kiểm thử sau:

### Lớp 1: Kiểm thử Model Binding & CustomDisplayName (Anti-Null DisplayName)
- **Mục tiêu:** Đảm bảo 100% property có `[CustomDisplayName]` đều phân giải được text hợp lệ, không trả về `null` gây sập `ArgumentNullException: Value cannot be null. Parameter name: value`.
- **Mẫu test:**
```csharp
[Test]
public void CateEnterpriseModel_DisplayName_NeverReturnsNull()
{
    var modelType = typeof(CateEnterpriseModel);
    var properties = modelType.GetProperties();

    foreach (var prop in properties)
    {
        var customAttr = prop.GetCustomAttributes(typeof(CustomDisplayNameAttribute), true)
                             .FirstOrDefault() as CustomDisplayNameAttribute;
        if (customAttr != null)
        {
            var displayName = customAttr.DisplayName;
            Assert.IsNotNull(displayName, $"Property '{prop.Name}' có DisplayName là null!");
            Assert.IsNotEmpty(displayName, $"Property '{prop.Name}' có DisplayName là rỗng!");
        }
    }
}
```

### Lớp 2: Kiểm thử Cache Layer & Stored Procedure Contract
- **Mục tiêu:** Xác minh các hàm `GetByID`, `Save`, `Delete`, `LoadList` giao tiếp đúng với CSDL SQL Server hoặc Cache memory.
- **Mẫu test:**
```csharp
[Test]
public void RM_DigitalSalesCache_GetByID_WithValidId_ReturnsRecord()
{
    // Arrange
    var cache = new RM_DigitalSalesCache();
    int testId = 1; // ID mẫu từ môi trường dev

    // Act
    var result = cache.GetByID(testId);

    // Assert
    if (result != null)
    {
        Assert.AreEqual(testId, result.DigitalSalesID);
        Assert.IsNotNull(result.Title);
    }
}
```

### Lớp 3: Kiểm thử Controller Action & Phân quyền 2 Tầng (Security Gatekeeping)
- **Mục tiêu:** 
  - Tầng 1: Xác thực `[ActionType]` attribute.
  - Tầng 2: Xác thực `HasDetailPermission` (QTHT, Người tạo, AM chủ trì, Thành viên cập nhật trạng thái). Nếu không có quyền, controller PHẢI chặn đứng và trả về JSON `{ status = false, message = "..." }`.
- **Mẫu test:**
```csharp
[Test]
public void Delete_UserWithoutPermission_ReturnsNoPermissionJson()
{
    // Arrange
    var controller = new DigitalSalesController();
    // Giả lập HttpContext với User thường không có quyền
    controller.ControllerContext = MockHelper.CreateMockContext("user_no_perm");

    // Act
    var result = controller.Delete(999) as JsonResult;

    // Assert
    Assert.IsNotNull(result);
    dynamic data = result.Data;
    Assert.IsFalse((bool)data.status);
    Assert.IsTrue(data.message.ToString().Contains("quyền"));
}
```

### Lớp 4: Kiểm thử Quản lý File Upload & An Ninh Tệp Tin (File Storage)
- **Mục tiêu:**
  - File upload phải lưu vào đúng thư mục `/Contents/Uploads/[Module]/[yyyyMM]/`.
  - Chặn đứng 100% các file có extension nguy hiểm (`.exe, .dll, .bat, .cmd, .ps1...`).
- **Mẫu test:**
```csharp
[TestCase("malware.exe")]
[TestCase("virus.bat")]
[TestCase("script.ps1")]
public void SaveUploadedFile_ForbiddenExtensions_ThrowsExceptionOrRejects(string fileName)
{
    // Arrange
    var mockFile = new Mock<HttpPostedFileBase>();
    mockFile.Setup(f => f.FileName).Returns(fileName);
    mockFile.Setup(f => f.ContentLength).Returns(1024);

    // Act & Assert
    var controller = new DigitalSalesController();
    var result = controller.InvokePrivateMethod("SaveUploadedFile", mockFile.Object);
    Assert.IsNull(result, "File nguy hiểm phải bị từ chối lưu!");
}
```

### Lớp 5: Kiểm thử Đa ngôn ngữ Sys_Messages DB Coverage
- **Mục tiêu:** Quét toàn bộ `LabelKey` được gọi trong Controller/Model và đối chiếu trực tiếp với CSDL SQL Server bảng `Sys_Messages`. Không được sót bất kỳ key nào.
- **Mẫu test (PowerShell / Node.js Runner):**
```powershell
# Quét toàn bộ mã nguồn tìm AppProcessor.Messagor.GetMessage("...")
# Đối chiếu với SELECT LabelKey FROM Sys_Messages WHERE LangCode = 'vi-VN'
# Assert: MissingKeys.Count == 0
```

---

## 🚀 Quy Trình Thực Thi & Tự Sửa Lỗi (Self-Fixing Loop)

Khi nhận yêu cầu code hoặc refactor:
1. **Bước 1 (Phân tích):** Đọc yêu cầu và nhận diện các lớp kiểm thử cần thiết.
2. **Bước 2 (Viết Test & Code):** Viết mã nguồn nghiệp vụ đi kèm bộ kiểm thử tự động.
3. **Bước 3 (Chạy Test Runner):** Thực thi script kiểm thử tự động (ví dụ: `test_suite_[module].js` hoặc `run_unit_test.ps1`).
4. **Bước 4 (Tự Phân Tích & Sửa Lỗi):**
   - Nếu có bất kỳ bài test nào **FAIL**: Đọc kỹ stack trace, xác định nguyên nhân gốc rễ, sửa trực tiếp file implementation.
   - Chạy lại test cho đến khi **100% PASS**.
5. **Bước 5 (Báo Cáo Tự Đánh Giá):** Xuất bảng **Self-Evaluation Report** theo chuẩn [TESTING.md](file:///d:/SVN/Bao-cao-SCT/Source/ReportDeptTourismSolution/.agents/rules/TESTING.md).
