using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.InventoriesOutbound.Commands.CreateInventoryOutbound;

public class InventoryOutboundVM
{
    public DateTime? OutboundDate { get; set; } 
    public DateTime? ExpectedOutboundDate { get; set; } 
    public string? OutboundBy { get; set; }
    public string? Status { get; set; } 
    public string? Remarks { get; set; }
    public int OrderId { get; set; }
    public List<OutboundDetailVM> OutboundDetails { get; set; } = new List<OutboundDetailVM>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<InventoryOutbound, InventoryOutboundVM>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())) 
                .ForMember(dest => dest.OutboundDate, opt => opt.MapFrom(src => src.OutboundDate)) 
                .ForMember(dest => dest.ExpectedOutboundDate, opt => opt.MapFrom(src => src.ExpectedOutboundDate)) 
                .ForMember(dest => dest.OutboundDetails, opt => opt.MapFrom(src => src.OutboundDetails.Select(detail => new OutboundDetailVM
                {
                    ProductId = detail.ProductId,
                    ProductName = detail.Product != null ? detail.Product.Name : "Unknown Product",
                    Quantity = detail.Quantity,
                    IsAvailable = detail.IsAvailable,
                    Note = detail.Note
                }).ToList()));
        }
    }
}
