# C# Coding Rules (CenIT ReportTourism - .NET Framework 4.5.2)

> **QUY TẮC BẮT BUỘC VỀ PHIÊN BẢN COMPILER:**
> Toàn bộ các project Web và Module (`WebApp`, `CateModule`, `ReportModule`, `SysModule`) sử dụng `Microsoft.Net.Compilers 1.0.0` (Roslyn C# 6.0).  
> **CẤM TUYỆT ĐỐI** sử dụng các tính năng của C# 7.0 trở lên:
> - ❌ `out var x`, `out int total`, `out int _` -> ✅ Bắt buộc khai báo trước: `int total; ... out total`
> - ❌ Tuples / Deconstruction: `(var a, var b) = ...`
> - ❌ Pattern matching: `if (x is MyModel m)`
> - ❌ Local functions
> - ❌ Throw expressions, default literal `default` không có kiểu.

---

## Naming Convention

Class

```csharp
CustomerService
```

PascalCase.

---

Interface

```csharp
ICustomerService
```

Luôn bắt đầu bằng I.

---

Method

```csharp
GetCustomer()

CreateCustomer()

UpdateCustomer()

DeleteCustomer()
```

Tên phải bắt đầu bằng động từ.

---

Property

```csharp
FullName
```

PascalCase.

---

Private Field

```csharp
_logger

_customerRepository
```

camelCase với tiền tố _.

---

Local Variable

```csharp
customer

order

department
```

camelCase.

---

Constant

```csharp
MaxRetry
```

PascalCase.

---

Enum

```csharp
OrderStatus
```

Không dùng Magic Number.

---

Boolean

Đặt tên:

```csharp
isActive

canDelete

hasPermission
```

Không dùng:

flag

status

check

---

# Formatting

4 spaces.

Không dùng tab.

Dấu {

```csharp
public class CustomerService
{
}
```

---

# Exception

Không được:

```csharp
catch(Exception)
{
}
```

Luôn:

```csharp
catch(Exception ex)
{
    _logger.LogError(ex);
    throw;
}
```

---

# Async

Method Async phải có hậu tố

Async

Ví dụ

```csharp
GetCustomerAsync()
```

---

# LINQ

Không viết LINQ quá dài.

Nếu trên 4 phép Select/Where thì tách biến.

---

# Null

Ưu tiên

```csharp
customer?.Name
```

hoặc

```csharp
if(customer == null)
```

---

# String

Ưu tiên

```csharp
string.IsNullOrWhiteSpace()
```

Không dùng

```csharp
value == ""
```

---

# var

Được dùng khi kiểu dữ liệu rõ ràng.

```csharp
var customer = new Customer();
```

Không dùng

```csharp
var a = GetSomethingUnknown();
```

---

# Collection

Ưu tiên

```csharp
List<Customer>
```

Không dùng Array nếu không cần.

---

# Dependency Injection

Không new Service trực tiếp.

Đúng

```csharp
ICustomerService
```

Sai

```csharp
new CustomerService()
```

---

# Repository

Không viết SQL trong Controller.

Controller

↓

Service

↓

Repository

↓

Database

---

# Controller

Controller chỉ xử lý:

- Validate
- Authorization
- Gọi Service

Không viết Business.

---

# Service

Toàn bộ Business Logic nằm trong Service.

---

# Repository

Chỉ truy cập Database.

Không xử lý Business.

---

# Comment

Chỉ comment những đoạn khó hiểu.

Không comment hiển nhiên.

---

# Performance

Không query DB trong foreach.

Không gọi API trong foreach.

Ưu tiên Batch.

---

# Security

Không nối chuỗi SQL.

Không hardcode Password.

Không hardcode Token.

Không hardcode URL Production.

---

# AI Rules

AI không được:

- đổi namespace
- đổi class
- đổi folder
- đổi kiến trúc
- đổi style
- optimize khi chưa yêu cầu

AI phải:

- sinh code giống codebase
- giữ đúng convention
- hạn chế thay đổi Git Diff

---

# Model & Attribute Safety (Phòng chống lỗi Model Binding 500)

## 1. Cấm DisplayName trả về NULL
- Mọi Custom Attribute kế thừa `DisplayNameAttribute` (ví dụ `CustomDisplayNameAttribute`):
  - BẮT BUỘC gọi `base(resourceName ?? string.Empty)` để `DisplayNameValue` không bị null.
  - Getter `DisplayName` BẮT BUỘC có fallback an toàn (trả về message dịch được, hoặc chuỗi resource name, hoặc `string.Empty`). **CẤM TUYỆT ĐỐI TRẢ VỀ NULL**.
  - *Hậu quả nếu vi phạm:* Khi ASP.NET MVC thực hiện Model Binding, `DataAnnotationsModelValidator` gán `context.DisplayName = metadata.GetDisplayName()`. `ValidationContext.set_DisplayName` sẽ quăng ngoại lệ `ArgumentNullException: Value cannot be null` làm sập toàn bộ request với HTTP 500 trước khi Action được gọi.

## 2. An toàn trong Constructor của Custom Validation Attributes
- Mọi Custom Validation Attribute (kế thừa `RequiredAttribute`, `ValidationAttribute`):
  - Constructor không được ném ngoại lệ nếu chạy ngoài HttpContext hoặc khi AppProcessor chưa khởi tạo (bắt buộc bọc `try-catch` khi đọc resource/message).