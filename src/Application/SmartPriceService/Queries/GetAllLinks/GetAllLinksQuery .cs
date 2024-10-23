using Microsoft.Extensions.Configuration;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Domain.ValueObjects;

namespace warehouse_BE.Application.SmartPriceService.Queries.GetAllLinks;

public class GetAllLinksQuery : IRequest<CommodityLinks>
{
}
public class GetAllLinksQueryHandler : IRequestHandler<GetAllLinksQuery, CommodityLinks>
{
    private readonly IExternalHttpService _externalHttpService;
    private readonly IApplicationDbContext _context;

    public GetAllLinksQueryHandler(IExternalHttpService externalHttpService, IApplicationDbContext context)
    {
        this._externalHttpService = externalHttpService;
       this._context = context;
    }

    public async Task<CommodityLinks> Handle(GetAllLinksQuery request, CancellationToken cancellationToken)
    {
        const string key = ConfigurationKeys.AiDriverServer;
        var rs = new CommodityLinks();
        // Fetch the configuration from the database
        var config = await _context.Configurations
            .FirstOrDefaultAsync(c => c.Key == key, cancellationToken);

        // Ensure the configuration was found
        if (config == null)
        {
            return rs;
        }

        // Get the base URL from the configuration value
        var baseUrl = config.Value;

        // Construct the endpoint URL
        var endpoint = $"{baseUrl}/link";

        // Make the external HTTP call to get commodity links
        var commodityLinks = await _externalHttpService.GetAsync<CommodityLinks>(endpoint);
        if(commodityLinks != null)
        {
            rs = commodityLinks;
        }
        return rs;
    }
}

