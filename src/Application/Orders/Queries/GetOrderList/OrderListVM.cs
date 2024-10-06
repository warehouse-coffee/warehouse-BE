using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Models;

namespace warehouse_BE.Application.Orders.Queries.GetOrderList;

public class OrderListVM
{
    public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
    public Page Page { get; set; } = new Page();
}
