using warehouse_BE.Application.Orders.Queries.GetOrderDetail;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Products.Queries.GetProductOrder;

public class ProductDto
{
    public string? Name { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<IGrouping<string, Inventory>, ProductDto>()
          .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Key));
        }
    }
}
