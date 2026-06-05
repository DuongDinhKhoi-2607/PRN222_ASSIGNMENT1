# PRN222_ASSIGNMENT1

Dự án này là một ứng dụng ASP.NET Core sử dụng Entity Framework Core và tích hợp Gemini API. Dưới đây là hướng dẫn chi tiết cách thiết lập, cấu hình database và lấy API Key của Gemini để chạy ứng dụng.

## 1. Yêu cầu hệ thống (Prerequisites)
- **.NET 8.0 SDK** (hoặc phiên bản tương thích với project).
- **SQL Server** (LocalDB hoặc SQL Server độc lập).
- **Visual Studio 2022** hoặc **Visual Studio Code**.

## 2. Cấu hình và Chạy Database
Dự án sử dụng Entity Framework Core (EF Core) với phương pháp Code-First. Các file Migration đã được tạo sẵn trong thư mục `DataAccessPlayer/Migrations`.

**Bước 1: Cấu hình chuỗi kết nối (Connection String)**
1. Mở file `PresentationPlayer/appsettings.json`.
2. Tìm đến phần `"ConnectionStrings"`.
3. Thay đổi `"DefaultConnection"` sao cho phù hợp với cấu hình SQL Server trên máy bạn. 
   - Ví dụ hiện tại: `"Server=(local);Database=RAGChatbotDB;User Id=sa;Password=12345;TrustServerCertificate=True"`
   - Nếu bạn dùng Windows Authentication, có thể đổi thành: `"Server=YOUR_SERVER_NAME;Database=RAGChatbotDB;Trusted_Connection=True;TrustServerCertificate=True"`

**Bước 2: Cập nhật Database (Chạy Migration)**
- **Cách 1: Sử dụng Visual Studio (Package Manager Console)**
  1. Chọn `PresentationPlayer` làm **Startup Project** (Click chuột phải vào project -> *Set as Startup Project*).
  2. Mở **Package Manager Console** (`Tools` > `NuGet Package Manager` > `Package Manager Console`).
  3. Chọn `Default project` là **DataAccessPlayer**.
  4. Chạy lệnh:
     ```powershell
     Update-Database
     ```
- **Cách 2: Sử dụng Terminal / Command Prompt (dotnet CLI)**
  Mở terminal tại thư mục gốc của dự án (`Assignment1_Group7`) và chạy lệnh:
  ```bash
  dotnet ef database update --project DataAccessPlayer --startup-project PresentationPlayer
  ```

## 3. Hướng dẫn lấy và cấu hình Gemini API Key
Dự án sử dụng AI Gemini của Google. Bạn cần có API Key để chức năng này hoạt động.

**Bước 1: Lấy API Key**
1. Truy cập vào **[Google AI Studio](https://aistudio.google.com/)**.
2. Đăng nhập bằng tài khoản Google của bạn.
3. Ở menu bên trái, chọn **Get API key**.
4. Bấm vào nút **Create API key** (có thể chọn tạo key ở một project Google Cloud mới hoặc có sẵn).
5. Copy đoạn mã API key vừa được tạo.

**Bước 2: Cấu hình vào dự án**
1. Mở file `PresentationPlayer/appsettings.json`.
2. Tìm đến mục `"Gemini"`.
3. Dán API Key của bạn vào thuộc tính `"ApiKey"`.
   ```json
   "Gemini": {
     "ApiKey": "DÁN_API_KEY_CỦA_BẠN_VÀO_ĐÂY"
   }
   ```
*(Lưu ý: Không nên commit API Key thật của bạn lên GitHub để bảo mật).*

## 4. Cách chạy ứng dụng
1. Mở Solution (`.sln` hoặc `.slnx`) bằng Visual Studio.
2. Đảm bảo project **PresentationPlayer** được chọn làm **Startup Project** (in đậm trong Solution Explorer).
3. Nhấn **F5** hoặc **Ctrl + F5** (Run without Debugging) để khởi chạy ứng dụng.
4. Trình duyệt sẽ tự động mở trang web của dự án.

---
**Lưu ý bổ sung:**
Nếu bạn gặp lỗi liên quan đến các package (NuGet), hãy chuột phải vào Solution và chọn **Restore NuGet Packages** hoặc chạy lệnh `dotnet restore` trước khi build dự án.
