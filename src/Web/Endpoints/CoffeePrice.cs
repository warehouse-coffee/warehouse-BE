using Microsoft.AspNetCore.Http.HttpResults;
using warehouse_BE.Application.CompanyOwner.Queries.GetCompanyOwnerList;
using warehouse_BE.Application.SmartPriceService.Queries.GetCoffeePriceGraph;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace warehouse_BE.Web.Endpoints
{
    public class CoffeePrice : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            app.MapGroup(this).RequireAuthorization()
            .MapGet(GetCoffeePricePredictGraph,"/predict-graph")
            ;
        }

        public Task<GraphVM> GetCoffeePricePredictGraph(ISender sender)
        {
            return sender.Send(new GetCoffeePriceGraphQuery() );
        }
    }
}
