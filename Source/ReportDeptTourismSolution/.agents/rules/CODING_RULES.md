# Coding Rules

Version: 1.0

## Mục tiêu

Mọi source code được AI hoặc lập trình viên tạo ra phải:

- Tuân thủ kiến trúc hiện có.
- Không làm thay đổi business logic nếu không được yêu cầu.
- Không thay đổi coding style của dự án.
- Chỉ sửa đúng phạm vi yêu cầu.
- Không gây lỗi Unicode hoặc lỗi font tiếng Việt.

---

# Nguyên tắc ưu tiên

Ưu tiên theo thứ tự:

1. Giữ nguyên kiến trúc.
2. Không ảnh hưởng chức năng.
3. Đồng nhất coding style.
4. Dễ đọc.
5. Dễ bảo trì.

---

# Không được phép

❌ Không rename namespace.

❌ Không rename project.

❌ Không rename class nếu không được yêu cầu.

❌ Không thay đổi folder structure.

❌ Không tự tạo package mới.

❌ Không đổi framework.

❌ Không đổi phiên bản .NET.

❌ Không tự động refactor toàn bộ file.

❌ Không format toàn bộ project.

❌ Không sửa các file không liên quan.

❌ Không bao giờ commit hoặc push các file trong thư mục `bin/` và `obj/` (`*.dll`, `*.pdb`, `*.cache`, `*.FileListAbsolute.txt`, v.v.).

❌ Không dùng lệnh `git add .` hoặc `git add -A` bừa bãi vì sẽ kéo theo các file nhị phân và cache biên dịch trong `bin/` và `obj/` vào Git staging.

---

# Quy tắc Encoding

Toàn bộ source phải sử dụng UTF-8.

Không được:

- xuất hiện ký tự �
- xuất hiện ký tự ?
- lỗi tiếng Việt
- ANSI
- UTF-16 nếu project không dùng

Không copy code từ Word.

---

# Quy tắc khi AI sinh code

AI phải:

- Đọc coding style hiện tại trước khi viết.
- Viết giống style hiện có.
- Không tự ý tối ưu khi chưa được yêu cầu.
- Không đổi business logic.
- Không sinh code dư thừa.
- Không tạo helper mới nếu project đã có helper tương tự.
- Không duplicate code.

---

# Source Control

Không commit:

- bin (tuyệt đối không commit các file `*.dll`, `*.pdb`, `*.cache`...)
- obj (tuyệt đối không commit `*.cache`, `*.FileListAbsolute.txt`...)
- publish
- temp
- backup
- package

Không commit:

- Password
- Token
- Connection String Production

### Quy tắc kiểm soát tệp bin/ và obj/ (Anti-Bin/Obj Git Pollution):
- **CẤM TUYỆT ĐỐI** commit và push các tệp tin trong thư mục `bin/` và `obj/` (`*.dll`, `*.pdb`, `*.cache`, `*.FileListAbsolute.txt`...).
- **CẤM DÙNG `git add .` HOẶC `git add -A` BỪA BÃI:**
  - BẮT BUỘC dùng lệnh `git add` chỉ định đích danh từng tệp mã nguồn cần commit (ví dụ: `git add Path/To/File.cs Path/To/View.cshtml`).
  - Trước khi commit, PHẢI chạy `git status` hoặc `git diff --cached --name-status` để kiểm tra danh sách staged files. Nếu có bất kỳ tệp nào thuộc `bin/` hay `obj/`, phải lập tức hủy stage bằng `git reset HEAD <file>`.
  - Luôn duy trì tệp `.gitignore` chuẩn ở thư mục gốc repo và solution để chặn tự động các thư mục build.

### Quy tắc nhánh & Upcode Demo:
- **CẤM TUYỆT ĐỐI tự động push hoặc merge sang nhánh `upcode-demo`!**
- Mọi thao tác commit và push code hàng ngày **CHỈ ĐƯỢC PHÉP** thực hiện trên nhánh làm việc chính hiện tại (`crm_v2` hoặc nhánh làm việc được chỉ định).
- **CHỈ ĐƯỢC PHÉP** merge hoặc push sang nhánh `upcode-demo` KHI VÀ CHỈ KHI người dùng có yêu cầu rõ ràng bằng văn bản (ví dụ: *"upcode demo"*, *"đẩy lên demo"*, *"deploy demo"*).

---

# Logging

Được phép log:

- Error
- Warning
- Information

Không log:

- Password
- JWT
- Access Token
- Refresh Token
- Connection String

---

# Quy chuẩn Tách Biệt Style (.css) Khỏi Razor View (.cshtml)

- **CẤM TUYỆT ĐỐI viết thẻ `<style>` nội tuyến** trực tiếp trong các file Razor View (`.cshtml`).
- **Tách riêng CSS ra file độc lập:** Mỗi file `.cshtml` nếu có định nghĩa CSS riêng BẮT BUỘC phải tách ra một file `.css` riêng biệt.
- **Đưa file style vào CHUNG THƯ MỤC với View (Co-located View & Style):**
  - File `.css` đặt ngay tại thư mục chứa file view tương ứng (ví dụ: `Areas/Cate/Views/DigitalSales/_DetailDiscussions.css` đặt cùng thư mục với `_DetailDiscussions.cshtml`).
  - Nhúng CSS vào view kèm cache-busting timestamp:
    `<link rel="stylesheet" href="~/Areas/[Area]/Views/[Folder]/[ViewName].css?v=@DateTime.Now.Ticks" />`
    + View cha có Layout: Đặt trong `@section HeadCss { ... }`.
    + Partial View nạp AJAX / Modal: Đặt thẻ `<link rel="stylesheet" ... />` trực tiếp ở đầu partial view.
- **Tuân thủ Triple Mirroring:** Toàn bộ file `.css` mới phải được sao chép và đồng bộ MD5 trên cả 3 cây thư mục (`Modules.*`, `publish_source`, `WebApp`).

---

# Checklist trước khi Commit

- Build thành công.
- Không warning mới.
- Không còn TODO.
- Không còn Console.WriteLine.
- Không còn dữ liệu test.
- Không lỗi Unicode.
- Không lỗi Encoding.
- Không còn thẻ `<style>` nội tuyến trong file `.cshtml` (100% tách ra file `.css` cùng thư mục).
- Không sửa file ngoài phạm vi.
- Không có bất kỳ file nào trong thư mục `bin/` hoặc `obj/` nằm trong staged list (`git diff --cached --name-status`).