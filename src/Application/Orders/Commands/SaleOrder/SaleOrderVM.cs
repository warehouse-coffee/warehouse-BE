using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Orders.Commands.SaleOrder;

public class SaleOrderVM
{
    public string OrderId { get; set; } = default!;
    public string Type { get; set; } = "Sale";
    public DateTime Date { get; set; }
    public decimal TotalPrice { get; set; }
    public List<ReservationVM> Reservations { get; set; } = new List<ReservationVM>();
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, SaleOrderVM>()
            .ForMember(dest => dest.Reservations, opt => opt.MapFrom(src => src.Reservations));

            CreateMap<Reservation, ReservationVM>()
           .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Inventory.ProductName));

        }
    }

}
