using warehouse_BE.Application.Product.Queries.GetProductList;
using warehouse_BE.Application.Products.Queries.GetProductList;
//using warehouse_BE.Application.TodoLists.Queries.GetTodos;

namespace warehouse_BE.Web.Endpoints;

public class Products : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            //.RequireAuthorization()
            .MapGet(GetProductList);
    }

    public async Task<ProductListVM> GetProductList(ISender sender)
    {
        return await sender.Send(new GetProductListQuery());
    }
}

