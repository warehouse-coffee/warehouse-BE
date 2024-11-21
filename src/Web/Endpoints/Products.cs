using warehouse_BE.Application.Products.Queries.GetProductList;
using Microsoft.AspNetCore.Authorization;

namespace warehouse_BE.Web.Endpoints;

public class Products : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(GetProductList);
    }

    public async Task<ProductListVM> GetProductList(ISender sender, GetProductListQuery query)
    {
        return await sender.Send(query);
    }
}

