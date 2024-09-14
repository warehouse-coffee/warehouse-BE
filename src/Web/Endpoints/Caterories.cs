using warehouse_BE.Application.Categories.Commands.CreateCaterory;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Web.Endpoints;

public class Categories : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateCategory);
            //.MapGet(GetCategoryList); 
    }


    public async Task<ResponseDto> CreateCategory(ISender sender, CreateCategoryCommand command)
    {
        return await sender.Send(command); 
    }


    //public async Task<CategoryListVM> GetCategoryList(ISender sender)
    //{
    //    return await sender.Send(new GetCategoryListQuery());
    //}
}