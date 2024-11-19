using Microsoft.AspNetCore.Authentication.Cookies;
using NoSQLSocialNetwork.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login"; // Đường dẫn khi người dùng chưa đăng nhập
                options.LogoutPath = "/Account/Logout"; // Đường dẫn khi người dùng đăng xuất
                options.ExpireTimeSpan = TimeSpan.FromDays(7); // Thời gian hết hạn của cookie
                options.SlidingExpiration = true; // Làm mới cookie khi người dùng truy cập lại
            });


builder.Services.AddSingleton<MongoDbService>();

builder.Services.AddControllersWithViews();







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

app.UseAuthentication();  // Sử dụng Authentication
app.UseAuthorization();   // Sử dụng Authorization

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
