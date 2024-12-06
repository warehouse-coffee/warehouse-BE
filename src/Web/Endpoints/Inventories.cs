using warehouse_BE.Application.Areas.CreateArea;
using warehouse_BE.Application.Inventories.Commands.CountSafeStock;
using warehouse_BE.Application.Inventories.Commands.DeleteInventoryProduct;
using warehouse_BE.Application.Inventories.Commands.UpdateSafeStock;
using warehouse_BE.Application.Inventories.Queries.GetInventoriesByStorage;
using warehouse_BE.Application.Inventories.Queries.GetListProductOfInventory;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Web.Endpoints
{
    public class Inventories : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            app.MapGroup(this)
                .RequireAuthorization()
                .MapPost(GetInventoryProductList)
                .MapDelete(DeleteInventoryProduct,"{id}")
                .MapPost(GetInventoriesByStorage,"storageId")
                .MapGet(GetSafeStock,"safestock")
                .MapPut(UpdateSafeStock,"safestock")
                ;
        }
        public Task<InventoryProductsListVM> GetInventoryProductList(ISender sender, GetListProductOfInventory query)
        {
            return sender.Send(query);
        }
        public Task<ResponseDto> DeleteInventoryProduct(ISender sender,int id)
        {
            return sender.Send(new DeleteInventoryProductCommand { Id = id });
        }
        public Task<InventoryListVM> GetInventoriesByStorage(ISender sender, GetInventoriesByStorageQuery query)
        {
            return sender.Send(query);
        }
        public Task<int> GetSafeStock(ISender sender, int id)
        {
            return sender.Send(new CountSafeStockCommand { Id = id });
        }
        public Task<bool> UpdateSafeStock(ISender sender, UpdateSafeStockCommand command)
        {
            return sender.Send(command);
        }
    }
}
