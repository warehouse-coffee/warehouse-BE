using warehouse_BE.Application.Stats.Queries.Admin;

namespace warehouse_BE.Web.Endpoints
{
    public class Stats : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            app.MapGroup(this)
                .RequireAuthorization()
                .MapGet(GetAdminStats,"admin");
        }

        public Task<AdminStatsVM> GetAdminStats(ISender sender)
        {
            return sender.Send(new AdminStatsQuery());
        }
    }
}
