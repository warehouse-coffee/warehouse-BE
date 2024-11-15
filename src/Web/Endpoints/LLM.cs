using warehouse_BE.Application.LLM.Queries.GetResponseIntent;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Web.Endpoints
{
    public class LLM : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            app.MapGroup(this)
                .RequireAuthorization()
                .MapGet(GetResponseIntent,"intent")
                ;
        }

        public Task<ResponseDto> GetResponseIntent(ISender sender, string intent = "")
        {
            var request = new GetResponseIntentQuery { Itent = intent };
            return sender.Send(request);
        }

    }
}
