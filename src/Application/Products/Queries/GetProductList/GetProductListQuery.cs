using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Products.Queries.GetProductList;


namespace warehouse_BE.Application.Products.Queries.GetProductList;

public class GetProductListQuery : IRequest<ProductListVM>
{

}

public class GetPrpductListQueryhandler : IRequestHandler<GetProductListQuery, ProductListVM>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetPrpductListQueryhandler (IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ProductListVM> Handle(GetProductListQuery request, CancellationToken cancellationToken)
    {
        var products = await _context.Products
                                 .ToListAsync(cancellationToken);

        // Sử dụng AutoMapper để chuyển đổi List<Product> sang List<ProductDto>
        var productDtos = _mapper.Map<List<ProductDto>>(products);

        // Tạo một đối tượng ProductListVM và gán ProductList
        var productListVM = new ProductListVM
        {
            ProductList = productDtos
        };

        return productListVM;
    }
}
