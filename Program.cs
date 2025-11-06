using Microsoft.EntityFrameworkCore;
using StretchFitnessHub.Data;
using StretchFitnessHub.Filters;
using StretchFitnessHub.Models; // Make sure Admin model namespace is included
using BCrypt.Net;

var builder = WebApplication.CreateBuilder(args);

// I-configure ang iisang DbContext na gagamitin ng application.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!_context.Admins.Any())
    {
        var defaultAdmin = new Admin
        {
            Username = "admin",
            Password = BCrypt.Net.BCrypt.HashPassword("123")
        };
        _context.Admins.Add(defaultAdmin);
        _context.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
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
    pattern: "{controller=Account}/{action=LandingPage}/{id?}");

app.Run();
