using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.SmartPriceService.Queries.GetCrawl;

public class GetCrawlQuery : IRequest<object>
{
    public string? query;
}
public class GetCrawlQueryHandler : IRequestHandler<GetCrawlQuery, object>
{
    private IExternalHttpService _externalHttpService;
    public GetCrawlQueryHandler(IExternalHttpService externalHttpService)
    {
        _externalHttpService = externalHttpService;
    }
    public async Task<object> Handle(GetCrawlQuery request, CancellationToken cancellationToken)
    {

        var queryParams = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(request.query))
        {
            queryParams.Add("q", request.query);
        }

        string url = "https://example.com/crawl";

        return await _externalHttpService.GetAsync<object>(url, queryParams);
    }
}
