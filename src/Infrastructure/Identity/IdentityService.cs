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
using warehouse_BE.Application.Employee.Commands.CreateEmployee;
using warehouse_BE.Application.Employee.Commands.UpdateEmployee;
using warehouse_BE.Domain.Common;
using warehouse_BE.Application.Employee.Queries.GetEmployeeDetail;
using warehouse_BE.Application.CompanyOwner.Queries.GetCompanyOwnerDetail;
using warehouse_BE.Application.Storages.Queries.GetStorageList;
using warehouse_BE.Application.CompanyOwner.Commands.UpdateCompanyOwner;
using warehouse_BE.Domain.Entities;
using warehouse_BE.Application.Employee.Queries.GetListEmployee;

namespace warehouse_BE.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILoggerService _logger;
    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        IConfiguration configuration,
        ApplicationDbContext context,
        RoleManager<IdentityRole> roleManager,
        ILoggerService logger)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _configuration = configuration;
        _context = context;
        _roleManager = roleManager;
        _logger = logger;
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
        if(user == null)
        {
            return Result.Success();
        }
        user.isDeleted = true;
        var result = await _userManager.UpdateAsync(user);
        return result.ToApplicationResult();
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }

    public async Task<string?> SignIn(string email, string password, string sourcePath)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.isDeleted);

            if (user == null)
            {
                return null;
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, password);

            if (!passwordValid)
            {
                return null;
            }
           
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
                new Claim("avatar", sourcePath+user.AvatarImage ?? string.Empty)
            };

                claims.AddRange(roles.Select(role => new Claim("role", role)));

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(24),
                    signingCredentials: creds);
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var existingToken = await _userManager.GetAuthenticationTokenAsync(user, "MyApp", "JWT");

            if (existingToken != null)
            {
                var handler = new JwtSecurityTokenHandler();
                var Token = handler.ReadToken(existingToken) as JwtSecurityToken;

                if (Token != null && Token.ValidTo > DateTime.UtcNow)
                {
                    jwtToken = existingToken;
                }
            } else
            {
                await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "JWT", jwtToken);
            }

            return jwtToken;

        } catch (Exception ex)
        {
            throw new Exception("error" + ex);
        }
    }
    public async Task<bool> Logout(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            await _userManager.RemoveAuthenticationTokenAsync(user, "MyApp", "JWT");
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("error" + ex);
        }
    }
    public async Task<bool> ValidateTokenAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null)
            return false;

        var user = await _userManager.FindByIdAsync(userIdClaim);
        if(user != null)
        {
            var authToken = await _userManager.GetAuthenticationTokenAsync(user, "MyApp", "JWT");

            if (user == null || authToken != token)
                return false;
        }
      

        return true;
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
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userRegister.Email && !u.isDeleted);
        if (existingUser != null)
        {
            return (Result.Failure(new[] { "Email is already in use." }), string.Empty);
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

        var companyExists = await _context.Companies.AnyAsync(c => c.CompanyId == userRegister.CompanyId);
        var roleExists = await _roleManager.RoleExistsAsync(userRegister.RoleName);

        if (!companyExists)
        {
            return (Result.Failure(new[] { "CompanyId does not exist." }), string.Empty);
        }

        if (!roleExists)
        {
            return (Result.Failure(new[] { $"Role {userRegister.RoleName} does not exist." }), string.Empty);
        }
        var user = new ApplicationUser
        {
            UserName = userRegister.UserName,
            Email = userRegister.Email,
            PhoneNumber = userRegister.PhoneNumber,
            CompanyId = userRegister.CompanyId,
            isActived = false   
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
            user.isActived = true;
            await _userManager.UpdateAsync(user);
            return Result.Failure(resetPasswordResult.Errors.Select(e => e.Description));
        }

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
    public async Task<(Result result, EmployeeDto? employeeDto)> CreateEmployee(EmployeeRequest request)
    {
        if(string.IsNullOrEmpty(request?.UserName)  || string.IsNullOrEmpty(request?.Password) || string.IsNullOrEmpty(request?.Email))
        {
            return (Result.Failure(new[] { "Unaccepted the request!." }), null);
        }
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && !u.isDeleted);
        if (existingUser != null)
        {
            return (Result.Failure(new[] { "Email is already in use." }), null);
        }
        var storageIds = request.Warehouses ?? new List<int>();

        var storages = storageIds.Any()
            ? await _context.Storages
                .Where(s => storageIds.Contains(s.Id)) 
                .ToListAsync()
            : new List<Storage>();

        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            CompanyId = request.CompanyId,
        };
        var employeeDto = new EmployeeDto();
        try
        {
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {

                var errors = result.Errors.Select(e => e.Description).ToList();
                return (Result.Failure(errors), null);
            }
            foreach (var storage in storages)
            {
                var userStorage = new UserStorage
                {
                    UserId = user.Id,
                    StorageId = storage.Id,
                };

                user.UserStorages.Add(userStorage);
            }

            await _userManager.UpdateAsync(user);
            var roleResult = await _userManager.AddToRoleAsync(user, "Employee");
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                var roleErrors = roleResult.Errors.Select(e => e.Description).ToList();
                return (Result.Failure(roleErrors), null);
            }
            employeeDto = new EmployeeDto
            {
                Id = user.Id,
                CompanyId = user.CompanyId,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                isActived = true, 
                AvatarImage = user.AvatarImage 
            };
        }
        catch
        {
            var result = await _userManager.CreateAsync(user, request.Password);
            var errors = result.Errors.Select(e => e.Description).ToList();
            return (Result.Failure(errors), null);
        }
        return (Result.Success(), employeeDto);
    }
    public async Task<Result> UpdateEmployee(UpdateEmployee request,CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request?.UserName) || string.IsNullOrEmpty(request?.Password) || string.IsNullOrEmpty(request?.Email))
        {
            return Result.Failure(new[] { "Unaccepted the request! Missing required fields." });
        }

        var user = await _userManager.FindByIdAsync(request.Id);
        if (user == null)
        {
            return Result.Failure(new[] { "Employee not found." });
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
        try
        {
            var currentUser = await _context.Users
            .Include(u => u.UserStorages)  
            .ThenInclude(us => us.Storage)
            .FirstOrDefaultAsync(u => u.Id == request.Id);

            if (currentUser == null)
            {
                return Result.Failure(new[] { "User not found." });
            }

            // Lấy danh sách Storage IDs cần thêm
            var storageIds = request.Warehouses ?? new List<int>();
            var newStorages = await _context.Storages
                                             .Where(s => storageIds.Contains(s.Id))
                                             .ToListAsync();

            // Remove `UserStorage` entries that are not in the `storageIds`
            currentUser.UserStorages = currentUser.UserStorages
                .Where(us => storageIds.Contains(us.StorageId))
                .ToList();


            // Thêm các `UserStorage` mới chưa có trong `currentUser.UserStorages`
            foreach (var storage in newStorages)
            {
                if (!currentUser.UserStorages.Any(us => us.StorageId == storage.Id))
                {
                    currentUser.UserStorages.Add(new UserStorage
                    {
                        UserId = currentUser.Id,
                        StorageId = storage.Id,
                        Storage = storage 
                    });
                }
            }

            // Cập nhật trạng thái của User và lưu thay đổi
            var updateResult = await _userManager.UpdateAsync(currentUser);

            await _context.SaveChangesAsync(cancellationToken);

            if (!updateResult.Succeeded)
            {
                return Result.Failure(updateResult.Errors.Select(e => e.Description));
            }
        }
        catch (Exception ex) 
        {
            _logger.LogError( "Failed to update Employee.", ex);
            return Result.Failure(new[] { "An error occurred while updating the Employee." });

        }
        return Result.Success();
    }
    public async Task<List<UserDto>> GetUsersByRoleAsync(string roleName, string companyId)
    {

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            throw new Exception($"Role '{roleName}' does not exist.");
        }

        var users = await _userManager.Users
        .Where(u => u.CompanyId == companyId && !u.isDeleted) 
        .ToListAsync();
        string RoleDisplay;
        if (roleName == "Employee")
        {
            RoleDisplay = "Employee"; 
        }
        else
        {
            RoleDisplay = roleName;
        }
            

        var usersInRole = new List<UserDto>();

        foreach (var user in users)
        {
            if (await _userManager.IsInRoleAsync(user, roleName))
            {
                usersInRole.Add(new UserDto
                {
                    Id = user.Id,
                    CompanyId = user.CompanyId,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    RoleName = RoleDisplay
                }); 
            }
        }

        return usersInRole;
    }
    public async Task<EmployeeDetailVM?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null || user.isDeleted)
        {
            return null; 
        }
        var EmployeeDetail = new EmployeeDetailVM
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            CompanyId = user.CompanyId 
        };

        return EmployeeDetail;
    }
    public async Task<Result> DeleteUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Failure(new[] { $"User with ID '{userId}' not found." });
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return Result.Success();
        }

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return Result.Failure(new[] { $"Failed to delete user: {errors}" });
    }
    public async Task<CompanyOwnerDetailDto?> GetCompanyOwnerByIdAsync(string userId)
    {
        var user = await _userManager.Users
            .Include(u => u.UserStorages)  
            .ThenInclude(us => us.Storage) 
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return null;
        }

        var employeeDetail = new CompanyOwnerDetailDto
        {
            CompayOwnerId = user.Id,
            Name = user.UserName,
            Email = user.Email,
            Phone = user.PhoneNumber,
            CompanyId = user.CompanyId,
            ImageFile = user.AvatarImage,
            Storages = user.UserStorages.Select(us => new StorageDto
            {
                Name = us.Storage.Name,
                Location = us.Storage.Location,
                Status = us.Storage.Status,
            }).ToList()
        };

        return employeeDetail;

    }
    public async Task<string> GetRoleNamebyUserId(string userId)
    {
        string rs = "";
        var user = await _userManager.FindByIdAsync(userId);
        if(user != null)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (roles != null && roles.Any())
            {
                rs = roles.First();
            }
        }
        return rs;
    }
    public async Task<Result> UpdateCompanyOwner(UpdateCompanyOwner request)
    {
        var companyOwner = await _userManager.FindByIdAsync(request.UserId);
        if (companyOwner == null)
        {
            return Result.Failure(new List<string> { "User not found." });
        }

        if (!string.IsNullOrEmpty(request.UserName))
        {
            companyOwner.UserName = request.UserName;
            var userNameResult = await _userManager.UpdateAsync(companyOwner);
            if (!userNameResult.Succeeded)
            {
                return Result.Failure(userNameResult.Errors.Select(e => e.Description).ToList());
            }
        }
        if (!string.IsNullOrEmpty(request.Password))
        {
            var passwordResult = await _userManager.RemovePasswordAsync(companyOwner);
            if (passwordResult.Succeeded)
            {
                passwordResult = await _userManager.AddPasswordAsync(companyOwner, request.Password);
                if (!passwordResult.Succeeded)
                {
                    return Result.Failure(passwordResult.Errors.Select(e => e.Description).ToList());
                }
            }
        }

        if (!string.IsNullOrEmpty(request.Email))
        {
            companyOwner.Email = request.Email;
            var emailResult = await _userManager.UpdateAsync(companyOwner);
            if (!emailResult.Succeeded)
            {
                return Result.Failure(emailResult.Errors.Select(e => e.Description).ToList());
            }
        }
        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            companyOwner.PhoneNumber = request.PhoneNumber;
            var phoneResult = await _userManager.UpdateAsync(companyOwner);
            if (!phoneResult.Succeeded)
            {
                return Result.Failure(phoneResult.Errors.Select(e => e.Description).ToList());
            }
        }

        if (!string.IsNullOrEmpty(request.CompanyId))
        {
            companyOwner.CompanyId = request.CompanyId;
            var companyResult = await _userManager.UpdateAsync(companyOwner);
            if (!companyResult.Succeeded)
            {
                return Result.Failure(companyResult.Errors.Select(e => e.Description).ToList());
            }
        }
        if (!string.IsNullOrEmpty(request.AvatarImage))
        {
            companyOwner.AvatarImage = request.AvatarImage;
            var companyResult = await _userManager.UpdateAsync(companyOwner);
            if (!companyResult.Succeeded)
            {
                return Result.Failure(companyResult.Errors.Select(e => e.Description).ToList());
            }
        }

        return Result.Success();
    }
    public async Task<Result> UpdateStoragesForUser(string userId, List<StorageDto> updatedStorages, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            // Get the current UserStorages associated with the user
            var currentUserStorages = user.UserStorages.ToList();

            // Remove UserStorages that are not present in updatedStorages
            var storagesToRemove = currentUserStorages
                .Where(us => !updatedStorages.Any(s => s.Id == us.StorageId))
                .ToList();

            if (storagesToRemove.Any())
            {
                _context.UserStorages.RemoveRange(storagesToRemove);
            }

            foreach (var storageDto in updatedStorages)
            {
                // Check if the UserStorage exists
                var existingUserStorage = currentUserStorages.FirstOrDefault(us => us.StorageId == storageDto.Id);

                if (existingUserStorage != null)
                {
                    // Update existing UserStorage
                    if (!string.IsNullOrEmpty(storageDto.Name) && !string.IsNullOrEmpty(storageDto.Location) && storageDto.Status.HasValue)
                    {
                        existingUserStorage.Storage.Name = storageDto.Name;
                        existingUserStorage.Storage.Location = storageDto.Location;
                        existingUserStorage.Storage.Status = (Domain.Enums.StorageStatus)storageDto.Status;
                    }
                }
                else
                {
                    // Add new UserStorage if not found
                    if (!string.IsNullOrEmpty(storageDto.Name) && !string.IsNullOrEmpty(storageDto.Location) && storageDto.Status.HasValue)
                    {
                        var newStorage = new Domain.Entities.Storage
                        {
                            Name = storageDto.Name,
                            Location = storageDto.Location,
                            Status = (Domain.Enums.StorageStatus)storageDto.Status,
                        };

                        // Create new UserStorage to link the user with the storage
                        var newUserStorage = new UserStorage
                        {
                            UserId = user.Id,
                            Storage = newStorage
                        };

                        user.UserStorages.Add(newUserStorage);
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();

    }
    public async Task<Result> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            user.isDeleted = true;
            await _userManager.UpdateAsync(user);
        } else {
            return Result.Failure(new[] { $"Failed to delete user:" });
        }
        return Result.Success();
    }
    public async Task<List<UserDto>> GetUserList()
    {
        var rs = new List<UserDto>();
        try
        {
            var users = await _userManager.Users
                .Where(o => !o.isDeleted)
                .ToListAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var roleName = roles.FirstOrDefault();

                if (roleName == "Super-Admin")
                    continue;

                var userDto = new UserDto
                {
                    Id = user.Id,
                    CompanyId = user.CompanyId,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    RoleName = roleName,
                    isActived = user.isActived
                };

                rs.Add(userDto);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("GetUserList - An error occurred while fetching user list.", ex);
        }
        return rs;
    }

    public async Task<Result> UpdateUser(UserDto userDto, string? password)
    {
        if(userDto.Id != null)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id);
            if(user != null)
            {
                var company = _context.Companies.Where(o => o.CompanyId == userDto.CompanyId).FirstOrDefault();
                if(company == null)
                {
                    _context.Companies.Add(new Domain.Entities.Company
                    {
                        CompanyId = userDto.CompanyId
                    });
                    await _context.SaveChangesAsync();
                }
                user.UserName = userDto.UserName;
                user.Email = userDto.Email;
                user.PhoneNumber = userDto.PhoneNumber;
                user.CompanyId = userDto.CompanyId;
                user.isActived = userDto.isActived;
                user.AvatarImage = userDto.AvatarImage;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    return Result.Failure(updateResult.Errors.Select(e => e.Description).ToArray());
                }
                if (!string.IsNullOrEmpty(password))
                {
                    var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                    if (!removePasswordResult.Succeeded)
                    {
                        return Result.Failure(removePasswordResult.Errors.Select(e => e.Description).ToArray());
                    }

                    var addPasswordResult = await _userManager.AddPasswordAsync(user, password);
                    if (!addPasswordResult.Succeeded)
                    {
                        return Result.Failure(addPasswordResult.Errors.Select(e => e.Description).ToArray());
                    }
                }

                if (!string.IsNullOrEmpty(userDto.RoleName))
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var currentRole = currentRoles.FirstOrDefault();
                    var roleExists = await _roleManager.RoleExistsAsync(userDto.RoleName);
                    if (!roleExists)
                    {
                        return Result.Failure(new[] { "Role does not exist." });
                    }
                        if (currentRole != userDto.RoleName)
                    {
                        if (!string.IsNullOrEmpty(currentRole))
                        {
                            var removeRoleResult = await _userManager.RemoveFromRoleAsync(user, currentRole);
                            if (!removeRoleResult.Succeeded)
                            {
                                return Result.Failure(removeRoleResult.Errors.Select(e => e.Description).ToArray());
                            }
                        }

                        var addRoleResult = await _userManager.AddToRoleAsync(user, userDto.RoleName);
                        if (!addRoleResult.Succeeded)
                        {
                            return Result.Failure(addRoleResult.Errors.Select(e => e.Description).ToArray());
                        }
                    }
                }
            }
        }
        
        return Result.Success();
    }
    public async Task<UserDto> GetUserById(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null || user.isDeleted)
        {
            return new UserDto();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var storages = user.UserStorages
    .Select(us => new Storage
    {
        Id = us.Storage.Id,
        Name = us.Storage.Name,
        Location = us.Storage.Location,
        Status = us.Storage.Status
    })
    .ToList();
        var userDto = new UserDto
        {
            Id = user.Id,
            CompanyId = user.CompanyId,   
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            RoleName = roles.FirstOrDefault(),  
            isActived = user.isActived,
            Storages = storages,
            AvatarImage = user.AvatarImage,
        };

        return userDto;
    }
    public async Task<List<Storage>> GetUserStoragesAsync(string userId)
    {
        var user = await _context.Users
            .Include(u => u.UserStorages)  // Include UserStorages instead of Storages
            .ThenInclude(us => us.Storage)  // Also include the related Storage entity
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.UserStorages.Select(us => us.Storage).ToList() ?? new List<Storage>();

    }
    public async Task<Storage> AddUserStorageAsync(string userId, Storage updatedStorage,CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        // Check if the storage already exists for this user through UserStorages
        var existingStorage = user.UserStorages
            .FirstOrDefault(us => us.Storage.Name == updatedStorage.Name);

        if (existingStorage != null)
        {
            throw new Exception($"Storage with name '{updatedStorage.Name}' already exists for this user.");
        }

        // Create a new UserStorage for the user and the updated storage
        var newUserStorage = new UserStorage
        {
            UserId = user.Id,
            StorageId = updatedStorage.Id  // Assuming updatedStorage already exists in the database
        };

        // Add the new UserStorage to the user's UserStorages collection
        user.UserStorages.Add(newUserStorage);

        // Optionally, you can update the Storage entity if needed
        // _context.Storages.Update(updatedStorage);

        await _context.SaveChangesAsync(cancellationToken);

        return updatedStorage;

    }

    public async Task<int> GetTotalUser()
    {
        var superAdminRole = await _roleManager.FindByNameAsync("Super-Admin");

        var user = await _userManager.Users.CountAsync();
        if (superAdminRole == null)
        {
            return await _userManager.Users.CountAsync();
        }

        var superAdminUserIds = await _context.UserRoles
            .Where(ur => ur.RoleId == superAdminRole.Id)
            .Select(ur => ur.UserId)
            .ToListAsync();
        var userCount = await _userManager.Users
            .Where(u => !superAdminUserIds.Contains(u.Id))
            .CountAsync();

        return userCount;
    }
}
