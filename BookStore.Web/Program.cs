using BookStore.Web.Data;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder =
    WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
    options =>
        options.UseSqlite(
            "Data Source=Orders.db"));

builder.Services.AddControllersWithViews();

WebApplication app =
    builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern:
        "{controller=Products}/{action=Index}/{id?}");

app.Run();
