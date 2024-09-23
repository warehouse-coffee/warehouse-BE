using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Application.Customer.Commands.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<ResponseDto>
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand,ResponseDto>
    {
        private readonly IIdentityService _identityService;
        private readonly IUser _currentUser;
        private readonly IApplicationDbContext _context;
        public CreateCustomerCommandHandler(IIdentityService identityService, IUser currentUser, IApplicationDbContext context)
        {
            _identityService = identityService;
            _currentUser = currentUser;
            _context = context;
        }

        public async Task<ResponseDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            try {
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
                var customer = new CustomerRequest {
                    UserName = request.UserName,
                    Email = request.Email,
                    Password = request.Password,
                    PhoneNumber = request.PhoneNumber,
                    CompanyId = companyIdResult.CompanyId,
                };

                var rs = await _identityService.CreateCustomer(customer);
                return new ResponseDto(200, "Customer created successfully.");
            }
            catch (Exception ex)
            {
                return new ResponseDto(500, $"An error occurred while creating the User: {ex.Message}");
            }
        }
    }
}
