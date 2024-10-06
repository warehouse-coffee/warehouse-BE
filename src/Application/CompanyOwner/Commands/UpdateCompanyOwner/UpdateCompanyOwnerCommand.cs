using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;
using warehouse_BE.Application.Storages.Queries.GetStorageList;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting; 

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
    public UpdateCompanyOwnerCommandHandler(IIdentityService identityService, IUser currentUser, IApplicationDbContext context, IWebHostEnvironment webHost)
    {
        this.identityService = identityService;
        this._currentUser = currentUser;
        this._context = context;
        this._webHostEnvironment = webHost;
    }
    public async Task<ResponseDto> Handle(UpdateCompanyOwnerCommand request, CancellationToken cancellationToken)
    {
        var rs = new ResponseDto { 
            StatusCode= 400,
            Message = "",
            Data="",
        };
        try
        {
            if (string.IsNullOrEmpty(_currentUser?.Id))
            {
                return new ResponseDto(500, "An error occurred: User ID is null or empty.");
            }
            var role = await identityService.GetRoleNamebyUserId(_currentUser.Id);
            string errorMessages;
            if (request.UserId != null)
            {
                string filePath = "";
                if (request.AvatarImage != null)
                {
                    var userId = request.UserId;
                    var uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "image");
                    var fileName = $"{userId}{Path.GetExtension(request.AvatarImage.FileName)}"; 
                     filePath = Path.Combine(uploadsFolder, fileName);

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.AvatarImage.CopyToAsync(stream);
                    }
                }
                var customer = new UpdateCompanyOwner
                {
                    UserId = request.UserId,
                    UserName = request.UserName,
                    Password = request.Password,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    CompanyId = request.CompanyId,
                    AvatarImage = filePath
                };
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
                            return new ResponseDto(200, "Customer created successfully.");
                        }
                    }

                    errorMessages = string.Join(", ", resultupdate.Errors);
                }
                catch
                {
                    return new ResponseDto(400, $"Customer creation unsuccessful");
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
                            return new ResponseDto(200, "Customer created successfully.");
                        }
                    }
                   
                    errorMessages = string.Join(", ", resultupdate.Errors);
                } catch
                {
                    return new ResponseDto(400, $"Customer creation unsuccessful");
                }
                }
            }
        } catch (Exception ex)
        {
            rs.Message = ex.Message;
        }
        return rs;
    }
}
