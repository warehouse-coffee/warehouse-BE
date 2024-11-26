using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Customers.Queries.GetListCustomerOfUser;

public class CustomerDto
{
    public int Id { get; set; }
    public string? Name { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Customer, CustomerDto>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name +" - "+ src.Phone));
        }

    }
}
