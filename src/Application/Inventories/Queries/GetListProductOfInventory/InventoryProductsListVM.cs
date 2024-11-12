
using warehouse_BE.Application.Common.Models;

namespace warehouse_BE.Application.Inventories.Queries.GetListProductOfInventory;

public class InventoryProductsListVM
{
    public InventoryProductsListVM()
    {
        Products = new List<InventoryProductsDto> { new InventoryProductsDto() };
    }

    public List<InventoryProductsDto>? Products { get; set; }
    public Page? Page { get; set; }
}
