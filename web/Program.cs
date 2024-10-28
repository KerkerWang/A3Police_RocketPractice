var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 向服務容器中添加 MVC controller 和 view 支援。
// 這樣就能讓應用程式使用 MVC 模式，來處理 request 和呈現 view 。
// 如果應用程式是 MVC 或 Razor Pages 項目，這一行是必須的。
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.

// 增強應用程式的錯誤處理和安全性，但對開發環境來說不是必須的。
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // 告訴瀏覽器只能以 HTTPS 通信，是一種安全強制策略。
    app.UseHsts();
}

//app.UseDeveloperExceptionPage();

// 作用：強制使用 HTTPS 協議，將所有 HTTP 請求自動重定向至 HTTPS。
// 必要性：在生產環境中使用 HTTPS 是必要的，這行可以強制 HTTPS 連接。
app.UseHttpsRedirection();

// 作用：啟用靜態檔案中介軟體，以提供 wwwroot 目錄下的靜態資源（如圖片、CSS、JavaScript）。
// 必要性：如果應用程式有靜態資源，這行是必要的。
// 替代：若應用程式不使用靜態資源（如 API 專案），這行可以省略。
app.UseStaticFiles();

// 作用：啟用路由中介軟體，讓應用程式依據定義的路由來處理請求。此行幫助 ASP.NET Core 根據控制器、動作和頁面處理請求。
// 必要性：若要在應用程式中使用路由（如 MVC 路由或 API 路由），這行是必須的。
// 替代：API 或 MVC 項目無法省略這行，但可以在基本頁面應用中省略。
app.UseRouting();

// 作用：啟用授權中介軟體，確保只有經授權的用戶可以存取特定資源。
// 必要性：如果應用程式包含任何需要身份驗證的頁面或 API，此行是必須的。
// 替代：如果不需要身份驗證或授權（例如開放 API 或公眾頁面），可以省略這行。
//app.UseAuthorization();

// 作用：設置預設的路由範本，指定了 URL 路徑結構，其中 {controller}、{action} 和可選的 {id} 表示控制器、動作和 ID 參數。
// 必要性：如果使用 MVC 控制器，這行是必須的，用於映射路由。
// 替代：可以替換成 app.MapDefaultControllerRoute(); 來使用相同的預設路由規則，簡化程式碼。
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();