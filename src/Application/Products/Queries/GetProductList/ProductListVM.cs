using warehouse_BE.Application.Common.Models;
using warehouse_BE.Application.Products.Queries.GetProductList;

namespace warehouse_BE.Application.Products.Queries.GetProductList;

public class ProductListVM
{
    public ProductListVM()
    {
        ProductList = new List<ProductDto> { new ProductDto() };
    }
    public List<ProductDto>? ProductList { get; set; }
    public Page? Page { get; set; }
}
