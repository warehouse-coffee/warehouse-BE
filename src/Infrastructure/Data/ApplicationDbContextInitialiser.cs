using System.Runtime.InteropServices;
using warehouse_BE.Domain.Constants;
using warehouse_BE.Domain.Entities;
using warehouse_BE.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace warehouse_BE.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default company 
        var existingCompany = await _context.Companies
        .FirstOrDefaultAsync(c => c.CompanyId == "HCMUTE");

        if (existingCompany == null)
        {
            var company = new Company
            {
                CompanyId = "HCMUTE",
                CompanyName = "Ho Chi Minh City University of Technology and Education",
                PhoneContact = "+84 28 3896 8641",
                EmailContact = "contact@hcmute.edu.vn",
                Created = DateTimeOffset.UtcNow,
                LastModified = DateTimeOffset.UtcNow,
            };
            _context.Companies.Add(company);
        }
        // Default roles
        var superAdminRole = new IdentityRole("Super-Admin");
        var adminRole = new IdentityRole("Admin");
        var customerRole = new IdentityRole("Customer");

        // Create roles if they don't exist
        if (_roleManager.Roles.All(r => r.Name != superAdminRole.Name))
        {
            await _roleManager.CreateAsync(superAdminRole);
        }

        if (_roleManager.Roles.All(r => r.Name != adminRole.Name))
        {
            await _roleManager.CreateAsync(adminRole);
        }

        if (_roleManager.Roles.All(r => r.Name != customerRole.Name))
        {
            await _roleManager.CreateAsync(customerRole);
        }

        // Default users
        var superAdmin = new ApplicationUser { UserName = "SuperAdmin@ute.com", Email = "superadmin@ute.com", CompanyId = "HCMUTE" };
        var admin = new ApplicationUser { UserName = "Admin@ute.com", Email = "admin@ute.com", CompanyId = "HCMUTE" };
        var customer = new ApplicationUser { UserName = "Customer@ute.com", Email = "customer@ute.com" , CompanyId = "HCMUTE" };

        // Create users and assign roles
        if (_userManager.Users.All(u => u.UserName != superAdmin.UserName))
        {
            await _userManager.CreateAsync(superAdmin, "SuperAdmin1!");
            await _userManager.AddToRolesAsync(superAdmin, new[] { superAdminRole.Name! });
        }

        if (_userManager.Users.All(u => u.UserName != admin.UserName))
        {
            await _userManager.CreateAsync(admin, "Admin1!");
            await _userManager.AddToRolesAsync(admin, new[] { adminRole.Name! });
        }

        if (_userManager.Users.All(u => u.UserName != customer.UserName))
        {
            await _userManager.CreateAsync(customer, "Customer1!");
            await _userManager.AddToRolesAsync(customer, new[] { customerRole.Name! });
        }

        await _context.SaveChangesAsync(); 
    }
}
