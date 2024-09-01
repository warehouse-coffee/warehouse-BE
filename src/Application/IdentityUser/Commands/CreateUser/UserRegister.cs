
namespace warehouse_BE.Application.IdentityUser.Commands.CreateUser;

public class UserRegister
{
    public string? UserName { get; set; }
    public string? Password { get; set; } 
    public string? Email { get; set; } 
    public string? PhoneNumber { get; set; } 
    public string? CompanyId { get; set; }
    public string? RoleName { get; set; }
}
