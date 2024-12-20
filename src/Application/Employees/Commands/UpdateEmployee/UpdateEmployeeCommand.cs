﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Employee.Queries.GetListEmployee;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Application.Employee.Commands.UpdateEmployee;

public class UpdateEmployeeCommand : IRequest<ResponseDto>
{
    public required string Id { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public List<int>? Warehouses { get; set; }
}
public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, ResponseDto>
{
    private readonly IIdentityService _identityService;
    private readonly IUser _currentUser;

    public UpdateEmployeeCommandHandler(IIdentityService identityService, IUser currentUser)
    {
        _identityService = identityService;
        _currentUser = currentUser;
    }

    public async Task<ResponseDto> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
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
            var Employee = new UpdateEmployee
            {
                Id = request.Id,
                UserName = request.UserName,
                Password = request.Password,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Warehouses = request.Warehouses,
            };

            var rs = await _identityService.UpdateEmployee(Employee, cancellationToken);
            if (rs.Succeeded)
            {
                var employee = await _identityService.GetUserById(request.Id);
                var employeeDto = new EmployeeDto();
                if (employee != null) 
                {
                    
                     employeeDto = new EmployeeDto
                    {
                        Id = employee.Id,
                        CompanyId = employee.CompanyId,
                        UserName = employee.UserName,
                        Email = employee.Email,
                        PhoneNumber = employee.PhoneNumber,
                        isActived = employee.isActived,
                        AvatarImage = employee.AvatarImage
                    };
                }
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
