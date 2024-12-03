using warehouse_BE.Application.Categories.Commands.CreateCaterory;
using warehouse_BE.Application.Categories.Commands.DeleteCaterogy;
using warehouse_BE.Application.Categories.Queries.GetListCategory;
using warehouse_BE.Application.Companies.Commands.DeleteCompany;
using warehouse_BE.Application.Companies.Commands.UpdateCompany;
using warehouse_BE.Application.Companies.Queries.CompanyDetail;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Web.Endpoints;

public class Categories : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateCategory)
            .MapGet(GetCategoryList)
            .MapDelete(DeleteCategory,"{id}")
        ;
    }


    public async Task<ResponseDto> CreateCategory(ISender sender, CreateCategoryCommand command)
    {
        return await sender.Send(command); 
    }

    public async Task<CategoryListVM> GetCategoryList(ISender sender)
    {
        return await sender.Send(new GetCategoryListQuery());
    }
    public async Task<bool> DeleteCategory(ISender sender,int id)
    {
        return await sender.Send(new DeleteCaterogyCommand { Id = id});
    }

}