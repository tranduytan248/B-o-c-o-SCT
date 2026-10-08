# Tích hợp Chatbot cho Sở Công Thương

Tên namespace `ReportTourism` là tên kỹ thuật cũ. Các công cụ dưới đây phục vụ doanh nghiệp, báo cáo sản xuất công nghiệp, thương mại và xuất nhập khẩu; không cung cấp công cụ du lịch hay CAPA.

## Cấu hình ứng dụng

**Trước khi bật chatbot**, triển khai 2 hàm phạm vi và 9 SP Chatbot theo [sp-deployment.md](sp-deployment.md). Các SP Chatbot nhận danh tính từ máy chủ và lọc dữ liệu theo tài khoản; không dùng trực tiếp SP dashboard toàn tỉnh. Xem [security-scope.md](security-scope.md) cho ma trận quyền và kiểm tra IDOR.

Đã đặt các giá trị công khai trong `CenIT.ReportTourism.WebApp/Configs/AppSettings.config`:

- `Chatbot:BaseUrl`: `https://api.chatbot.cenit.vn`
- `Chatbot:BotId`: `cmuwhm1eh0005ltjxdirj27wr`
- `Chatbot:WidgetLoaderUrl`: `https://api.chatbot.cenit.vn/public/bots/cmuwhm1eh0005ltjxdirj27wr/widget-loader.js`
- `Chatbot:Enabled`: `false` cho đến khi cấu hình hoàn tất.
- `Chatbot:ProvinceRoleIds`: `1,6`, đối chiếu role thực tế; cần thêm Dashboard/View và không có phân công riêng/role doanh nghiệp.
- `Chatbot:EnterpriseRoleIds`: `5`. Cán bộ quản lý không nằm trong allowlist toàn tỉnh chỉ đọc doanh nghiệp được phân công. Hai danh sách không được giao nhau.

Đặt integration API key trên máy chủ IIS bằng biến môi trường **`CHATBOT_APIKEY`**, rồi đặt `CHATBOT_ENABLED=true` (hoặc đổi `Chatbot:Enabled` thành `true` trong cấu hình triển khai). Recycle application pool sau khi thay đổi biến môi trường. Không đưa API key vào Git, HTML hay JavaScript. Các biến `CHATBOT_BASEURL`, `CHATBOT_BOTID`, `CHATBOT_WIDGETLOADERURL` có thể ghi đè các URL/ID trên.

Capability sử dụng ASP.NET `MachineKey.Protect/Unprotect` với purpose riêng `SCT.Chatbot.Tools`, `v1`; không cần thêm thư viện JWT hoặc secret ký token mới. Khi chạy nhiều IIS node, cấu hình cùng machine key qua cấu hình triển khai bí mật; không commit key. Capability không phải cookie đăng nhập và chỉ executor này đọc được.

Triển khai qua HTTPS. Các URL Chatbot chỉ chấp nhận HTTPS. Không đổi cơ chế đăng nhập hoặc pipeline Web API hiện hữu: controller tích hợp dùng MVC và các route mặc định của ứng dụng.

## Cấu hình trên Chatbot site

Trong bot `cmuwhm1eh0005ltjxdirj27wr`, đặt:

- `viewerSessionEndpoint`: `https://<application-host>/<virtual-directory-if-any>/Chatbot/Session`
- `executorUrl`: `https://<application-host>/<virtual-directory-if-any>/Chatbot/Tools`

Hai URL phải cùng origin. Bật chế độ yêu cầu viewer session/authenticated viewer của bot; không dùng widget công khai để truy cập dữ liệu ứng dụng. Ứng dụng tự tạo viewer session trước khi nạp loader, để gửi CSRF token theo cơ chế của MVC. Không nhúng loader trực tiếp vào trang login.

Catalog đầy đủ để copy từng `name`, `description`, `inputSchema` vào External Tools:

`Source/ReportDeptTourismSolution/CenIT.ReportTourism.WebApp/Chatbot/external-tools.json`

File catalog này cũng là nguồn kiểm tra input ở executor. Tất cả field input đều optional; unknown fields bị từ chối. Các tool xử lý trường hợp thiếu thông tin bằng mặc định an toàn hoặc trả `needsClarification` và danh sách ứng viên. Không tự chọn doanh nghiệp khi tên khớp nhiều kết quả.

Nếu đăng ký qua API thay vì nhập thủ công, gửi `PUT https://api.chatbot.cenit.vn/public/bots/cmuwhm1eh0005ltjxdirj27wr/external-tools` với `x-api-key` từ máy chủ, body là object gồm `executorUrl` thật và mảng `tools` trong catalog. Endpoint này thay catalog của bot: kiểm tra tool đã có trước khi đồng bộ. Sau khi thêm/đổi schema hoặc mô tả, kiểm tra và bật các tool trên Portal.

## Danh sách công cụ và câu hỏi tự nhiên

