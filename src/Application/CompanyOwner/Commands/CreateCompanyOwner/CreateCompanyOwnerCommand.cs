using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Customer.Commands.CreateCustomer;
using warehouse_BE.Application.IdentityUser.Commands.CreateUser;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Application.CompanyOwner.Commands.CreateCompanyOwner;

public class CreateCompanyOwnerCommand : IRequest<ResponseDto>
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}
public class CreateCompanyOwnerCommandHandler : IRequestHandler<CreateCompanyOwnerCommand, ResponseDto>
{
    private readonly IIdentityService _identityService;
    private readonly IUser _currentUser;
    private readonly IApplicationDbContext _context;
    public CreateCompanyOwnerCommandHandler(IIdentityService identityService, IUser currentUser, IApplicationDbContext context)
    {
        _identityService = identityService;
        _currentUser = currentUser;
        _context = context;
    }
    public async Task<ResponseDto> Handle(CreateCompanyOwnerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(_currentUser?.Id))
            {
                return new ResponseDto(500, "An error occurred: User ID is null or empty.");
            }
            var companyIdResult = await _identityService.GetCompanyId(_currentUser.Id);
            if (!companyIdResult.Result.Succeeded)
            {
                return new ResponseDto(500, string.Join(", ", companyIdResult.Result.Errors));
            }
            if (string.IsNullOrEmpty(companyIdResult.CompanyId))
            {
                return new ResponseDto(500, "User is not associated with any company.");
            }
            var companyOwner = new UserRegister
            {
                UserName = request.UserName,
                Email = request.Email,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber,
                CompanyId = companyIdResult.CompanyId,
                RoleName = "Admin"
            };

            var rs = await _identityService.RegisterAsync(companyOwner);
            if (rs.Result.Succeeded)
            {
                return new ResponseDto(200, "Customer created successfully.");
            }
            var errorMessages = string.Join(", ", rs.Result.Errors);
            return new ResponseDto(400, $"Customer creation unsuccessful. Errors: {errorMessages}");
        }
        catch (Exception ex)
        {
            return new ResponseDto(500, $"An error occurred while creating the User: {ex.Message}");
        }
    }
}