using warehouse_BE.Application.Areas.CreateArea;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Web.Endpoints;

public class Areas : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(CreateArea)

            ;
    }
    public Task<ResponseDto> CreateArea(ISender sender, CreateAreaCommand command)
    {
        return sender.Send(command);
    }
}
