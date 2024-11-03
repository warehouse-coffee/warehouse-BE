using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Orders.Queries.GetOrderList;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Storages.Queries.GetStorageProducts;

public class ProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Units { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string? Image { get; set; }
    public string Status { get; set; } = string.Empty; 
    public DateTime Expiration { get; set; }
    public DateTime ImportDate { get; set; }
    public DateTime ExportDate { get; set; }
    public int SafeStock { get; set; }
    public decimal TotalPrice { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Product, ProductDto>()
            ;

        }

    }
}
