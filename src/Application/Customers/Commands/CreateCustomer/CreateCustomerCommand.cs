using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Customers.Commands.CreateCustomer;

public class CreateCustomerCommand : IRequest<ResponseDto>
{
    public required string Name { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? CompanyName { get; init; }
}
public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    private readonly IMapper _mapper;

    public CreateCustomerCommandHandler(IApplicationDbContext context, IUser currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ResponseDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        Customer? createdCustomer = null; 
        Company? company = null;

        try
        {
            var existingCustomer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Name == request.Name && c.Phone == request.Phone, cancellationToken);

            if (existingCustomer != null)
            {
                return new ResponseDto(409, "Customer already exists.");
            }
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return new ResponseDto(400, "Customer name is required.");
            }

            if (!string.IsNullOrWhiteSpace(request.CompanyName))
            {
                company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.CompanyName == request.CompanyName, cancellationToken);

                if (company == null)
                {
                    company = new Company
                    {
                        CompanyName = request.CompanyName,
                        CompanyId = "USERCOMPANY"
                    };

                    _context.Companies.Add(company);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }

            createdCustomer = new Customer
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                CompanyId = company?.Id 
            };

            _context.Customers.Add(createdCustomer);
            await _context.SaveChangesAsync(cancellationToken);

            var createdCustomerDto = _mapper.Map<CustomerVM>(createdCustomer);
            return new ResponseDto(201, "Customer created successfully", createdCustomerDto);
        }
        catch (Exception ex)
        {
            if (createdCustomer != null)
            {
                _context.Customers.Remove(createdCustomer);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return new ResponseDto(500, $"An error occurred while creating the customer: {ex.Message}");
        }
    }
}
