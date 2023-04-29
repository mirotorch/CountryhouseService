using CountryhouseService.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using WebPWrecover.Services;
using CountryhouseService.Services;
using CountryhouseService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
    builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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
app.MapRazorPages();

var scopedFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopedFactory.CreateScope())
{
    var RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var UserManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    await CreateRoles(RoleManager, UserManager);
}


app.Run();



 async Task CreateRoles(RoleManager<IdentityRole> RoleManager, UserManager<User> UserManager)
{
    //initializing custom roles 
    string[] roleNames = { "Owner", "Worker", };
    IdentityResult roleResult;

    foreach (var roleName in roleNames)
    {
        var roleExist = await RoleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}