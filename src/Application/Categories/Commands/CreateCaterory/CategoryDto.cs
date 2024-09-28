using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Products.Queries.GetProductList;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Categories.Commands.CreateCaterory;

public class CategoryDto
{
    public string? Name { get; set; }
    public List<ProductDto>? Products { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CategoryDto, Category>().ReverseMap();
        }
    }
}
