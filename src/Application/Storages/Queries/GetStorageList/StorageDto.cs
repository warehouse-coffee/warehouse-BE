using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Companies.Queries.GetCompanyList;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Storages.Queries.GetStorageList;

public class StorageDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Location { get; set; }
    public string? Status { get; set; }
    public List<AreaDto>? Areas { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Storage, StorageDto>().ForMember(dest => dest.Areas, opt => opt.MapFrom(src => src.Areas)); 
            CreateMap<Area, AreaDto>();
        }

    }
}