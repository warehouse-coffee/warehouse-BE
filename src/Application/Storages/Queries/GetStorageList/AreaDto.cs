using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.Storages.Queries.GetStorageList;

public class AreaDto
{
    public string? Name { get; set; }
    public List<ProductDto>? Products { get; set; }
}
