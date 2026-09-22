---
name: browser-testing-ui-ux
description: Chuyên gia kiểm thử giao diện trực quan (UI/UX) và chẩn đoán runtime thông qua trình duyệt / DevTools. Kế thừa triết lý browser-testing-with-devtools từ Addy Osmani, chuyên sâu kiểm tra lỗi Console, kiểm toán Network AJAX, xác thực vòng đời Modal Bootstrap, kiểm tra vỡ layout bảng dữ liệu DataTable, và thẩm định tương tác Form validation client/server trên hệ thống Web CenIT TOC CRM và ứng dụng đa nền tảng.
---

# 🧪 Skill: Browser Testing UI/UX — Kiểm Thử Giao Diện & Runtime Trình Duyệt

> **Cảm hứng & Triết lý:** Kế thừa phương pháp luận `browser-testing-with-devtools` từ [Addy Osmani's Agent Skills](https://github.com/addyosmani/agent-skills).  
> **Nguyên tắc cốt lõi:** *"Không suy đoán giao diện dựa trên code — phải chứng minh bằng bằng chứng render thực tế trên trình duyệt."*  
> Đo lường và xác thực qua 5 trục: Console sạch lỗi, Network phản hồi chuẩn, DOM & Bố cục ổn định, Vòng đời Modal không kẹt backdrop, và Trải nghiệm tương tác Form mượt mà.

---

## 🎯 Khi Nào Kích Hoạt Skill Này?

- Sau khi xây dựng hoặc sửa đổi View `.cshtml`, JavaScript `.js`, hoặc Modal.
- Kiểm tra tính đúng đắn của vòng đời Form: Thêm mới, Chỉnh sửa, Xóa, Cập nhật trạng thái.
- Phát hiện và sửa lỗi kẹt lớp mờ đen của Modal (`modal-backdrop`), lỗi xung đột z-index, hoặc lỗi tràn viền (Horizontal scrollbar).
- Xác minh hiển thị thông báo lỗi xác thực đỏ (`@Html.ValidationMessageFor`) khi submit dữ liệu không hợp lệ.
- Kiểm toán tải tài nguyên (0 lỗi 404 cho CSS, JS, Fonts, CKEditor) và kiểm tra lỗi JavaScript Console.

---

## 🔍 5 Trục Kiểm Thử Runtime Trình Duyệt (The 5-Axis Verification Gate)

```
                       ┌─────────────────────────┐
                       │  CONSOLE ZERO-ERROR     │ (0 JS Errors, 0 Uncaught Warnings)
                       └───────────┬─────────────┘
                                   │
       ┌───────────────────────────┼───────────────────────────┐
       ▼                           ▼                           ▼
┌───────────────┐          ┌───────────────┐          ┌────────────────┐
│ NETWORK AUDIT │          │ DOM & MODAL   │          │ FORM & STATE   │
│ (200 OK, Clean│          │ (data_width,  │          │ (Validation đỏ,│
│  JSON/Partial)│          │  No Backdrop) │          │  Keep Inputs)  │
└───────┬───────┘          └───────┬───────┘          └────────┬───────┘
        │                          │                           │
        └──────────────────────────┼───────────────────────────┘
                                   ▼
                       ┌─────────────────────────┐
                       │ RESPONSIVE & CONTRAST   │ (WCAG AA 4.5:1, 48px touch)
                       └─────────────────────────┘
```

---

### 1. Trục 1: Console Zero-Error (Kiểm Toán Bảng Điều Khiển JS)
- **Tiêu chí ĐẠT:** Mở DevTools Console (`F12`), duyệt qua trang danh sách, mở modal Thêm mới/Sửa/Xóa, thực hiện submit form:
  - **TUYỆT ĐỐI 0** lỗi đỏ: `Uncaught TypeError`, `ReferenceError`, `jQuery is not defined`, `$ is not a function`.
  - Không có cảnh báo thư viện bị deprecated hoặc tải trùng lặp version của jQuery / Bootstrap.

### 2. Trục 2: Network Audit (Kiểm Toán Luồng Mạng AJAX)
- **Tiêu chí ĐẠT:** Tab Network của trình duyệt:
  - **Tài nguyên tĩnh (Static Assets):** Toàn bộ file CSS, JS, Select2, FontAwesome, CKEditor, ảnh icon phải trả về HTTP 200 hoặc 304. **Nghiêm cấm xuất hiện HTTP 404**.
  - **Endpoint Lấy Dữ Liệu (`/Get`):** Trả về JSON đúng cấu trúc DataTables: `{ data: [...], recordsTotal: N, recordsFiltered: N }`.
  - **Endpoint Ghi Dữ Liệu (`/Add`, `/Edit`, `/Delete`):**
    - Khi thành công: Trả về JSON `{ status: true, message: "..." }`.
    - Khi validation lỗi: Trả về HTML Partial View chứa các thẻ lỗi và mã form với HTTP 200 (không được trả về 500 Internal Server Error).

### 3. Trục 3: Vòng Đời Modal & DOM Layout (Modal Lifecycle & Backdrop)
- **Kích thước hiển thị:**
  - Nút có `data_width = "1024px"` $\rightarrow$ Hộp thoại modal phải mở rộng chuẩn 1024px trên màn hình desktop, không bị co rúm thành 500px.
  - Nút xác nhận xóa `data_width = "600px"` $\rightarrow$ Hộp thoại mở vừa vặn, căn giữa màn hình.
- **Hiện tượng kẹt Backdrop (`modal-backdrop`):**
  - Mở modal $\rightarrow$ Nhấn "Đóng" hoặc lưu thành công $\rightarrow$ Kiểm tra DOM: Thẻ `<div class="modal-backdrop">` phải biến mất hoàn toàn, class `modal-open` trên thẻ `<body>` phải được gỡ bỏ.
  - Người dùng có thể cuộn trang và nhấp chuột vào các nút trên màn hình chính bình thường mà không bị lớp kính mờ che khuất.

### 4. Trục 4: Form Validation & Giữ Dữ Liệu Người Dùng
- **Kiểm thử kịch bản Submit Form Rỗng / Dữ liệu sai:**
  1. Mở modal Thêm mới $\rightarrow$ Bấm nút "Lưu" ngay lập tức mà không điền thông tin bắt buộc.
  2. **Kết quả mong đợi:**
     - Modal **KHÔNG ĐƯỢC ĐÓNG**.
     - Trang **KHÔNG ĐƯỢC F5 / RELOAD TOÀN BỘ**.
     - Các dòng chữ báo lỗi màu đỏ (`@Html.ValidationMessageFor`) xuất hiện ngay bên dưới từng ô nhập liệu bị thiếu.
     - Các ô nhập văn bản khác nếu đã gõ một phần **KHÔNG BỊ MẤT TRẮNG DỮ LIỆU**.
- **Kiểm thử kịch bản Submit Thành Công:**
  1. Điền thông tin chuẩn $\rightarrow$ Bấm "Lưu".
  2. Nếu ô `#chkNotDismissModal` (Thêm tiếp) được chọn $\rightarrow$ Hiện Toast xanh, DataTable tự reload dữ liệu mới, form tự xóa trắng để nhập tiếp.
  3. Nếu không chọn `#chkNotDismissModal` $\rightarrow$ Modal đóng nhẹ nhàng, Toast xanh hiện lên, DataTable reload đúng vị trí trang hiện tại.

### 5. Trục 5: Responsive Bảng Dữ Liệu & Phân Cấp Trực Quan
- **Độ co giãn DataTable:**
  - Bọc bảng trong `<div class="table-responsive-md">`.
  - Khi thu nhỏ cửa sổ trình duyệt về kích thước tablet/laptop nhỏ, thanh cuộn ngang chỉ xuất hiện bên trong khung bảng dữ liệu, **không làm xuất hiện thanh cuộn ngang toàn trang web** (tránh vỡ Header/Sidebar).
- **Cắt tỉa văn bản dài (`.toggle-more`):**
  - Các cột ghi chú hoặc danh sách dịch vụ dài phải được hiển thị rút gọn kèm nút "Xem thêm" / "Ẩn bớt", không để chữ tràn vô tận làm vỡ chiều cao hàng.

---

## 🚫 Bảng Chống Bao Biện Khi Test UI/UX (Anti-Rationalization Table)

| Bao biện của AI / Tester thiếu kỷ luật | Hậu quả thực tế | Quy tắc bắt buộc thi hành |
| :--- | :--- | :--- |
| *"Tôi đọc qua code thấy viết đúng cú pháp rồi nên không cần kiểm tra trình duyệt."* | Code đúng cú pháp nhưng runtime vẫn có thể vỡ giao diện do xung đột CSS, thiếu script bundle, hoặc sai selector jQuery. | **BẮT BUỘC kiểm chứng trực quan**: Mở trình duyệt/preview, soi selector, kiểm tra DOM thật. |
| *"Lỗi 404 của file JS/CSS phụ này không ảnh hưởng đến chức năng chính."* | Trình duyệt mất thời gian timeout chờ tải file, giao diện bị giật, console đỏ lòe làm giảm sút uy tín dự án. | **BẮT BUỘC sửa sạch 100% lỗi 404 static assets** trong Network tab. |
| *"Lỗi kẹt lớp mờ Modal chỉ xảy ra khi bấm nút quá nhanh, người dùng bình thường không bị."* | Người dùng bị đơ toàn bộ màn hình, tưởng hệ thống bị treo, bắt buộc phải F5 và mất toàn bộ ngữ cảnh làm việc. | **BẮT BUỘC bọc code đóng modal & reload trong sự kiện `hidden.bs.modal`** để dọn dẹp sạch sẽ backdrop. |
| *"Validation trả về trang lỗi màu vàng của ASP.NET cũng biết là sai rồi."* | Đây là lỗi nghiêm trọng (Unhandled Exception / YSOD), làm lộ cấu trúc hệ thống và mang lại trải nghiệm tồi tệ. | **BẮT BUỘC Controller trả về PartialView ruột** và JavaScript thế vào `#bodyForm` khi `!ModelState.IsValid`. |
| *"Chỉ cần test trên màn hình full HD 1920x1080 là đủ."* | Khách hàng và nhân viên thường dùng laptop 1366x768 hoặc 14 inch, giao diện sẽ bị tràn viền và vỡ nút bấm nếu không kiểm tra responsive. | **BẮT BUỘC kiểm tra co giãn giao diện ở độ phân giải 1366x768 và tablet**. |

---

## 📋 Quy Trình Thực Hiện Test UI/UX 4 Bước Chuẩn

1. **Bước 1: Smoke Test & Console Audit**
   - Mở màn hình `Index.cshtml`.
   - Bật Console DevTools $\rightarrow$ Kiểm tra không có bất kỳ log đỏ nào.
   - Kiểm tra DataTable hiển thị dữ liệu phân trang và số thứ tự chuẩn.
2. **Bước 2: Form & Modal Boundary Test**
   - Bấm nút Thêm mới $\rightarrow$ Đo kích thước modal có đúng `data_width` không.
   - Bấm nút Lưu mà không điền thông tin $\rightarrow$ Kiểm tra hiển thị lỗi đỏ bên dưới từng trường.
   - Điền dữ liệu chuẩn $\rightarrow$ Lưu thành công $\rightarrow$ Kiểm tra Toast xanh và bảng DataTable cập nhật dòng mới.
3. **Bước 3: Modal Cleanup Verification**
   - Xác nhận lớp backdrop mờ đen biến mất 100%.
   - Thử cuộn trang và tương tác với các nút khác trên trang chính.
4. **Bước 4: Layout & Responsive Check**
   - Thu nhỏ cửa sổ về bề rộng 1024px và 768px.
   - Kiểm tra các nút bấm hành động, thanh tìm kiếm, và bảng dữ liệu vẫn hiển thị ngay ngắn, không tràn vỡ giao diện.