Catalog đã mở rộng thành **23 công cụ** theo 8 yêu cầu nghiệp vụ. Xem [business-tools.md](business-tools.md) để biết các tool mới, mapping SP, drill-down, công thức và giới hạn nguồn GTSXCN. Xem thêm [sp-deployment.md](sp-deployment.md) cho danh sách REUSE/CREATE/ALTER đã đối chiếu database ngày 07/10/2026. Các đoạn tổng quan phía dưới mô tả nhóm tool ban đầu.

| Tool | Phạm vi | Ví dụ người dùng |
| --- | --- | --- |
| `search_enterprises` | Doanh nghiệp được phân công | “Tìm doanh nghiệp sản xuất có tên Khánh Hòa” |
| `get_enterprise_detail` | Doanh nghiệp được phân công | “Thông tin doanh nghiệp có mã số thuế …” |
| `search_reports` | Doanh nghiệp được phân công + quyền Report/Import/View | “Báo cáo doanh nghiệp này trong năm nay” |
| `get_report_detail` | Doanh nghiệp được phân công + quyền Report/Import/View | “Doanh thu tháng 9 của công ty này bao nhiêu?” |
| `get_my_reporting_status` | Doanh nghiệp được phân công + quyền Report/Import/View | “Doanh nghiệp nào của tôi chưa nộp báo cáo tháng trước?” |
| `get_commerce_statistics` | Quyền Report/Dashboard/View, phạm vi dữ liệu được cấp cho tài khoản | “Tổng quan tình hình công thương tháng trước” |
| `analyze_commerce_indicator` | Quyền Report/Dashboard/View, phạm vi dữ liệu được cấp cho tài khoản | “Xuất khẩu tháng 9 tăng hay giảm so với tháng trước?” |
| `get_reporting_warnings` | Quyền Report/Dashboard/View, phạm vi dữ liệu được cấp cho tài khoản | “Doanh nghiệp sản xuất nào giảm doanh thu trên 20%?” |
| `get_reporting_progress` | Quyền Report/Dashboard/View, phạm vi dữ liệu được cấp cho tài khoản | “Tiến độ báo cáo thương mại tại phường … thế nào?” |

Chatbot/LLM chuyển câu hỏi thành input có cấu trúc; ứng dụng không tự parse câu văn hay chạy SQL do model tạo. Tên, địa chỉ, mã số thuế và bộ lọc tên dashboard hỗ trợ tìm một phần và không phân biệt dấu/hoa thường. `search_reports.keyword` sử dụng bộ tìm kiếm báo cáo hiện hữu, không bảo đảm cùng ngữ nghĩa tìm không dấu.

Tháng dùng `yyyy-MM`, ưu tiên hơn `timeframe`. Mặc định `latest`: tháng có dữ liệu chỉ tiêu gần nhất **trong phạm vi tài khoản**, tính theo UTC+7; nếu chưa có chỉ tiêu, dùng tháng hiện tại. Tài khoản doanh nghiệp không có Dashboard/View dùng tháng hiện tại để tránh tiết lộ kỳ báo cáo của tài khoản khác. Tệp báo cáo không quyết định tháng kinh tế mặc định. `search_reports` hỗ trợ `this_month`, `last_month`, `this_year`, `last_year`, `recent` (3 tháng gồm tháng hiện tại). Các tool dashboard và đọc chi tiết dùng một tháng: `latest`, `this_month` hoặc `last_month`. Công cụ query_report_indicators/get_report_submission_history có thể đọc khoảng tối đa 12 tháng. Không nhận tháng tương lai. Mặc định lấy 10 kết quả, tối đa 50; chi tiết báo cáo có `offset`, `nextOffset`, `hasMore` để đọc tiếp.

Ví dụ:

```json
{"toolName":"search_enterprises","input":{"keyword":"Khanh Hoa","businessType":"manufacturing","limit":5}}
```

```json
{"toolName":"analyze_commerce_indicator","input":{"month":"2026-09","reportType":"manufacturing","metric":"secondary"}}
```

```json
{"toolName":"get_reporting_progress","input":{"timeframe":"last_month","reportType":"trading","area":"Vĩnh Hải","limit":20}}
```

Phạm vi dashboard được tính lại từ user/role/quyền/phân công trong DB ở mỗi lần gọi. Doanh nghiệp chỉ đọc phân công rõ ràng hoặc mã số thuế khớp username khi chưa có phân công; không suy ra quyền từ email, tên hoặc người tạo. Tài khoản có phân công riêng luôn bị giới hạn theo phân công, kể cả admin. Toàn tỉnh chỉ dành cho role được máy chủ cho phép, có Dashboard/View, không có role doanh nghiệp và không có phân công riêng. ID không thuộc phạm vi bị từ chối; bộ lọc, tổng số, kỳ gần nhất, metadata và drill-down cũng dùng cùng phạm vi. Không dùng cache dashboard/báo cáo dùng chung cho Chatbot.

