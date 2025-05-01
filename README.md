# QuickRecord

> **QuickRecord** 是一個以 **ASP.NET Core 8 MVC** 開發的「快速紀錄小貼紙」範例專案，  
> 示範 **Microsoft 365 (Entra ID) 登入**、**角色授權 (Admin / User)**、  
> **Entity Framework Core + Azure SQL Database** 與 **Azure App Service** 部署流程。

---

## 目錄
1. [功能特色](#功能特色)
2. [開發環境需求](#開發環境需求)
3. [快速開始 (本機)](#快速開始-本機)
4. [Azure 部署步驟](#azure部署步驟)
5. [專案結構](#專案結構)
6. [重要環境變數](#重要環境變數)
7. [分支策略](#分支策略)
8. [常見問題](#常見問題)
9. [授權](#授權)

---

## 功能特色
- **M365 單一登入**：使用 Microsoft.Identity.Web 整合 Entra ID (`OpenIdConnect`)。  
- **角色權限**  
  - `Admin`：可檢視 / 管理所有使用者筆記  
  - `User`：僅能 CRUD 自己的筆記  
- **快速貼紙 (QuickNotes)**  
  - 文字長度 ≤ 500 字  
  - 依建立時間倒序顯示  
  - 支援關鍵字搜尋  
- **Azure SQL Database** 儲存  
  - `OwnerId` (Object Id) 追蹤筆記擁有者  
- **CI / CD 友善**：所有敏感設定皆以 **環境變數** 或 **Key Vault 參照** 管理  
- **VS 2022 友善**：內建 Git Branch / Merge、EF Core Tooling

---

## 開發環境需求
| 項目 | 版本 |
|------|------|
| .NET SDK | **8.0 LTS** |
| IDE | Visual Studio 2022 17.9+ (*含 ASP.NET 與 Web 開發工作負載*) |
| EF Core CLI | `dotnet tool install -g dotnet-ef` |
| 資料庫 | SQL Server Express 或 Azure SQL |
| Azure CLI (部署用) | `az version` ≥ 2.58 |

---

## 快速開始 (本機)

1. **Clone 專案**
   ```bash
   git clone https://github.com/your-org/QuickRecord.git
   cd QuickRecord
2. **設定機密 (dotnet user-secrets)**
   dotnet user-secrets init

   # Azure AD
   dotnet user-secrets set "AzureAd:Instance" "https://login.microsoftonline.com/"
   dotnet user-secrets set "AzureAd:TenantId" "<TenantId>"
   dotnet user-secrets set "AzureAd:ClientId" "<AppId>"
   dotnet user-secrets set "AzureAd:ClientSecret" "<Secret>"
   dotnet user-secrets set "AzureAd:CallbackPath" "/signin-oidc"
   
   # Connection String
   dotnet user-secrets set "ConnectionStrings:MyDBConn" "Server=(localdb)\\MSSQLLocalDB;Database=QuickRecord;Trusted_Connection=True;"

3. **資料庫遷移**
   dotnet ef database update

4. **Run**
   dotnet run

---

## 專業結構
QuickRecord/
├─ Controllers/
│  └─ QuickNotesController.cs  // CRUD + 權限
├─ Data/
│  └─ AppDbContext.cs         // EF Core DbContext
├─ Models/
│  └─ QuickNote.cs
├─ Views/
│  ├─ QuickNotes/
│  └─ Shared/_Layout.cshtml   // 登入狀態列
├─ wwwroot/
├─ Program.cs                 // 認證 + DI + Middlewares
└─ README.md

---

## 重要環境變數
| 名稱 | 用途 | 建議存放位置 |
|------|------|--------------|
| `ConnectionStrings:MyDBConn` | SQL 連線字串 | App Service 設定值 / Key Vault |
| `AzureAd:ClientSecret` | 應用程式密碼 | **Key Vault** 參照 |
| 其餘 `AzureAd:*` | Entra ID 參數 | App Service 設定值 |

---

## 分支策略
| 分支 | 目的 |
|------|------|
| `master` / `main` | 可隨時部署之穩定版 |
| `feature/*` | 新功能開發（例：`feature/login-ms365`） |
| `hotfix/*` | 緊急修補 |

---

## 授權
本專案採用 MIT License，詳見 LICENSE。
