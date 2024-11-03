using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Common.Models;

namespace warehouse_BE.Application.Storages.Queries.GetStorageProducts;

public class GetStorageProductsQuery : IRequest<StorageProductListVM>
{
    public int StorageId { get; set; }
    public Page? Page { get; set; }
    public string? SearchText { get; set; }
    public List<FilterData>? FilterData { get; set; }
}
public class GetStorageProductsQueryHandler : IRequestHandler<GetStorageProductsQuery, StorageProductListVM>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILoggerService _loggerService;

    public GetStorageProductsQueryHandler(IApplicationDbContext context, IMapper mapper, ILoggerService loggerService)
    {
        _context = context;
        _mapper = mapper;
        _loggerService = loggerService;
    }

    public async Task<StorageProductListVM> Handle(GetStorageProductsQuery request, CancellationToken cancellationToken)
    {
        var result = new StorageProductListVM();
        try
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Storage)
                .Where(p => p.StorageId == request.StorageId && !p.IsDeleted); 

                // search by Name Product
                if (!string.IsNullOrEmpty(request.SearchText))
            {
                query = query.Where(p => p.Name.Contains(request.SearchText));
            }

            // Apply sort
            if (request.FilterData != null)
            {
                foreach (var filter in request.FilterData)
                {
                    switch (filter.Prop)
                    {
                        case "Units":
                            if (filter.Filter == "Equals" && filter.Type == "string")
                            {
                                query = query.Where(p => p.Units == filter.Value);
                            }
                            break;

                        case "Status":
                            if (filter.Filter == "Equals" && filter.Type == "string")
                            {
                                query = query.Where(p => p.Status.ToString() == filter.Value);
                            }
                            break;

                        case "Expiration":
                            if (filter.Filter == "GreaterThan" && filter.Type == "date" && DateTime.TryParse(filter.Value, out var expirationDate))
                            {
                                query = query.Where(p => p.Expiration > expirationDate);
                            }
                            else if (filter.Filter == "LessThan" && filter.Type == "date" && DateTime.TryParse(filter.Value, out expirationDate))
                            {
                                query = query.Where(p => p.Expiration < expirationDate);
                            }
                            break;

                        case "ImportDate":
                            if (filter.Filter == "GreaterThan" && filter.Type == "date" && DateTime.TryParse(filter.Value, out var importDate))
                            {
                                query = query.Where(p => p.ImportDate > importDate);
                            }
                            else if (filter.Filter == "LessThan" && filter.Type == "date" && DateTime.TryParse(filter.Value, out importDate))
                            {
                                query = query.Where(p => p.ImportDate < importDate);
                            }
                            break;
                    }
                }
            }

            // Page
            var totalElements = await query.CountAsync(cancellationToken);
            var products = await query
                .Skip((request.Page?.PageNumber - 1 ?? 0) * (request.Page?.Size ?? 1))
                .Take(request.Page?.Size ?? 10)
                .Select(p => _mapper.Map<ProductDto>(p))
                .ToListAsync(cancellationToken);

            if (products.Count > 0)
            {
                result.Products = products;
                result.Page = new Page
                {
                    Size = request.Page?.Size ?? 0,
                    TotalElements = totalElements,
                    PageNumber = request.Page?.PageNumber ?? 1
                };
            }
        }
        catch (Exception ex)
        {
            _loggerService.LogError("Error Get Storage Products : ", ex);
            result.Products = new List<ProductDto>();
            result.Page = new Page
            {
                Size = 0,
                TotalElements = 0,
                PageNumber = 1
            };
        }
        return result;
    }
}

