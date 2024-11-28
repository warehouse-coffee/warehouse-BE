namespace warehouse_BE.Application.Products.Queries.GetProductOrder;

public class ProductsOrderVM
{
    public ProductsOrderVM()
    {
        Products = new List<ProductDto>();
    }
    public List<ProductDto>? Products { get; set; }
}
