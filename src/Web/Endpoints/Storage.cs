using warehouse_BE.Application.Response;
using warehouse_BE.Application.Storages.Commads.CreateStorage;
using warehouse_BE.Application.Storages.Queries.GetStorageList;
using warehouse_BE.Application.Storages.Queries.GetStorageOfUser;
using warehouse_BE.Application.Storages.Queries.GetStorageProducts;

namespace warehouse_BE.Web.Endpoints;

public class Storage : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateStorage)
            .MapGet(GetStorageList)
            .MapPost(GetStorageOfUser,"/user")
            .MapPost(GetStorageProducts,"/products");
    }

    public async Task<ResponseDto> CreateStorage(ISender sender, CreateStorageCommand command)
    {
        return await sender.Send(command);
    }

    public async Task<StorageListVM> GetStorageList(ISender sender)
    {
        return await sender.Send(new GetStorageListQuery());
    }
    public async Task<UserStorageList> GetStorageOfUser(ISender sender, GetStorageOfUserQuery query)
    {
        return await sender.Send(query);
    }
    public async Task<StorageProductListVM> GetStorageProducts(ISender sender, GetStorageProductsQuery query)
    {
        return await sender.Send(query);
    }
}
