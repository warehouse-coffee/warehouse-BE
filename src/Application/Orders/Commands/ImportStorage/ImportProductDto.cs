using warehouse_BE.Domain.Entities;
using warehouse_BE.Domain.Enums;

namespace warehouse_BE.Application.Orders.Commands.ImportStorage;

public class ImportProductDto
{
    public string? Name { get; set; }
    public required string Unit { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string? Note { get; set; }
    public required DateTime Expiration { get; set; }
    public required int CategoryId { get; set; }
    public required int AreaId { get; set; }
    public required int StorageId { get; set; }
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ImportProductDto, OrderDetail>()
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Price * src.Quantity));
        }
    }
}
