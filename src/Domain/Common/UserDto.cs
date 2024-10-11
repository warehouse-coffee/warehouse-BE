
namespace warehouse_BE.Domain.Common
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string? CompanyId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? RoleName { get; set; }
        public bool isActived { get; set; }
    }
}
