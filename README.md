# 🥗 EcoClean — Healthy Eat Clean Management System

## Tech Stack
- **Backend**: ASP.NET Core 8 Web API + EF Core + SQL Server
- **Frontend**: ASP.NET Core MVC + Razor Views + Bootstrap 5 + Chart.js
- **AI**: Gemini 1.5 Flash (food scan, chatbot, meal plan)
- **Payment**: VietQR Banking (free API)
- **Deploy**: Docker Compose + Nginx

---

## Cấu trúc dự án
```
EcoClean/
├── EcoClean.API/          ← Web API (port 5000)
│   ├── Controllers/       ← 9 controllers
│   ├── Services/          ← Business logic + AI
│   ├── Models/            ← EF Core entities
│   ├── DTOs/              ← Request/Response DTOs
│   ├── Helpers/           ← BMI, TDEE, JWT
│   ├── Data/              ← DbContext + Migrations
│   └── Middleware/        ← Exception handler
├── EcoClean.MVC/          ← MVC Frontend (port 5001)
│   ├── Controllers/       ← 7 controllers
│   ├── Views/             ← Razor views (12 pages)
│   ├── Services/          ← ApiClient wrapper
│   └── wwwroot/           ← CSS/JS/Images
├── docker-compose.yml
├── nginx.conf
└── .github/workflows/deploy.yml
```

---

## Chạy nhanh (Development)

### Yêu cầu
- .NET 8 SDK
- SQL Server 2022 hoặc Docker
- Gemini API key (https://aistudio.google.com/app/apikey)

### Bước 1 — Cấu hình API
```bash
cd EcoClean.API
# Sửa appsettings.json
#   ConnectionStrings:DefaultConnection  ← SQL Server connection string
#   GeminiApiKey                         ← Gemini API key
#   Payment:BankCode + AccountNumber     ← Thông tin tài khoản ngân hàng
```

### Bước 2 — Migration & Seed
```bash
cd EcoClean.API
dotnet ef migrations add InitialCreate
dotnet ef database update
# Database tự tạo và seed 8 recipes mẫu
```

### Bước 3 — Chạy cả 2 project
```bash
# Terminal 1 — API
cd EcoClean.API && dotnet run   # http://localhost:5000
# Swagger UI: http://localhost:5000/swagger

# Terminal 2 — MVC
cd EcoClean.MVC && dotnet run   # http://localhost:5001
```

---

## Deploy với Docker Compose (Production)

```bash
# 1. Clone repo
git clone <repo-url> && cd EcoClean

# 2. Tạo file .env
cp .env.example .env
# Điền JWT_SECRET, GEMINI_API_KEY, BANK_CODE, BANK_ACCOUNT

# 3. Build & Start
docker compose up -d --build

# 4. Kiểm tra
docker compose ps
docker compose logs api --tail=50
```

Sau khi chạy:
- Frontend: http://localhost (qua Nginx)
- API Swagger: http://localhost/swagger
- SQL Server: localhost:1433

---

## API Endpoints

| Method | Path | Auth | Mô tả |
|--------|------|------|-------|
| POST | /api/auth/register | ❌ | Đăng ký |
| POST | /api/auth/login | ❌ | Đăng nhập |
| GET/POST | /api/profile | ✅ | BMI + TDEE |
| GET | /api/recipes | ❌ | Thư viện món |
| GET/POST/DELETE | /api/mealplans | ✅ | Lịch ăn |
| GET/POST/DELETE | /api/weight | ✅ | Cân nặng |
| POST | /api/subscriptions | ✅ | Đăng ký Premium |
| POST | /api/foodscan | ✅👑 | Quét ảnh AI |
| POST | /api/chat | ✅👑 | Chatbot |
| POST | /api/aimealplan | ✅👑 | Thực đơn AI |

✅ = JWT required | 👑 = Premium required

---

## Biến môi trường

| Tên | Mô tả |
|-----|-------|
| `JWT_SECRET` | Khoá bí mật JWT (≥32 ký tự) |
| `GEMINI_API_KEY` | API key từ Google AI Studio |
| `BANK_CODE` | Mã ngân hàng VietQR (VD: MB, VCB, TCB) |
| `BANK_ACCOUNT` | Số tài khoản ngân hàng |
