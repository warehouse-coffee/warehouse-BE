using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Companies.Queries.GetCompanyList;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Storages.Queries.GetStoragebyCompanyId;

public class StorageDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Status { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Storage, StorageDto>()
             .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())); 

        }

    }
}
