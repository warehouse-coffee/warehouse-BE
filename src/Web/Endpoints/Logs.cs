using Microsoft.AspNetCore.Mvc;
using warehouse_BE.Application.Logs.Queries.GetLogList;

namespace warehouse_BE.Web.Endpoints;

public class Logs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetLogs)
            ;
    }
    public Task<LogList> GetLogs(ISender sender,[FromQuery] string? date, [FromQuery] string? typelog = null, [FromQuery] int? hour = null)
    {
        var query = new GetLogListQuery
        {
            Date = date,
            Hour = hour,
            TypeLog = typelog
        };

        return sender.Send(query);
    }
}