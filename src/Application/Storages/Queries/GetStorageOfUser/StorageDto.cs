using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Companies.Queries.GetCompanyList;
using warehouse_BE.Application.Storages.Commads.UpdateStorage;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Storages.Queries.GetStorageOfUser;

public class StorageDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Status { get; set; }
    public List<AreaDto>? Areas { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Storage, StorageDto>()
             .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
             .ForMember(dest => dest.Areas, opt => opt.MapFrom(src => src.Areas != null
                ? src.Areas.Select(o => new AreaDto
                {
                    Id = o.Id,
                    Name = o.Name
                }).ToList()
                : new List<AreaDto>()));

        }

    }
}
