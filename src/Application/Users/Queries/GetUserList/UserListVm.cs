using warehouse_BE.Application.Common.Models;
using warehouse_BE.Domain.Common;

namespace warehouse_BE.Application.Users.Queries.GetUserList;

public class UserListVm
{
    public UserListVm() { 
        this.Users  = new List<UserDto> { };
    }
    public List<UserDto> Users { get; set; }
    public Page? Page { get; set; }
}
