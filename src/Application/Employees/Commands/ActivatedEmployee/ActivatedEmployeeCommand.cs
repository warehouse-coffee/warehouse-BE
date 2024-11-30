using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Application.Employees.Commands.ActivatedEmployee;

public class ActivatedEmployeeCommand : IRequest<ResponseDto>
{
    public required string UserId { get; set; }
    public bool IsActive { get; set; }
}
public class ActivatedEmployeeCommandHandler : IRequestHandler<ActivatedEmployeeCommand, ResponseDto>
{
    private readonly IIdentityService _identityService;
    private readonly ILoggerService _logger;

    public ActivatedEmployeeCommandHandler(IIdentityService identityService, ILoggerService logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<ResponseDto> Handle(ActivatedEmployeeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            
            var result = await _identityService.ActivateAsync(request.UserId, request.IsActive);

            if (result.Succeeded)
            {
                _logger.LogInformation($"User with ID {request.UserId} has been {(request.IsActive ? "activated" : "deactivated")}.");
                return new ResponseDto(200, "Employee status updated successfully.");
            }

            _logger.LogWarning($"Failed to activate/deactivate user with ID {request.UserId}. Errors: {string.Join(", ", result.Errors)}");
            return new ResponseDto(400, "Failed to update employee status.", result.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while activating/deactivating user with ID {request.UserId}: ",ex);
            return new ResponseDto(500, "An error occurred while updating the employee's status.");
        }
    }
}