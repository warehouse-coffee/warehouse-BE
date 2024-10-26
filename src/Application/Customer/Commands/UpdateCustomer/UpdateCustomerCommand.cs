using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Application.Customer.Commands.UpdateCustomer
{
    public class UpdateCustomerCommand : IRequest<ResponseDto>
    {
        public required string Id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActived { get; set; }
        public IFormFile? AvatarImage { get; set; }
        public List<string>? Warehouses { get; set; }
    }
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, ResponseDto>
    {
        private readonly IIdentityService _identityService;
        private readonly IUser _currentUser;

        public UpdateCustomerCommandHandler(IIdentityService identityService, IUser currentUser)
        {
            _identityService = identityService;
            _currentUser = currentUser;
        }

        public async Task<ResponseDto> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
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
                var customer = new UpdateCustomer
                {
                    CustomerId = request.Id,
                    UserName = request.UserName,
                    Password = request.Password,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    IsActived = request.IsActived,
                    AvatarImage = "request.AvatarImage",
                    Warehouses = request.Warehouses,

                };

                var rs = await _identityService.UpdateCustomer(customer);
                if (rs.Succeeded)
                {
                    return new ResponseDto(200, "Customer created successfully.");
                }
                var errorMessages = string.Join(", ", rs.Errors);
                return new ResponseDto(400, $"Customer creation unsuccessful. Errors: {errorMessages}");
            }
            catch (Exception ex)
            {
                return new ResponseDto(500, $"An error occurred while creating the User: {ex.Message}");
            }
        }
    }
}
