using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.Categories.Queries.GetListCategory;

public class CategoryListVM
{
    public CategoryListVM()
    {
        this.Categories = new List<CategoryDto> { };
    }
    public List<CategoryDto> Categories { get; set; }
}
