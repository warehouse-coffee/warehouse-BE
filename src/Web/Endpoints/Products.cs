using warehouse_BE.Application.Product.Queries.GetProductList;
using warehouse_BE.Application.Products.Queries.GetProductList;
using Microsoft.AspNetCore.Authorization;

namespace warehouse_BE.Web.Endpoints;

public class Products : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetProductList);
    }

    [Authorize(Roles = "Administrator")]
    public async Task<ProductListVM> GetProductList(ISender sender)
    {
        return await sender.Send(new GetProductListQuery());
    }
}

