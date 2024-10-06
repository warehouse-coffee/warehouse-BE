using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Orders.Queries.GetOrderDetail;

public class OrderProductDto 
{
    public string? ProductName { get; set; } 
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Units { get; set; } 
    public string? Note { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<OrderDetail, OrderProductDto>()
               .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
               .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
               .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
               .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
               .ForMember(dest => dest.Units, opt => opt.MapFrom(src => src.Product.Units)); 
            ;
        }
    }

}
