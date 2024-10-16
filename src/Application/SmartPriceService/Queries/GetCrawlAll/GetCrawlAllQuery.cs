using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.SmartPriceService.Queries.GetCrawlAll;

public class GetCrawlAllQuery : IRequest<object>
{
}
public class GetCrawlAllQueryHandler : IRequestHandler<GetCrawlAllQuery, object>
{
    private readonly IExternalHttpService _externalHttpService;

    public GetCrawlAllQueryHandler(IExternalHttpService externalHttpService)
    {
        _externalHttpService = externalHttpService;
    }

    public async Task<object> Handle(GetCrawlAllQuery request, CancellationToken cancellationToken)
    {
        string url = "https://example.com/crawl_all";
        return await _externalHttpService.GetAsync<object>(url);
    }
}
