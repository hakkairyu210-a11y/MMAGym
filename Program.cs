using Microsoft.EntityFrameworkCore;
using MMA.Data;
using MMA.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add Entity Framework Core SQL Server MMADbContext (Connect via DefaultConnection)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                    ?? builder.Configuration.GetConnectionString("MmaConnection");

builder.Services.AddDbContext<MmaDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<MMADbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Seed Database asynchronously if available
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<MmaDbContext>();
        await DbSeeder.SeedAsync(dbContext);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Cấu hình kết nối SQL Server local chưa sẵn sàng, ứng dụng sẽ chạy ở chế độ dự phòng.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapBlazorHub();
app.MapRazorPages();

app.Run();
