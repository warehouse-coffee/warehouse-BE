using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Categories.Commands.CreateCaterory;

public class CreateCategoryCommand : IRequest<ResponseDto>
{
    public required CategoryDto Category { get; set; }
}
public record CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    private readonly IIdentityService _identityService;
    public CreateCategoryCommandHandler(IApplicationDbContext context, IUser currentUser, IIdentityService identityService )
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
    }
    public async Task<ResponseDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Id != null)
        {
            try
            {
                var (result, companyId) = await _identityService.GetCompanyId(_currentUser.Id);
                var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == request.Category.Name && c.CompanyId == companyId, cancellationToken);
                if (existingCategory != null)
                {

                    return new ResponseDto(409, "Category with the same name already exists");
                }
                var categoryEntity = new Category
                {
                    Name = request.Category.Name,
                    CompanyId = companyId,
                };


                _context.Categories.Add(categoryEntity);

                await _context.SaveChangesAsync(cancellationToken);
                var data = new Categories.Queries.GetListCategory.CategoryDto
                {
                    Id = categoryEntity.Id,
                    Name = request.Category.Name,
                    CompanyId = companyId,
                };
                return new ResponseDto(201, "Category created successfully", data);
            }
            catch (Exception ex)
            {
                return new ResponseDto(500, $"An error occurred while creating the category: {ex.Message}");
            }
        }
        return new ResponseDto(400, $"Create Category fail");
    }
}