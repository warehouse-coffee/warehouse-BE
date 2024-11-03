using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.Orders.Commands.SaleOrder;

public class ReservationVM
{
    public string ProductName { get; set; } = default!;
    public int ReservedQuantity { get; set; }
    public DateTime ReservedDate { get; set; }
    public DateTime? ExpectedPickupDate { get; set; }
    public string Status { get; set; } = "Pending";
}
