using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.LLM.Queries.GetResponseIntent;

public class GetResponseIntentQuery : IRequest<string>
{
    public required string Itent {  get; set; }
}
public class GetResponseIntentQueryHandler : IRequestHandler<GetResponseIntentQuery, string>
{
    private  readonly IApplicationDbContext context;
    private readonly ILoggerService logger;
    private readonly IIdentityService identityService;

    public GetResponseIntentQueryHandler(IApplicationDbContext context, ILoggerService logger, IIdentityService identityService)
    {
        this.context = context;
        this.logger = logger;
        this.identityService = identityService;
    }
    public async Task<string> Handle(GetResponseIntentQuery request, CancellationToken cancellationToken)
    {
        string rs = "";

        try
        {
            switch (request.Itent.ToLower()) // Dùng ToLower() để tránh trường hợp phân biệt chữ hoa chữ thường
            {
                case "empty_storage":
                    var data = await context.Storages.Where(o => o.IsDeleted).ToListAsync();
                    rs = "Response for empty storage";
                    break;

                case "empty_area":
                    rs = "Response for empty area";
                    break;

                case "coffee_price_today":
                    rs = "Response for coffee price today";
                    break;

                case "near_expired_product":
                    rs = "Response for near expired product";
                    break;

                case "most_valuable_product":
                    rs = "Response for most valuable product";
                    break;

                case "coffee_price_tomorrow":
                    rs = "Response for coffee price tomorrow";
                    break;

                case "others":
                    rs = "Response for others";
                    break;

                default:
                    rs = "Unknown intent";
                    break;
            }
        } catch (Exception ex)
        {
            logger.LogError("Error at LLM get Instent", ex);
        }
        return rs;
    }
}
