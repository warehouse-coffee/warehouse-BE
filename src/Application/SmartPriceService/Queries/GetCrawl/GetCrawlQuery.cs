using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Domain.ValueObjects;

namespace warehouse_BE.Application.SmartPriceService.Queries.GetCrawl;

public class GetCrawlQuery : IRequest<CrawlResponse>
{
    public string? Query;
}
public class GetCrawlQueryHandler : IRequestHandler<GetCrawlQuery, CrawlResponse>
{
    private readonly IExternalHttpService _externalHttpService;
    private readonly IApplicationDbContext _context;

    public GetCrawlQueryHandler(IExternalHttpService externalHttpService, IApplicationDbContext context)
    {
        _externalHttpService = externalHttpService;
        _context = context;
    }

    public async Task<CrawlResponse> Handle(GetCrawlQuery request, CancellationToken cancellationToken)
    {
        const string key = ConfigurationKeys.AiDriverServer;

        var config = await _context.Configurations
            .FirstOrDefaultAsync(c => c.Key == key, cancellationToken);

        var rs = new CrawlResponse();

        if (config == null)
        {
            return rs; 
        }

        var baseUrl = config.Value;

        var endpoint = $"{baseUrl}/crawl?q={request.Query}";

        var response = await _externalHttpService.GetAsync<CrawlResponse>(endpoint);
        if (response != null)
        {
           rs = response; 
        }

        return rs; 
    }
}
