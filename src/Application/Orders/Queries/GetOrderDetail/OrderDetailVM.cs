using warehouse_BE.Application.Orders.Queries.GetOrderList;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Orders.Queries.GetOrderDetail;

public class OrderDetailVM
{
    public string? Type { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderProductDto>? OrderProductDtos { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Order, OrderDetailVM>()
                .ForMember(dest => dest.OrderProductDtos, opt => opt.MapFrom(src => src.OrderDetails))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.OrderDetails.Sum(od => od.TotalPrice)));
        }
    }
}
