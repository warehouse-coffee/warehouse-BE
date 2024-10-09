using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;
using warehouse_BE.Application.Storages.Queries.GetStorageList;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace warehouse_BE.Application.CompanyOwner.Commands.UpdateCompanyOwner;

public class UpdateCompanyOwnerCommand : IRequest<ResponseDto>
{
    public  string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
     public string? Email { get; set; }
     public string? PhoneNumber { get; set; }
    public string? CompanyId { get; set; }
   public List<StorageDto>? Storages { get; set; }
    public IFormFile? AvatarImage { get; set; }
}
public class UpdateCompanyOwnerCommandHandler : IRequestHandler<UpdateCompanyOwnerCommand, ResponseDto>
{
    public readonly IIdentityService identityService;
    private readonly IUser _currentUser;
    private readonly IApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IFileService _fileService;
    public UpdateCompanyOwnerCommandHandler(IIdentityService identityService, IUser currentUser, IApplicationDbContext context, IWebHostEnvironment webHost, IFileService fileService)
    {
        this.identityService = identityService;
        this._currentUser = currentUser;
        this._context = context;
        this._webHostEnvironment = webHost;
        this._fileService = fileService;
    }
    public async Task<ResponseDto> Handle(UpdateCompanyOwnerCommand request, CancellationToken cancellationToken)
    {
        var rs = new ResponseDto { 
            StatusCode= 400,
            Message = "",
            Data="",
        };
        string fileName = "";
        try
        {
            if (string.IsNullOrEmpty(_currentUser?.Id))
            {
                return new ResponseDto(500, "An error occurred: User ID is null or empty.");
            }
            var role = await identityService.GetRoleNamebyUserId(_currentUser.Id);
            if (request.UserId != null)
            {
                var customer = new UpdateCompanyOwner
                {
                    UserId = request.UserId,
                    UserName = request.UserName,
                    Password = request.Password,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    CompanyId = request.CompanyId,
                    AvatarImage = fileName
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
                        customer.AvatarImage = fileName;
                    } 
                }
                if (role != null && role == "Admin")
                {
                    var companyIdResult = await identityService.GetCompanyId(_currentUser.Id);
                    if (!companyIdResult.Result.Succeeded)
                    {
                        return new ResponseDto(500, string.Join(", ", companyIdResult.Result.Errors));
                    }
                    if (string.IsNullOrEmpty(companyIdResult.CompanyId))
                    {
                        return new ResponseDto(500, "User is not associated with any company.");
                    }
                    try
                    {
                        customer.CompanyId = companyIdResult.CompanyId;
                        var resultupdate = await identityService.UpdateCompanyOwner(customer);
                        if (request.Storages != null && request.Storages.Any())
                        {
                            var rsUpdateStorage = await identityService.UpdateStoragesForUser(request.UserId, request.Storages, cancellationToken);
                            if (resultupdate.Succeeded && rsUpdateStorage.Succeeded)
                            {
                                return new ResponseDto(200, "Customer update successfully.");
                            }
                        }
                        if (resultupdate.Errors != null && resultupdate.Errors.Any())
                        {
                            rs.Message = string.Join(", ", resultupdate.Errors);
                            rs.StatusCode = 400;
                        } else
                        {
                            rs.Message = "Customer update successfully.";
                            rs.StatusCode = 201;
                        }
                    }
                    catch
                    {
                        return new ResponseDto(400, $"Customer update unsuccessful");
                    }

                }

                if (role != null && role == "Super-Admin")
                {
                    if (string.IsNullOrEmpty(request.CompanyId))
                    {
                       return new ResponseDto(500, "Company is required.");
                    }
                    try
                    {
                        var resultupdate = await identityService.UpdateCompanyOwner(customer);
                        
                        if (request.Storages != null && request.Storages.Any())
                        {
                            var rsUpdateStorage = await identityService.UpdateStoragesForUser(request.UserId, request.Storages, cancellationToken);
                            if (resultupdate.Succeeded && rsUpdateStorage.Succeeded)
                            {
                                return new ResponseDto(200, "Customer update successfully.");
                            }
                        }
                        if (resultupdate.Errors != null && resultupdate.Errors.Any())
                        {
                            rs.Message = string.Join(", ", resultupdate.Errors);
                            rs.StatusCode = 400;
                        } else
                        {
                            rs.Message = "Customer update successfully.";
                            rs.StatusCode = 201;
                        }
                        
                    } catch
                    {
                        return new ResponseDto(400, $"Customer update unsuccessful");
                    }
                }
            }
        } catch (Exception ex)
        {
            _fileService.DeleteFile(fileName);
            rs.Message = ex.Message;
        }
        return rs;
    }
}
