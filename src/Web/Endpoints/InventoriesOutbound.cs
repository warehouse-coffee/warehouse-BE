using warehouse_BE.Application.Categories.Commands.CreateCaterory;
using warehouse_BE.Application.InventoriesOutbound.Commands.CreateInventoryOutbound;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Web.Endpoints
{
    public class InventoriesOutbound : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            app.MapGroup(this)
           .RequireAuthorization()
           .MapPost(CreateInventoryOutbound)
           ;
        }

        public async Task<ResponseDto> CreateInventoryOutbound(ISender sender, CreateInventoryOutboundCommand command)
        {
            return await sender.Send(command);
        }
    }
}
