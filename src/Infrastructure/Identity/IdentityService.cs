using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using warehouse_BE.Application.IdentityUser.Commands.CreateUser;
using warehouse_BE.Infrastructure.Data;
using warehouse_BE.Application.Customer.Commands.CreateCustomer;
using warehouse_BE.Application.Customer.Commands.UpdateCustomer;

namespace warehouse_BE.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        IConfiguration configuration,
        ApplicationDbContext context,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _configuration = configuration;
        _context = context;
        _roleManager = roleManager;
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.UserName;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName,
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }

    public async Task<string?> SignIn(string email, string password)
    {

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return null; 
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, password);

        if (!passwordValid)
        {
            return null; 
        }
        try
        {
            var roles = await _userManager.GetRolesAsync(user);

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var secretKey = jwtSettings["SecretKey"] ?? throw new ArgumentNullException("SecretKey is null");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("userId", user.Id),
                new Claim("email", user.Email ?? string.Empty),
                new Claim("username", user.UserName ?? string.Empty),
            };

            claims.AddRange(roles.Select(role => new Claim("role", role)));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        } catch (Exception ex)
        {
            throw new Exception("error" + ex);
        }
    }

    public async Task<(Result Result, string UserId)> RegisterAsync(UserRegister userRegister)
    {

        if (string.IsNullOrWhiteSpace(userRegister.UserName))
        {
            return (Result.Failure(new[] { "UserName cannot be null or empty." }), string.Empty);
        }

        if (string.IsNullOrWhiteSpace(userRegister.Password))
        {
            return (Result.Failure(new[] { "Password cannot be null or empty." }), string.Empty);
        }

        if (string.IsNullOrWhiteSpace(userRegister.Email))
        {
            return (Result.Failure(new[] { "Email cannot be null or empty." }), string.Empty);
        }

        if (string.IsNullOrWhiteSpace(userRegister.PhoneNumber))
        {
            return (Result.Failure(new[] { "PhoneNumber cannot be null or empty." }), string.Empty);
        }

        if (string.IsNullOrWhiteSpace(userRegister.CompanyId))
        {
            return (Result.Failure(new[] { "CompanyId cannot be null or empty." }), string.Empty);
        }

        if (string.IsNullOrWhiteSpace(userRegister.RoleName))
        {
            return (Result.Failure(new[] { "RoleName cannot be null or empty." }), string.Empty);
        }

        var companyExists = await _context.Company.AnyAsync(c => c.CompanyId == userRegister.CompanyId);
        if (!companyExists)
        {
            return (Result.Failure(new[] { "CompanyId does not exist." }), string.Empty);
        }
        var roleExists = await _roleManager.RoleExistsAsync(userRegister.RoleName);
        if (!roleExists)
        {
            return (Result.Failure(new[] { $"Role {userRegister.RoleName} does not exist." }),string.Empty);
        }
        var user = new ApplicationUser
        {
            UserName = userRegister.UserName,
            Email = userRegister.Email,
            PhoneNumber = userRegister.PhoneNumber,
            CompanyId = userRegister.CompanyId
        };

        var result = await _userManager.CreateAsync(user, userRegister.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return (Result.Failure(errors), string.Empty);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, userRegister.RoleName);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            var errors = roleResult.Errors.Select(e => e.Description).ToList();
            return (Result.Failure(errors), string.Empty);
        }

        return (Result.Success(), user.Id);

    }
    public async Task<Result> ResetPasswordAsync(string email, string currentPassword, string newPassword)
    {
        // Tìm kiếm người dùng dựa trên email
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return Result.Failure(new[] { "User not found." });
        }

        // Xác thực mật khẩu hiện tại
        var passwordCheck = await _userManager.CheckPasswordAsync(user, currentPassword);
        if (!passwordCheck)
        {
            return Result.Failure(new[] { "Current password is incorrect." });
        }

        // Thực hiện reset mật khẩu
        var resetPasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!resetPasswordResult.Succeeded)
        {
            return Result.Failure(resetPasswordResult.Errors.Select(e => e.Description));
        }

        // Nếu mọi thứ thành công
        return Result.Success();
    }
    public async Task<(Result Result, string CompanyId)> GetCompanyId(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return (Result.Failure(new[] { "User not found." }),string.Empty);
        }

        var companyId = user.CompanyId;

        if (string.IsNullOrEmpty(companyId))
        {
            return (Result.Failure(new[] { "User is not associated with any company." }),string.Empty);
        }

        return (Result.Success(), companyId);
    }
    public async Task<Result> CreateCustomer(CustomerRequest request)
    {
        if(string.IsNullOrEmpty(request?.UserName)  || string.IsNullOrEmpty(request?.Password) || string.IsNullOrEmpty(request?.Email))
        {
            return (Result.Failure(new[] { "Unaccepted the request!." }));

        }
        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            CompanyId = request.CompanyId
        };
        try
        {
           var result = await _userManager.CreateAsync(user, request.Password);
           var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
        }
        catch
        {
            var result = await _userManager.CreateAsync(user, request.Password);
            var errors = result.Errors.Select(e => e.Description).ToList();
            return (Result.Failure(errors));
        }
        return (Result.Success());
    }
    public async Task<Result> UpdateCustomer(UpdateCustomer request)
    {
        if (string.IsNullOrEmpty(request?.UserName) || string.IsNullOrEmpty(request?.Password) || string.IsNullOrEmpty(request?.Email))
        {
            return Result.Failure(new[] { "Unaccepted the request! Missing required fields." });
        }

        var user = await _userManager.FindByIdAsync(request.CustomerId);
        if (user == null)
        {
            return Result.Failure(new[] { "Customer not found." });
        }

        if (!string.IsNullOrEmpty(request.UserName) && request.UserName != user.UserName)
        {
            user.UserName = request.UserName;
        }

        if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
        {
            user.Email = request.Email;
        }

        if (!string.IsNullOrEmpty(request.PhoneNumber) && request.PhoneNumber != user.PhoneNumber)
        {
            user.PhoneNumber = request.PhoneNumber;
        }

        if (!string.IsNullOrEmpty(request.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordChangeResult = await _userManager.ResetPasswordAsync(user, token, request.Password);

            if (!passwordChangeResult.Succeeded)
            {
                return Result.Failure(passwordChangeResult.Errors.Select(e => e.Description));
            }
        }

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            return Result.Failure(updateResult.Errors.Select(e => e.Description));
        }

        return Result.Success();
    }
}
