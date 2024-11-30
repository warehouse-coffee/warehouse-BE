using warehouse_BE.Application.Common.Models;
using warehouse_BE.Application.CompanyOwner.Commands.UpdateCompanyOwner;
using warehouse_BE.Application.CompanyOwner.Queries.GetCompanyOwnerDetail;
using warehouse_BE.Application.Employee.Commands.CreateEmployee;
using warehouse_BE.Application.Employee.Commands.UpdateEmployee;
using warehouse_BE.Application.Employee.Queries.GetEmployeeDetail;
using warehouse_BE.Application.Employee.Queries.GetListEmployee;
using warehouse_BE.Application.IdentityUser.Commands.CreateUser;
using warehouse_BE.Application.Storages.Queries.GetStorageList;
using warehouse_BE.Domain.Common;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

    Task<Result> DeleteUserAsync(string userId);

    Task<string?> SignIn(string username, string password, string sourcePath);
    Task<(Result Result, string UserId)> RegisterAsync(UserRegister userRegister);
    Task<Result> ResetPasswordAsync(string email, string currentPassword, string newPassword);
    Task<(Result Result, string CompanyId)> GetCompanyId(string userId);
    Task<(Result result, EmployeeDto? employeeDto)> CreateEmployee(EmployeeRequest request);
    Task<Result> UpdateEmployee(UpdateEmployee request, CancellationToken cancellation);
    Task<List<UserDto>> GetUsersByRoleAsync(string roleName, string companyId);
    Task<EmployeeDetailVM?> GetUserByIdAsync(string userId);
    Task<Result> DeleteUserByIdAsync(string userId);
    Task<CompanyOwnerDetailDto?> GetCompanyOwnerByIdAsync(string userId);
    Task<string> GetRoleNamebyUserId(string userId);
    Task<Result> UpdateCompanyOwner(UpdateCompanyOwner request);
    Task<Result> UpdateStoragesForUser(string userId, List<StorageDto> updatedStorages, CancellationToken cancellationToken);
    Task<Result> DeleteUser(string userId);
    Task<List<UserDto>> GetUserList();
    Task<Result> UpdateUser(UserDto userDto, string? password);
    Task<UserDto> GetUserById(string userId);
    Task<bool> Logout(string userId);
    Task<bool> ValidateTokenAsync(string token);
    Task<List<Storage>> GetUserStoragesAsync(string userId);
    Task<Storage> AddUserStorageAsync(string userId, Storage updatedStorage, CancellationToken cancellationToken);
    Task<int> GetTotalUser();
    Task<int> TotalActiveEmployee(string companyId);
    Task<int> TotalOnlineEmployee(string companyId);
    Task<bool> UpdateUserOnlineStatusAsync(string userId, bool isOnline);
    Task<int> GetCompanyIDInt(string userID);
    Task<Result> ActivateAsync(string userId, bool isActive);
}
