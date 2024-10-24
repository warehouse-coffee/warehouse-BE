using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;
using warehouse_BE.Application.Storages.Queries.GetStorageList;
using warehouse_BE.Domain.Entities;
using warehouse_BE.Domain.Enums;

namespace warehouse_BE.Application.Storages.Commads.CreateStorage;

public record CreateStorageCommand : IRequest<ResponseDto>
{
    public required string Name { get; init; }
    public required string Location { get; init; }
    public required StorageStatus Status { get; init; }
    public List<AreaDto>? Areas { get; init; }

}

public class CreateStorageCommandHandler : IRequestHandler<CreateStorageCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;

        public CreateStorageCommandHandler(IApplicationDbContext context, IUser currentUser, IMapper mapper, IIdentityService identityService)
        {
            _context = context;
            _currentUser = currentUser;
            _mapper = mapper;
            _identityService = identityService;
        }

    public async Task<ResponseDto> Handle(CreateStorageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var companyId = string.Empty;
            if (_currentUser.Id != null)
            {
               var data = await _identityService.GetCompanyId(_currentUser.Id);
               companyId = data.CompanyId;
            }
            
            var areas = request.Areas != null && request.Areas.Any()
            ? _mapper.Map<List<Area>>(request.Areas)
            : new List<Area>();

            foreach (var area in areas)
            {
                foreach (var product in area.Products)
                {
                    if (product.Category != null)
                    {
                        var category = await _context.Categories
                       .FirstOrDefaultAsync(c => c.Name == product.Category.Name, cancellationToken);

                        if (category == null)
                        {
                            category = new Category
                            {
                                Name = product.Category.Name,
                                CompanyId = companyId,
                                Created = DateTimeOffset.UtcNow,
                                CreatedBy = _currentUser.Id,
                                LastModified = DateTimeOffset.UtcNow,
                                LastModifiedBy = _currentUser.Id
                            };

                            _context.Categories.Add(category);
                            await _context.SaveChangesAsync(cancellationToken);
                        }

                        product.CategoryId = category.Id;
                    }
                   
                }
            }
            var entity = new Storage
            {
                Name = request.Name,
                Location = request.Location,
                Status = request.Status,
                Areas = areas,
                CompanyId = companyId,
            };

            _context.Storages.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            var createdStorageDto = _mapper.Map<StorageDto>(entity); 

            return new ResponseDto(201, "Storage created successfully", createdStorageDto);
        }
        catch (Exception ex)
        {
            return new ResponseDto(500, $"An error occurred while creating the storage: {ex.Message}");
        }
    }
}
