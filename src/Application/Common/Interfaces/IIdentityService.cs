using warehouse_BE.Application.Common.Models;
using warehouse_BE.Application.IdentityUser.Commands.CreateUser;

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

}
