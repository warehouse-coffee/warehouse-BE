using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.Storages.Queries.GetStorageList;

public class ProductDto
{
    public string? Name { get; set; }
    public string? Units { get; set; }
    public int Amount { get; set; }
    public string? Image { get; set; }
    public string? Status { get; set; }  // 'Sold or Not Sold,...'
    public DateTime? Expiration { get; set; }
    public DateTime? ImportDate { get; set; }
    public DateTime? ExportDate { get; set; }

    // Relationships
    public int? CategoryId { get; set; }
    public int? AreaId { get; set; }

    private class Mapping : Profile
    {
        public Mapping() {
            CreateMap<warehouse_BE.Domain.Entities.Product, ProductDto>()
           .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
           .ForMember(dest => dest.Units, opt => opt.MapFrom(src => src.Units))
           .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
           .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
           .ForMember(dest => dest.Expiration, opt => opt.MapFrom(src => src.Expiration))
           .ForMember(dest => dest.ImportDate, opt => opt.MapFrom(src => src.ImportDate))
           .ForMember(dest => dest.ExportDate, opt => opt.MapFrom(src => src.ExportDate))
           .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
           .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.AreaId));

        }
    }
}

