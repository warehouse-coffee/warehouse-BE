using warehouse_BE.Application.Configurations.Commands.CreateConfig;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Web.Endpoints
{
    public class Configurations : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            app.MapGroup(this)
                .RequireAuthorization()
                .MapPost(CreateConfig)

                ;
        }
        public Task<ResponseDto> CreateConfig (ISender sender, CreateConfigCommand command)
        {
            return sender.Send(command);
        }
    }
}
