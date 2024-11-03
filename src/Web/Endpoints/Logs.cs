using Microsoft.AspNetCore.Mvc;
using warehouse_BE.Application.Logs.Queries.GetLogList;

namespace warehouse_BE.Web.Endpoints;

public class Logs : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapPost(GetLogs)
            ;
    }
    public Task<LogList> GetLogs(ISender sender, GetLogListQuery query)
    {
        return sender.Send(query);
    }
}