﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Storages.Queries.GetStorageList;

public class AreaDto
{
    public int Id { get; set; }
    public string? Name { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Area, AreaDto>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForSourceMember(src => src.Products, opt => opt.DoNotValidate())
               .ReverseMap();
            CreateMap<AreaDto, Area>().ReverseMap();

        }
    }
}
