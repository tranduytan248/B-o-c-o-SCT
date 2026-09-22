---
name: version-prod
description: Tự động đóng gói và tạo version phát hành Production theo mẫu "build <tên version>" (ví dụ: "build v1.0.0", "build 2026.09.08"). Tự động biên dịch Release ra thư mục publish_source, so sánh diff giữa publish_source và Source_Prod để lọc ra các file chưa có (NEW) và các file có version cập nhật (MODIFIED), rồi copy toàn bộ vào thư mục version/<tên version>/ kèm file ghi chú UPDATE_NOTES.md. TUÂN THỦ LOẠI TRỪ (IGNORE) HOÀN TOÀN CÁC THƯ MỤC BIN VÀ OBJ TRUNG GIAN. LUÔN kích hoạt skill này khi người dùng gõ lệnh theo mẫu "build tên version" (ví dụ: build v1.0, build release-1.0, build 2026.09.08).
---

# Version Prod Workflow (build <tên version>)

Khi người dùng nhập câu lệnh theo mẫu **`build <tên version>`** (ví dụ: `build v1.0.0`, `build 2026.09.08`, `build prod-patch-1`), agent PHẢI kích hoạt skill này và thực thi quy trình tuần tự dưới đây.

---

## 🚫 QUY TẮC QUAN TRỌNG: IGNORE THƯ MỤC BIN & OBJ
1. **TUYỆT ĐỐI KHÔNG** commit hay push bất kỳ thư mục `bin/` và `obj/` trung gian nào của các project con (`Modules.*`, `CenIT.*`, `Jobs.*`, `Core.*`) lên GitHub.
2. **CHỈ DUY NHẤT** các thư mục phát hành sau được phép giữ lại:
   - `publish_source/` (gói xuất bản đầy đủ của bản build mới nhất)
   - `Source_Prod/` (mã nguồn Production hiện hành dùng để đối chiếu diff)
   - `version/` và `version/**` (các gói cập nhật version theo từng phiên bản)
   - `dlls/` (thư viện ngoài)
3. Tệp `.gitignore` ở thư mục gốc luôn phải duy trì các quy tắc sau:
   ```gitignore
   [Bb]in/
   [Oo]bj/
   .vs/
   *.user
   *.suo
   !publish_source/
   !publish_source/**
   !Source_Prod/
   !Source_Prod/**
   !version/
   !version/**
   !dlls/
   !dlls/**
   ```

---

## 🔍 CƠ CHẾ SO SÁNH (DIFF): `publish_source` vs `Source_Prod`
- **Nguồn mới (Bản build vừa biên dịch)**: Nằm tại thư mục `publish_source/`.
- **Nguồn chuẩn Production hiện hành**: Nằm tại thư mục `Source_Prod/`.
- **Đầu ra phát hành**: Được đóng gói tại `version/<tên version>/`.
- **Nguyên tắc chọn lọc**:
  - **File NEW (Chưa có)**: Xuất hiện trong `publish_source/` nhưng **hoàn toàn chưa có** trong `Source_Prod/`.
  - **File MODIFIED (Cập nhật)**: Đã có trong `Source_Prod/` nhưng có sự thay đổi về kích thước (File Length) hoặc mã băm nội dung (MD5 Hash).
  - **File UNCHANGED (Không đổi)**: Giống hệt cả về kích thước và nội dung giữa hai nguồn -> **Bỏ qua**, không đưa vào gói cập nhật.

---

## QUY TRÌNH THỰC HIỆN CHI TIẾT

### Bước 1 — Bóc tách Tên Version từ Câu lệnh
- Từ câu lệnh của người dùng (ví dụ: `build v1.0.0`, `build 2026.09.08`), trích xuất chuỗi `<tên version>`:
  - Nếu user nhập `build v1.0.0` -> `<tên version>` = `v1.0.0`
  - Nếu user nhập `build 2026.09.08` -> `<tên version>` = `2026.09.08`
- Loại bỏ các ký tự đặc biệt nguy hiểm cho hệ điều hành, đảm bảo tên thư mục hợp lệ.

---

### Bước 2 — Kiểm tra Trạng thái Git
Kiểm tra nhanh nhánh hiện tại và đảm bảo không có file `bin`/`obj` trung gian bị theo dõi:
```powershell
git status
```

---

### Bước 3 — Thực thi Script Tạo Version Production
Chạy script PowerShell tự động hóa toàn bộ quy trình:
```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\build_version_prod.ps1 -VersionName "<tên version>"
```
*(Nếu muốn kèm ghi chú tính năng cụ thể: thêm cờ `-Notes "Mô tả nội dung cập nhật"`)*
*(Nếu vừa build xong và muốn so sánh nhanh: thêm cờ `-SkipBuild`)*

*Script này tự động thực hiện 5 tác vụ:*
1. **Biên dịch Release**: Sử dụng MSBuild để build toàn bộ Solution `CenIT.Solution.TOC.sln`, đóng gói Package WebApp và đồng bộ sang `publish_source/`.
2. **Quét tệp 2 nguồn**: Quét đệ quy toàn bộ danh sách tệp trong `publish_source/` và `Source_Prod/`.
3. **So sánh (Diff) logic chuẩn xác**:
   - Tách biệt rõ ràng danh sách file **chưa có (NEW)** và danh sách file **cập nhật (MODIFIED)**.
4. **Sao chép vào `version/<tên version>/`**:
   - Toàn bộ file NEW và MODIFIED được sao chép vào `version/<tên version>/` (giữ nguyên cấu trúc thư mục tương đối như `bin/`, `Views/`, `Contents/`, `Configs/`,...).
5. **Tự động xuất tệp Ghi Chú & Báo Cáo**:
   - **`UPDATE_NOTES.md`**: Bảng chi tiết danh sách chính xác file nào được cập nhật (MODIFIED), file nào thêm mới (NEW), lịch sử commit Git gần nhất và hướng dẫn triển khai.
   - **`manifest.txt`** & **`manifest.json`**: Thống kê số lượng và danh sách toàn bộ các file.

---

### Bước 4 — Kiểm tra Thư mục `version/<tên version>`
Kiểm tra danh sách tệp và nội dung báo cáo:
```powershell
Get-ChildItem -Path "version\<tên version>"
Get-Content -Path "version\<tên version>\UPDATE_NOTES.md"
Get-Content -Path "version\<tên version>\manifest.txt"
```

---

### Bước 5 — Báo cáo Kết quả cho Người dùng
Xuất báo cáo tổng kết rõ ràng bao gồm:
1. **Tên version**: `<tên version>`
2. **Thư mục lưu trữ**: `version/<tên version>/`
3. **Chế độ so sánh**: `publish_source` vs `Source_Prod`
4. **Số lượng file mới (NEW)** và **Số lượng file cập nhật (MODIFIED)**
5. **Tệp Ghi chú Cập nhật**: `version/<tên version>/UPDATE_NOTES.md` (chứa danh sách chi tiết các file cập nhật và nội dung cập nhật)
6. **Xác nhận**: Đã tuân thủ loại trừ hoàn toàn các thư mục `bin/` và `obj/` trung gian của mã nguồn.
