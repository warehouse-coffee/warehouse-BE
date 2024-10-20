using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.Categories.Queries.GetListCategory;

public class CategoryDto
{
    public int Id { get; set; }
    public  string? Name { get; set; }
    public string? CompanyId { get; set; }
}
