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
    private readonly IMapper _mapper;
    public CreateCategoryCommandHandler(IApplicationDbContext context, IUser currentUser, IMapper mapper)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
    }
    public async Task<ResponseDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == request.Category.Name, cancellationToken);
            if (existingCategory != null)
            {

                return new ResponseDto(409, "Category with the same name already exists");
            }
            var categoryEntity = new Category
            {
                Name = request.Category.Name,
            };
            if (request.Category.Products != null && request.Category.Products.Any())
            {
                var products = _mapper.Map<List<Product>>(request.Category.Products);

                // Gán CategoryId cho từng sản phẩm
                foreach (var product in products)
                {
                    product.Category = categoryEntity; // Gán trực tiếp categoryEntity cho mỗi sản phẩm
                }

                // Thêm danh sách sản phẩm vào Category
                categoryEntity.Products = products;

            }

            _context.Categories.Add(categoryEntity);

            // Lưu thay đổi vào database
            await _context.SaveChangesAsync(cancellationToken);

            return new ResponseDto(201, "Category created successfully");
        }
        catch (Exception ex)
        {
            return new ResponseDto(500, $"An error occurred while creating the category: {ex.Message}");
        }
    }
}