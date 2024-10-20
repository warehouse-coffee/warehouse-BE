using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Common.Models;
using warehouse_BE.Application.Response;
using warehouse_BE.Domain.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace warehouse_BE.Application.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<ResponseDto>
{
    public  string? Id { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? RoleName { get; set; }
    public string? CompanyId { get; set; }
    public bool IsActived { get; set; }
    public IFormFile? AvatarImage { get; set; }
}
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ResponseDto>
{
    private readonly IIdentityService _identityService;
    private readonly IFileService _fileService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UpdateUserCommandHandler(IIdentityService identityService,IFileService fileService, IHttpContextAccessor httpContextAccessor)
    {
        this._identityService = identityService;
        this._fileService = fileService;    
        this._httpContextAccessor = httpContextAccessor;
    }
    public async Task<ResponseDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var rs = new ResponseDto {
            Message = "Failed to update user: an unexpected error occurred.",
            StatusCode = 400
         };
        if (request.Id != null)
        {   
            var currentUser = await _identityService.GetUserById(request.Id);
            if (!string.IsNullOrEmpty(currentUser.Id))
            {
                string fileName = "";

                var userDto = new UserDto
                {
                    Id = request.Id,
                    UserName = request.UserName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    RoleName = request.RoleName,
                    isActived = request.IsActived,
                    CompanyId = request.CompanyId
                };
                if (request.AvatarImage != null)
                {
                    if (request.AvatarImage.Length > 1 * 1024 * 1024)
                    {
                        return new ResponseDto(StatusCodes.Status400BadRequest, "File size should not exceed 1 MB");
                    }
                    string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];

                    fileName = await _fileService.SaveFileAsync(request.AvatarImage, allowedFileExtentions);
                    if (fileName != null)
                    {
                        userDto.AvatarImage = fileName;
                    }
                }
                var result = await _identityService.UpdateUser(userDto, request.Password);
                var user = await _identityService.GetUserById(request.Id);
                if (user != null)
                {
                    if (_httpContextAccessor.HttpContext != null &&
                   _httpContextAccessor.HttpContext.Request != null)
                    {
                        var requestContext = _httpContextAccessor.HttpContext.Request;
                        var scheme = requestContext.Scheme;
                        var host = requestContext.Host.Value;
                        user.AvatarImage = $"{scheme}://{host}/Resources/{user.AvatarImage}";
                    }
                }
                if (result.Succeeded)
                {
                    rs.Message = "User updated successfully.";
                    rs.StatusCode = 200;
                    rs.Data = user;
                }
                else
                {
                    rs.Message = "Failed to update user: " + string.Join(", ", result.Errors);
                }
            }
        }
        return rs;
    }
}
