---
name: upcode-demo
description: Quy trình tự động biên dịch, đóng gói WebApp sang thư mục publish_source, merge code sang nhánh upcode-demo và đẩy lên GitHub để kích hoạt workflow deploy lên FTP Demo sử dụng các secret FTP_SERVER_DEMO, FTP_USERNAME_DEMO, FTP_PASSWORD_DEMO. TUÂN THỦ LOẠI TRỪ (IGNORE) HOÀN TOÀN CÁC THƯ MỤC BIN VÀ OBJ TRUNG GIAN. LUÔN kích hoạt skill này khi người dùng gõ hoặc yêu cầu "upcode demo", "upload code demo", "đẩy code demo", "up code demo", "deploy demo", hoặc bất kỳ câu lệnh nào ngụ ý muốn đưa bản dựng demo lên môi trường demo.
---

# Upcode Demo Workflow

Khi được kích hoạt bởi các lệnh như `upcode demo`, `upload code demo`, `đẩy code demo`, thực hiện tuần tự các bước dưới đây bằng terminal PowerShell / Git Bash:

---

## 🚫 QUY TẮC QUAN TRỌNG: IGNORE THƯ MỤC BIN & OBJ
- **TUYỆT ĐỐI KHÔNG** commit hay push các thư mục `bin/` và `obj/` trung gian của các project con (`Modules.*`, `CenIT.*`, `Jobs.*`, `Core.*`) lên GitHub.
- **CHỈ DUY NHẤT** thư mục gói phát hành `publish_source/` (chứa các file đã đóng gói cần thiết để deploy FTP) và thư mục `dlls/` (thư viện bên ngoài) được phép đưa lên.
- Tệp `.gitignore` ở thư mục gốc luôn phải duy trì các quy tắc sau:
  ```gitignore
  [Bb]in/
  [Oo]bj/
  .vs/
  *.user
  *.suo
  !publish_source/
  !publish_source/**
  !dlls/
  !dlls/**
  ```

---

## Quy trình thực hiện chi tiết

### Bước 0 — Kiểm tra nhánh và trạng thái Git
```powershell
git status
git branch --show-current
```
- Ghi nhớ tên nhánh hiện tại (gọi là `<current>`). Nhánh này sẽ dùng để quay về sau khi hoàn tất quy trình.
- Đảm bảo các thư mục `bin/` và `obj/` trung gian không bị theo dõi trong Git index.

---

### Bước 1 — Biên dịch và xuất bản sang thư mục `publish_source`
Chạy script tự động hóa xuất bản đã được tối ưu hóa cho Solution:
```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\build_publish.ps1
```
*Script này thực hiện:*
1. Tự động định vị công cụ **MSBuild** mới nhất trên máy trạm.
2. Biên dịch toàn bộ Solution `CenIT.Solution.TOC.sln` ở cấu hình `Release`.
3. Đóng gói ứng dụng `CenIT.Solution.TOC.WebApp` và đồng bộ (Robocopy Mirror) toàn bộ thư mục `PackageTmp` sang `publish_source/`.

*Kiểm tra:* Thư mục `publish_source/` phải có đầy đủ các thư mục/tệp: `App_Data`, `Areas`, `bin`, `Configs`, `Contents`, `Global.asax`, `Libraries`, `Views`, `Web.config`.

---

### Bước 2 — Commit các thay đổi trên nhánh làm việc hiện tại
Kiểm tra và commit mã nguồn cùng với thư mục `publish_source/` (bỏ qua mọi `bin/`, `obj/` nhờ `.gitignore`):
```powershell
git add .gitignore .agents/ .github/ scripts/ publish_source/
git add -u
git commit -m "build: update publish_source for demo release"
git push origin <current>
```
*(Nếu working tree sạch và không có thay đổi mới trên `<current>`, bỏ qua commit này).*

---

### Bước 3 — Merge sang nhánh `upcode-demo` và Push lên GitHub
```powershell
git checkout upcode-demo
git pull origin upcode-demo
git merge <current> -m "chore: merge <current> into upcode-demo for deployment"
git push origin upcode-demo
```
*Xử lý nếu nhánh `upcode-demo` chưa có trên remote:*
```powershell
git push -u origin upcode-demo
```

---

### Bước 4 — Kích hoạt GitHub Action Deploy lên FTP Demo
Khi nhánh `upcode-demo` được push lên GitHub:
1. Workflow `.github/workflows/deploy-demo.yml` sẽ tự động kích hoạt.
2. Action sử dụng 3 Secret trên GitHub Repository để kết nối FTP:
   - `FTP_SERVER_DEMO`: Địa chỉ IP máy chủ FTP Demo (ví dụ `10.57.30.10`)
   - `FTP_USERNAME_DEMO`: Tài khoản FTP Demo (ví dụ `quanlydoanhthucenit`)
   - `FTP_PASSWORD_DEMO`: Mật khẩu FTP Demo
3. Tải toàn bộ nội dung từ `./publish_source/` lên thư mục gốc của FTP Demo.

*(Tùy chọn dự phòng: Nếu máy chủ FTP Demo là IP nội bộ không mở NAT ra ngoài internet cho GitHub runner công cộng, có thể chạy lệnh upload trực tiếp từ máy trạm nội bộ qua script `scripts\deploy_ftp_demo.ps1`).*

---

### Bước 5 — Quay về nhánh ban đầu và Báo cáo
```powershell
git checkout <current>
```

Báo cáo kết quả rõ ràng, ngắn gọn cho người dùng:
1. **Loại trừ thư mục**: Đã loại trừ hoàn toàn `bin/` và `obj/` trung gian khỏi Git.
2. **Build & Publish**: Trạng thái biên dịch và số lượng tệp/thư mục trong `publish_source`.
3. **Git Merge & Push**: Nhánh `<current>` đã được merge vào `upcode-demo` và push thành công lên GitHub.
4. **Deploy Status**: GitHub Action đã nhận được trigger để đồng bộ lên máy chủ FTP Demo qua 3 secret `FTP_SERVER_DEMO`, `FTP_USERNAME_DEMO`, `FTP_PASSWORD_DEMO`.

---

## Xử lý sự cố (Troubleshooting)

- **Xung đột Merge (Conflict)**: DỪNG LẠI NGAY LẬP TỨC. Chạy `git merge --abort`, liệt kê danh sách file xung đột (`git diff --name-only --diff-filter=U`) và báo người dùng. Tuyệt đối không tự ý resolve conflict khi chưa có chỉ dẫn.
- **Lỗi MSBuild**: Nếu thiếu Reference Assemblies của các framework cũ, kiểm tra xem tất cả các project trong Solution đã target đồng bộ `v4.8` chưa.
- **Nhánh chưa tồn tại**: Nếu nhánh `upcode-demo` chưa có, tạo từ nhánh `main` bằng `git checkout -b upcode-demo main`.
