using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Application.Employee.Commands.CreateEmployee
{
    public class CreateEmployeeCommand : IRequest<ResponseDto>
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public List<int>? Warehouses { get; set; }
    }
    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand,ResponseDto>
    {
        private readonly IIdentityService _identityService;
        private readonly IUser _currentUser;
        public CreateEmployeeCommandHandler(IIdentityService identityService, IUser currentUser)
        {
            _identityService = identityService;
            _currentUser = currentUser;
        }

        public async Task<ResponseDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
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
                var Employee = new EmployeeRequest {
                    UserName = request.UserName,
                    Email = request.Email,
                    Password = request.Password,
                    PhoneNumber = request.PhoneNumber,
                    CompanyId = companyIdResult.CompanyId,
                    Warehouses = request.Warehouses,
                };

                var (rs,employeeDto) = await _identityService.CreateEmployee(Employee);
                if (rs.Succeeded)
                {
                    return new ResponseDto(200, "Employee created successfully.", employeeDto);
                }
                var errorMessages = string.Join(", ", rs.Errors);
                return new ResponseDto(400, $"Employee creation unsuccessful. Errors: {errorMessages}");
            }
            catch (Exception ex)
            {
                return new ResponseDto(500, $"An error occurred while creating the Employee: {ex.Message}");
            }
        }
    }
}
