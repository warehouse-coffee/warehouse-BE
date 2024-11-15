
using System.Linq;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.LLM.Queries.GetResponseIntent;

public class GetResponseIntentQuery : IRequest<ResponseDto>
{
    public required string Itent {  get; set; }
}
public class GetResponseIntentQueryHandler : IRequestHandler<GetResponseIntentQuery, ResponseDto>
{
    private  readonly IApplicationDbContext context;
    private readonly ILoggerService logger;
    private readonly IIdentityService identityService;
    private readonly IUser user;

    public GetResponseIntentQueryHandler(IApplicationDbContext context, ILoggerService logger, IIdentityService identityService, IUser user)
    {
        this.context = context;
        this.logger = logger;
        this.identityService = identityService;
        this.user = user;
    }
    public async Task<ResponseDto> Handle(GetResponseIntentQuery request, CancellationToken cancellationToken)
    {
        var  result = new ResponseDto();
        if (user.Id != null)
        {
            var userStorages = await identityService.GetUserStoragesAsync(user.Id);
            var userStorageIds = userStorages.Select(storage => storage.Id).ToList();
            try
            {
                switch (request.Itent.ToLower())
                {
                    case "empty_storage":
                        var emptyStorages = await context.Storages
                                            .Where(storage => storage.Inventories.All(inventory => inventory.TotalQuantity == 0 && userStorageIds.Contains(inventory.StorageId)))
                                            .Select(o => new { o.Id, o.Name })
                                            .ToListAsync();

                        result.Data = emptyStorages;
                        break;

                    case "empty_area":
                        result.Message = "Response for empty area";
                        var productsWithZeroQuantity = await context.Products
                                                             .Where(product => product.Quantity == 0 && userStorageIds.Contains(product.StorageId))
                                                             .Select(product => new
                                                             {
                                                                 Name = product.Area != null ? product.Area.Name : "N/A",
                                                                 Storage_Name = product.Storage != null ? product.Storage.Name : "N/A",
                                                             })
                                                             .Distinct()
                                                             .ToListAsync();
                        var emptyProductAreas = await context.Storages
                                .Where(storage => userStorageIds.Contains(storage.Id))
                                .Select(storage => new
                                {
                                    StorageId = storage.Id,
                                    Areas = storage.Areas
                                })
                                .ToListAsync();
                        var emptyArea = emptyProductAreas
                                             .SelectMany(storage => storage.Areas ?? new List<Area>(), (storage, area) => new { storage, area })
                                             .Where(item => item.area.Products != null && !item.area.Products.Any())
                                             .Select(item => new
                                             {
                                                 Name = item.area.Name,
                                                 Storage_Name = context.Storages
                                                     .Where(s => s.Id == item.storage.StorageId)
                                                     .Select(s => s.Name)
                                                     .FirstOrDefault() ?? "N/A"
                                             })
                                             .ToList();

                        var combinedResults = productsWithZeroQuantity
                                                       .Concat(emptyArea) 
                                                       .Distinct()  
                                                       .ToList();
                        result.Data = combinedResults;
                        break;

                    case "coffee_price_today":
                        result.Message = "Response for coffee price today";
                        break;

                    case "near_expired_product":
                        result.Message = "The list of products near expiration within the next month.";
                        var currentDate = DateTime.UtcNow;
                        var oneMonthLater = currentDate.AddMonths(1);

                        var nearExpiredProducts = context.Products
                                                         .Where(product => product.Expiration <= oneMonthLater && product.Expiration >= currentDate && userStorageIds.Contains(product.StorageId))
                                                         .Select(product => new
                                                         {
                                                             ProductName = product.Name, 
                                                             product.Expiration,
                                                             product.Quantity,
                                                             product.StorageId,
                                                             AreaName = product.Area != null ? product.Area.Name : "N/A",
                                                         })
                                                         .ToList();

                        result.Data = nearExpiredProducts;
                        break;

                    case "most_valuable_product":
                        var mostValuableProduct = context.Inventories.Where(o => userStorageIds.Contains(o.StorageId))
                                     .OrderByDescending(inventory => inventory.TotalPrice) 
                                     .FirstOrDefault(); 

                        if (mostValuableProduct != null)
                        {
                            result.Message = $"Most valuable product: {mostValuableProduct.ProductName}";
                        }
                        else
                        {
                            result.Message = "No products found.";
                        }
                        result.Data = mostValuableProduct;
                        break;

                    case "coffee_price_tomorrow":
                        result.Message = "Response for coffee price tomorrow";
                        break;

                    case "others":
                        result.Message = "Response for others";
                        break;

                    default:
                        result.Message = "Unknown intent";
                        break;
                }

            }
            catch (Exception ex)
            {
                logger.LogError("Error at LLM get Instent", ex);
            }
        }
        return result;
    }
}
