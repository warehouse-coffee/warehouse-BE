using warehouse_BE.Application.Common.Models;
using warehouse_BE.Application.Customer.Commands.CreateCustomer;
using warehouse_BE.Application.Customer.Commands.UpdateCustomer;
using warehouse_BE.Application.Customer.Queries.GetCustomerDetail;
using warehouse_BE.Application.IdentityUser.Commands.CreateUser;
using warehouse_BE.Domain.Common;

namespace warehouse_BE.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

    Task<Result> DeleteUserAsync(string userId);

    Task<string?> SignIn(string username, string password);
    Task<(Result Result, string UserId)> RegisterAsync(UserRegister userRegister);
    Task<Result> ResetPasswordAsync(string email, string currentPassword, string newPassword);
    Task<(Result Result, string CompanyId)> GetCompanyId(string userId);
    Task<Result> CreateCustomer(CustomerRequest request);
    Task<Result> UpdateCustomer(UpdateCustomer request);
    Task<List<UserDto>> GetUsersByRoleAsync(string roleName, string companyId);
    Task<CustomerDetailVM?> GetUserByIdAsync(string userId);
    Task<Result> DeleteUserByIdAsync(string userId);
}
