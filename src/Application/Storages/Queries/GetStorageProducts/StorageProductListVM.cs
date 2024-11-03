using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Models;

namespace warehouse_BE.Application.Storages.Queries.GetStorageProducts;

public class StorageProductListVM
{
    public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    public Page Page { get; set; } = new Page();
}
