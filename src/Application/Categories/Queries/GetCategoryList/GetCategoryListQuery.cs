using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Domain.Common;

namespace warehouse_BE.Application.Categories.Queries.GetListCategory;

public class GetCategoryListQuery : IRequest<CategoryListVM>
{
}
public class GetListCategoryQueryHandler : IRequestHandler<GetCategoryListQuery, CategoryListVM>
{
    private readonly IUser _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;    
    public GetListCategoryQueryHandler(IUser currentUser, IIdentityService identityService, IApplicationDbContext context, IMapper mapper)
    {
        _currentUser = currentUser;
        _identityService = identityService;
        _context = context;
        _mapper = mapper;
    }
    public async Task<CategoryListVM> Handle(GetCategoryListQuery request, CancellationToken cancellationToken)
    {
        var rs = new CategoryListVM();
        string role = string.Empty;
        UserDto? user = null;

        if (!string.IsNullOrEmpty(_currentUser?.Id))
        {
            user = await _identityService.GetUserById(_currentUser.Id);

            if (user != null)
            {
                role = user.RoleName ?? string.Empty; 
            }
        }

        if (user?.CompanyId != null)
        {
            switch (role)
            {
                case "Super-Admin":
                    rs.Categories = await _context.Categories
                                     .Select(c => new CategoryDto
                                     {
                                         Id = c.Id,
                                         Name = c.Name,
                                         CompanyId = c.CompanyId
                                     })
                                     .ToListAsync(cancellationToken);

                    break;

                case "Admin":
                    rs.Categories = await _context.Categories
                                     .Where(c => c.CompanyId == user.CompanyId)
                                     .Select(c => new CategoryDto
                                     {
                                         Id = c.Id,
                                         Name = c.Name,
                                         CompanyId = c.CompanyId
                                     })
                                     .ToListAsync(cancellationToken);
                    break;

                case "Customer":
                    var customerStorageIds = user.Storages?.Select(s => s.Id).ToList() ?? new List<int>();

                    rs.Categories = await _context.Categories
                                                  .Where(c => c.Products.Any(p => customerStorageIds.Contains(p.StorageId)))
                                                  .Select(c => new CategoryDto
                                                  {
                                                      Id = c.Id,
                                                      Name = c.Name,
                                                      CompanyId = c.CompanyId
                                                  })
                                                  .ToListAsync(cancellationToken);
                    break;
                default:
                    throw new InvalidOperationException($"Role '{role}' is not recognized.");

            }
        }

        return rs;
    }

}

