using warehouse_BE.Application.Configurations.Commands.CreateConfig;
using warehouse_BE.Application.Configurations.Queries.GetAllConfig;
using warehouse_BE.Application.Logs.Queries.GetLogList;
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
                .MapGet(GetAllConfig)
                ;
        }
        public Task<ResponseDto> CreateConfig (ISender sender, CreateConfigCommand command)
        {
            return sender.Send(command);
        }
        public async Task<ConfigVm> GetAllConfig(ISender sender)
        {
            return await sender.Send(new GetAllConfigQuery());
        }
    }
}
