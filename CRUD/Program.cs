using CRUD.Context;
using CRUD.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("koneksi")));

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();

    try
    {
        await EnsureRolesAsync(roleManager, logger);
        await EnsureDefaultAdminAsync(userManager, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while creating roles or default admin user.");
    }
}

app.Run();

static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
{
    string[] roleNames = { "Admin", "Staff", "Member" };

    foreach (var roleName in roleNames)
    {
        try
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
                logger.LogInformation($"Role {roleName} created successfully.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occurred while creating role {roleName}");
            throw;
        }
    }
}

static async Task EnsureDefaultAdminAsync(UserManager<AppUser> userManager, ILogger logger)
{
    var adminEmail = "admin@example.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        try
        {
            var admin = new AppUser { UserName = adminEmail, Email = adminEmail };
            var result = await userManager.CreateAsync(admin, "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
                logger.LogInformation("Default admin user created and assigned to Admin role successfully.");
            }
            else
            {
                logger.LogError("Failed to create default admin user.");
                foreach (var error in result.Errors)
                {
                    logger.LogError(error.Description);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating the default admin user.");
            throw;
        }
    }
}
