using warehouse_BE.Application.Areas.CreateArea;
using warehouse_BE.Application.Categories.Queries.GetListCategory;
using warehouse_BE.Application.Response;
using warehouse_BE.Application.SmartPriceService.Queries.GetAllLinks;
using warehouse_BE.Application.SmartPriceService.Queries.GetCrawl;
using warehouse_BE.Application.SmartPriceService.Queries.GetCrawlAll;

namespace warehouse_BE.Web.Endpoints;

public class Craw : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetLinks,"/links")
            .MapGet(GetCraws, "/craws")
            .MapGet(GetCrawsAll, "/crawsAll")
            ;
    }
    public  Task<CommodityLinks> GetLinks(ISender sender)
    {
        return  sender.Send(new GetAllLinksQuery());
    }
    public Task<CrawlResponse> GetCraws(ISender sender)
    {
        return sender.Send(new GetCrawlQuery());
    }
    public Task<ResponseDto> GetCrawsAll(ISender sender)
    {
        return sender.Send(new GetCrawlAllQuery());
    }
}


