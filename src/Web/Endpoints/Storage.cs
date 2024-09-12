using warehouse_BE.Application.Response;
using warehouse_BE.Application.Storages.Commads.CreateStorage;
using warehouse_BE.Application.Storages.Queries.GetStorageList;

namespace warehouse_BE.Web.Endpoints;

public class Storage : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateStorage)
            .MapGet(GetStorageList);
    }

    public async Task<ResponseDto> CreateStorage(ISender sender, CreateStorageCommand command)
    {
        return await sender.Send(command);
    }

    public async Task<StorageListVM> GetStorageList(ISender sender)
    {
        return await sender.Send(new GetStorageListQuery());
    }
}
