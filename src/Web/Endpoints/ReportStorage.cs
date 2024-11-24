using warehouse_BE.Application.ReportStorage.Queries.GetReportOftheStorage;

namespace warehouse_BE.Web.Endpoints
{
    public class ReportStorage : EndpointGroupBase
    { 
        public override void Map(WebApplication app)
        {
            app.MapGroup(this)
                .RequireAuthorization()
                .MapGet(GetReportStorage)
                ;
        }

        public Task<ReportVM> GetReportStorage(ISender sender, DateTime dateStart, DateTime dateEnd )
        {
            var request = new GetReportOftheStorage { 
                StartDate = dateStart 
                , EndDate = dateEnd
            };
            return sender.Send(request);
        }   
    } 
}

