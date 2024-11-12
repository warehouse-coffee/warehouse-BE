
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Inventories.Queries.GetListProductOfInventory;

public class InventoryProductsDto
{

    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public DateTime? Expiration { get; set; }
    public decimal TotalPrice { get; set; }
    public int SafeStock { get; set; }
    public int CategoryId { get; set; }
    public int StorageId { get; set; }
    public string StorageName { get; set; } = string.Empty;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Inventory, InventoryProductsDto>()
                .ForMember(d => d.StorageName, opt => opt.MapFrom(s => s.Storage != null ? s.Storage.Name : string.Empty));
        }
    }
}
