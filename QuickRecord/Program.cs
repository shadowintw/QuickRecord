using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using QuickRecord.Data;

var builder = WebApplication.CreateBuilder(args);

// ───── 1. 註冊 DbContext ─────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDBConn")));

// ───── 2. 認證：Microsoft Entra ID (Azure AD) ─────
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi()   // 若日後需呼叫 Microsoft Graph
    .AddInMemoryTokenCaches();

// ───── 3. 授權：角色與預設登入 ─────
builder.Services.AddAuthorization(options =>
{
    // 全站預設必須已驗證
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    // 只允許 Admin 角色
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// ───── 4. MVC 與 Microsoft Identity UI ─────
builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();   // 自帶 /MicrosoftIdentity/Account/* 登入登出頁

var app = builder.Build();

// ───── 5. Middleware 管線 ─────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();               // 生產環境強制 HSTS
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ★ 一定要先驗證再授權
app.UseAuthentication();
app.UseAuthorization();

// ───── 6. 路由 ─────
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=QuickNotes}/{action=Index}/{id?}");

app.Run();
