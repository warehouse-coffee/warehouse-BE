using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.InventoriesOutbound.Commands.CreateInventoryOutbound;

public class OutboundDetailVM
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; } 
    public int Quantity { get; set; }
    public bool IsAvailable { get; set; }
    public string? Note { get; set; }
}
