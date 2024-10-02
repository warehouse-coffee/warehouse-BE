using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;
using warehouse_BE.Application.Storages.Queries.GetStorageList;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.CompanyOwner.Commands.UpdateCompanyOwner;

public class UpdateCompanyOwnerCommand : IRequest<ResponseDto>
{
    public required string UserId { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? CompanyId { get; set; }
    public List<StorageDto>? Storages { get; set; }
}
public class UpdateCompanyOwnerCommandHandler : IRequestHandler<UpdateCompanyOwnerCommand, ResponseDto>
{
    public readonly IIdentityService identityService;
    private readonly IUser _currentUser;
    private readonly IApplicationDbContext _context;

    public UpdateCompanyOwnerCommandHandler(IIdentityService identityService, IUser currentUser, IApplicationDbContext context)
    {
        this.identityService = identityService;
        this._currentUser = currentUser;
        _context = context;
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
            var customer = new UpdateCompanyOwner
            {
                UserId = request.UserId,
                UserName = request.UserName,
                Password = request.Password,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                CompanyId = request.CompanyId
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
        } catch (Exception ex)
        {
            rs.Message = ex.Message;
        }
        return rs;
    }
}
