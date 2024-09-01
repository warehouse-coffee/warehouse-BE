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
using warehouse_BE.Domain.Entities;
using warehouse_BE.Application.IdentityUser.Commands.CreateUser;
using warehouse_BE.Infrastructure.Data;

namespace warehouse_BE.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        IConfiguration configuration,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _configuration = configuration;
        _context = context;
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

    public async Task<string?> SignIn(string username, string password)
    {

        var user = await _userManager.FindByNameAsync(username);

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
                new Claim("userId", user.Id),
            };

            claims.AddRange(roles.Select(role => new Claim("role", role)));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
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

}
