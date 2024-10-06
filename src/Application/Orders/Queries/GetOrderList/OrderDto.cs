using warehouse_BE.Application.Storages.Queries.GetStorageList;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Orders.Queries.GetOrderList;

public class OrderDto
{
    public string OrderId { get; set; } = string.Empty; 
    public string Type { get; set; } = string.Empty; 
    public DateTime Date { get; set; } 
    public decimal TotalPrice { get; set; } 
    public int OrderDetailsCount { get; set; } 
    public int TotalQuantity { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Order, OrderDto>().ForMember(dest => dest.OrderDetailsCount, opt => opt.MapFrom(src => src.OrderDetails.Count)) 
                    .ForMember(dest => dest.TotalQuantity, opt => opt.MapFrom(src => src.OrderDetails.Sum(od => od.Quantity)))
            ;

        }

    }

}
