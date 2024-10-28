using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Guardians.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Configuration;
using Guardians.Models;
using Guardians.Services;
using Utility = Microsoft.IdentityModel.Tokens.Utility;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBContext") ?? throw new InvalidOperationException("Connection string 'DBContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

//加入HttpContext
builder.Services.AddHttpContextAccessor();

//加入Session
builder.Services.AddSession();

//告知將使用Cookie驗證機制
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        //重新導回登入頁
        options.LoginPath = new PathString("/Account/Login");
        //設定登入後重新導回的頁面的轉址參數
        options.ReturnUrlParameter = "ret";
        //閒置時間>指定時間就要登出
        options.ExpireTimeSpan = TimeSpan.FromMinutes(1500);
    });

//注入服務
builder.Services.AddSingleton<Guardians.Models.Utility>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IUnitService, UnitService>();
//appsetting參數
builder.Services.Configure<MyConfiguration>(builder.Configuration.GetSection("MyConfiguration"));

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