`get_my_reporting_status` kiểm tra đã/chưa nộp, không suy diễn quá hạn. Cảnh báo dashboard là biến động chỉ tiêu/xung đột số liệu, không phải thông báo chậm nộp. `0101` là **doanh thu công nghiệp**, không phải GTSXCN. Số liệu/đơn vị/so sánh lấy từ domain service hiện hữu; không biến giá trị thiếu thành 0 hoặc tự tạo số liệu lịch sử. Summary registry trong phạm vi tài khoản được tách khỏi số liệu theo bộ lọc. Công cụ analytics mới dùng TypeNExpected từ SP Summary cho độ phủ và công bố riêng số registry được phép xem.

## API contract

### `POST /Chatbot/Session`

Không có body. Browser gửi cookie đăng nhập và header `X-CSRF-Token` lấy từ `@Html.AntiForgeryToken()` của trang hiện tại. Server giải mã và kiểm tra hết hạn Forms Authentication ticket, lấy ID thật từ user database, kiểm tra user active. Không nhận danh tính từ browser.

Server gọi URL viewer-session đã cung cấp với `x-api-key`, `subjectId` là ID user dạng chuỗi và `toolCapability: {token, expiresAt}`. API trả:

```json
{"viewerToken":"<viewer-token>","expiresAt":"2026-10-06T10:30:00Z"}
```

`401`: chưa đăng nhập/cookie hết hạn/user không active; `400`: CSRF sai; `503`: tắt/chưa cấu hình; `502`: Chatbot lỗi, timeout hoặc response sai. API dùng JSON, không redirect sang login. Token không được cache.

### `POST /Chatbot/Tools`

Chatbot BE gửi:

```http
Authorization: Bearer <toolCapability.token>
Idempotency-Key: <toolCallId>
Content-Type: application/json
```

```json
{
  "version": 1,
  "botId": "cmuwhm1eh0005ltjxdirj27wr",
  "subjectId": "123",
  "toolCallId": "call-unique-id",
  "toolName": "get_my_reporting_status",
  "input": {"timeframe":"last_month","status":"missing"}
}
```

Response luôn là JSON `{"result": ...}` khi thành công. Input phải là object; dùng `{}` khi không có bộ lọc. Backend kiểm tra version, capability hết hạn, bot/subject khớp, tool allowlist, header khớp toolCallId, input type/enum/giới hạn và quyền hiện tại. Không đưa password, salt, access token, file upload, file content hoặc model doanh nghiệp đầy đủ vào kết quả.

`400`: input/contract sai; `401`: capability sai/hết hạn/subject không khớp/user không active; `403`: bot/tool/quyền/phạm vi sai; `413`: request >16 KB hoặc kết quả >256 KB; `415`: không phải JSON; `504`: query quá chậm. Query nghiệp vụ có thời hạn chờ 12 giây để nằm trong thời hạn executor 15 giây. Stored procedure hiện tại đồng bộ và không hỗ trợ cancellation: query đã bắt đầu có thể chạy tiếp trong backend sau timeout; tất cả tool chỉ đọc và không có side effect ghi dữ liệu. Cần đo thời gian trên IIS/database thực tế.

Capability sống tối đa 15 phút và không vượt quá hạn cookie khi cấp. Executor đọc lại user/assignment/permission hiện tại. Widget được hủy khi logout (kể cả thông báo logout giữa các tab), pagehide hoặc hết phiên; khi hết phiên nó xin token mới bằng cookie rồi khởi tạo lại. Capability đã cấp không bị thu hồi lập tức ở server khi logout; nó hết hạn theo thời gian trên. Đây là mô hình capability ngắn hạn của contract, không phải session thu hồi tập trung. Nếu cần thu hồi tức thời cho token đã phát hành, cần thêm kho session/revocation dùng chung.

## Kiểm tra triển khai

Đã kiểm tra schema/SP và query bodies trên database cấu hình hiện tại; xem business-tools.md và database-audit.json. Chưa gọi upstream có xác thực vì chưa được cấp API key và chưa có IIS chạy ứng dụng trong môi trường này. Không thêm test case tự động theo hướng dẫn dự án. Cần xác minh trên môi trường triển khai:

1. Login sai/trang công khai không có widget; Session chưa login trả 401; CSRF sai trả 400.
2. Login thành công: Session trả viewer token, loader mở chatbot; không có API key trong response/DOM.
3. Gọi tool với đúng capability; thử subject khác, bot khác, hết hạn, tên tool lạ, input lạ và Idempotency-Key sai.
4. User thường không xem dashboard nếu thiếu quyền; doanh nghiệp ngoài phân công bị từ chối; user bị disable không gọi tool được.
5. Bộ lọc tên nhiều kết quả trả needsClarification; tháng năm và chỉ tiêu đọc đúng dữ liệu; không có số liệu vẫn trả missing/null theo domain.
6. Logout/đổi tab/phiên hết hạn hủy widget; API/upstream timeout và response lớn được xử lý.
7. Các tool hiện tại chỉ đọc nên gọi lại cùng toolCallId không ghi trùng. Trước khi thêm tool ghi, cần cơ chế idempotency bền vững trong database.

Build đầy đủ yêu cầu Windows/.NET Framework 4.8, Visual Studio MSBuild Web Application targets và restore NuGet của solution. .NET 6 SDK trên macOS không có các targets đó.
