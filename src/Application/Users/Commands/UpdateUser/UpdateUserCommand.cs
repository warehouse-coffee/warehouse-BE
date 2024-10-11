using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Common.Models;
using warehouse_BE.Application.Response;
using warehouse_BE.Domain.Common;

namespace warehouse_BE.Application.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<ResponseDto>
{
    public  string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? RoleName { get; set; }
}
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ResponseDto>
{
    private readonly IIdentityService _identityService;
    private readonly IUser _currentUser;
    public UpdateUserCommandHandler(IIdentityService identityService, IUser currentUser)
    {
        _identityService = identityService;
        _currentUser = currentUser;
    }
    public async Task<ResponseDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var rs = new ResponseDto();
        var userDto = new UserDto
        {
            Id = request.UserId,
            UserName = request.UserName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            RoleName = request.RoleName
        };
        
        var result = await _identityService.UpdateUser(userDto,  request.Password);
        if (result.Succeeded)
        {
            rs.Message = "User updated successfully.";
            rs.StatusCode = 200; 
        }
        else
        {
            rs.Message = "Failed to update user.";
            rs.StatusCode = 400; 
        }

        return rs;
    }
}
