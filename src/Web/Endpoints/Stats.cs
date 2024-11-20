using warehouse_BE.Application.Stats.Queries.Admin;
using warehouse_BE.Application.Stats.Queries.Employee;

namespace warehouse_BE.Web.Endpoints
{
    public class Stats : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            app.MapGroup(this)
                .RequireAuthorization()
                .MapGet(GetAdminStats,"admin")
                .MapGet(GetEmployeeStats, "employee");
        }

        public Task<AdminStatsVM> GetAdminStats(ISender sender)
        {
            return sender.Send(new AdminStatsQuery());
        }

        public Task<EmployeeStatsVM> GetEmployeeStats(ISender sender)
        {
            return sender.Send(new EmployeeStatsQuery());
        }
    }
}
