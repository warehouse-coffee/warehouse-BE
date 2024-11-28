using warehouse_BE.Application.Response;
using warehouse_BE.Application.Storages.Commads.CreateStorage;
using warehouse_BE.Application.Storages.Commads.DeleteStorage;
using warehouse_BE.Application.Storages.Commads.UpdateStorage;
using warehouse_BE.Application.Storages.Queries.GetListISorageInfoOfUser;
using warehouse_BE.Application.Storages.Queries.GetStorageDetailUpdate;
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
            .MapGet(GetListStorageInfoOfUser,"user-list")
            .MapPost(GetStorageProducts,"/products")
            .MapGet(GetStorageDetailUpdate,"/detail/{id}")
            .MapPut(UpdateStorage,"")
            .MapDelete(DeleteStorage,"{id}")
            ;
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
    public async Task<ListISorageInfoOfUserVM> GetListStorageInfoOfUser(ISender sender)
    {
        return await sender.Send(new GetListISorageInfoOfUserQuery());
    }
    public async Task<Application.Storages.Queries.GetStorageDetailUpdate.StorageDto> GetStorageDetailUpdate(ISender sender, int id)
    {
        var query = new GetStorageDetailUpdateQuery { StorageId = id };
        return await sender.Send(query);
    }
    public async Task<ResponseDto> UpdateStorage(ISender sender, UpdateStorageCommand command)
    {
        return await sender.Send(command);
    }
    public async Task<ResponseDto> DeleteStorage(ISender sender, int id)
    {
        var command = new DeleteStorageCommand { Id = id };
        return await sender.Send(command);
    }
}
