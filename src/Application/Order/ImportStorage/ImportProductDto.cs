using warehouse_BE.Domain.Entities;
using warehouse_BE.Domain.Enums;

namespace warehouse_BE.Application.Order.ImportStorage;

public class ImportProductDto
{
    public string? Name { get; set; } 
    public int Quantity { get; set; } 
    public decimal Price { get; set; } 
    public ProductStatus Status { get; set; } 
    public DateTime ImportDate { get; set; }
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ImportProductDto, OrderDetail>()
                .ForMember(dest => dest.TotalPice, opt => opt.MapFrom(src => src.Price * src.Quantity));
        }
    }
}
