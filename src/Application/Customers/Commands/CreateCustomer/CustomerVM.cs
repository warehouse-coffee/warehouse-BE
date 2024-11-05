using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Customers.Commands.CreateCustomer;

public class CustomerVM
{
    public required string Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? CompanyName { get; set; } 

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Customer, CustomerVM>()
                .ForMember(dest => dest.CompanyName, opt =>
                    opt.MapFrom(src => src.Company != null ? src.Company.CompanyName : string.Empty)); 
        }
    }
}

