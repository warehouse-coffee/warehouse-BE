using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.LLM.Queries.GetResponseIntent;
using warehouse_BE.Domain.ValueObjects;

namespace warehouse_BE.Application.SmartPriceService.Queries.GetCoffeePriceGraph;

public class GetCoffeePriceGraphQuery : IRequest<GraphVM>
{
}
public class GetCoffeePriceGraphQueryHandler : IRequestHandler<GetCoffeePriceGraphQuery, GraphVM>
{

    private readonly IExternalHttpService externalHttpService;
    private readonly IApplicationDbContext context;
    private readonly ILoggerService logger;

    public GetCoffeePriceGraphQueryHandler(IExternalHttpService externalHttpService, IApplicationDbContext context, ILoggerService logger)
    {
        this.externalHttpService = externalHttpService;
        this.context = context;
        this.logger = logger;
    }
    public async Task<GraphVM> Handle(GetCoffeePriceGraphQuery request, CancellationToken cancellationToken)
    {
        var rs = new GraphVM();
        const string key = ConfigurationKeys.AiDriverServer;
        try
        {
            var config = await context.Configurations
                   .FirstOrDefaultAsync(c => c.Key == key, cancellationToken);
            if (config != null)
            {
                var baseUrl = config.Value;
                var endpoint = $"{baseUrl}/predict_graph";
                var data = await externalHttpService.GetAsync<List<PointInfo>>(endpoint);
                if (data != null)
                {
                    foreach (var point in data)
                    {
                        point.ai_predict = Math.Round(point.ai_predict, 2);
                        point.real_price_difference_rate = Math.Round(point.real_price_difference_rate, 2);
                    }
                    rs.pointInfos = data;
                }
            }
        } catch (Exception ex)
        {
            logger.LogError("Error at getCoffee Price  Graph ", ex);
        }
        
        return rs;
    }
}
