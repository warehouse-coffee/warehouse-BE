using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Models;

namespace warehouse_BE.Application.Inventories.Queries.GetInventoriesByStorage;

public class InventoryListVM
{
    public InventoryListVM()
    {
        Inventories = new List<InventoryDto>();
    }
    public Page? Page { get; set; }
    public List<InventoryDto>? Inventories { get; set; }
}
