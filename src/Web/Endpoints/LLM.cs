namespace warehouse_BE.Web.Endpoints
{
    public class LLM : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            app.MapGroup(this)
                .RequireAuthorization()

                ;
        }

    }
}
