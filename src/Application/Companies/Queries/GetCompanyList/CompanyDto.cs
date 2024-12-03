using warehouse_BE.Application.Common.Mappings;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Companies.Queries.GetCompanyList;

public class CompanyDto 
{
    public int Id { get; set; }
    public string? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public string? PhoneContact { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Company, CompanyDto>();
        }
       
    }
}
