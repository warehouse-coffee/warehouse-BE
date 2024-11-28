using warehouse_BE.Application.Products.Queries.GetProductList;
using Microsoft.AspNetCore.Authorization;
using warehouse_BE.Application.Products.Queries.GetProductOrder;

namespace warehouse_BE.Web.Endpoints;

public class Products : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(GetProductList)
            .MapGet(GetProductsOrder,"/product-order")
            ;
    }

    public async Task<ProductListVM> GetProductList(ISender sender, GetProductListQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<ProductsOrderVM> GetProductsOrder(ISender sender)
    {
        return await sender.Send(new GetProductsOrder());
    }
}

