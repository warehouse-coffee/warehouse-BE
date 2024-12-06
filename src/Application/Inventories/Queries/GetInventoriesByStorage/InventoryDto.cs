

using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Inventories.Queries.GetInventoriesByStorage;

public class InventoryDto
{
    public int Id { get; set; }
    public required string ProductName { get; set; }
    public int AvailableQuantity { get; set; }
    public DateTime? Expiration { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal TotalSalePrice { get; set; }
    public int SafeStock { get; set; }
    public string Status { get; set; } = "NA";  // Default value

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Inventory, InventoryDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => GetStatus(src.AvailableQuantity, src.SafeStock)));
        }

        // Logic to determine status
        private string GetStatus(int availableQuantity, int safeStock)
        {
            if (availableQuantity == 0)
            {
                return "Out of Stock";
            }
            else if (availableQuantity < safeStock)
            {
                return "Low Stock";
            }
            else
            {
                return "In Stock";
            }
        }
    }
}