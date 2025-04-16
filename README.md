# Simple E-commerce API with .NET Core

這是一個使用 .NET 6 建立的簡易電商 API 專案，具備以下功能：
- 商品管理（CRUD）
- 會員註冊、登入（JWT 驗證）
- 會員資料查詢與更新

## 技術棧

- ASP.NET Core 6
- Entity Framework Core
- SQL Server（SSMS）
- JWT 驗證(單一Function)
- Swagger（API 測試）
- PostMan (API 測試)

## 環境需求

- .NET 6 SDK
- SQL Server
- Visual Studio 2022 或 VS Code

## 安裝步驟

1. **Clone 專案**
   ```bash
   git clone 此專案git網址
   cd 你的專案資料夾

## API說明

商品系統
功能名稱 | 方法 | 路由 | 描述 | 是否需要登入（建議）
查詢所有商品 | GET | /api/products | 取得所有商品資料 | 否
查詢單一商品 | GET | /api/products/{id} | 依照 id 查詢商品 | 否
新增商品 | POST | /api/products | 新增商品資料 | ✅
更新商品 | PUT | /api/products/{id} | 更新指定 id 的商品資料 | ✅
刪除商品 | DELETE | /api/products/{id} | 刪除指定 id 的商品 | ✅

會員系統
功能名稱 | 方法 | 路由 | 描述 | 是否需要登入
會員註冊 | POST | /api/auth/register | 新增會員帳號 | 否
會員登入 | POST | /api/auth/login | 會員登入並取得 JWT Token | 否
取得目前會員資訊 | GET | /api/auth/me | 查看登入中的會員資料 | ✅
修改會員資訊 | PUT | /api/auth/me | 修改登入中的會員 Email/密碼 | ✅

## TODO LIST

- 購物車系統
- 訂單系統
- 商品分類搜尋
- 前端介面
- 第三方API串接
- 安全性功能
- 單元測試
- 性能優化