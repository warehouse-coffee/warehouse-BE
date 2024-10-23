using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Application.SmartPriceService.Queries.GetCrawlAll;

public class GetCrawlAllQuery : IRequest<ResponseDto>
{
}
public class GetCrawlAllQueryHandler : IRequestHandler<GetCrawlAllQuery, ResponseDto>
{
    private readonly IExternalHttpService _externalHttpService;

    public GetCrawlAllQueryHandler(IExternalHttpService externalHttpService)
    {
        _externalHttpService = externalHttpService;
    }

    public async Task<ResponseDto> Handle(GetCrawlAllQuery request, CancellationToken cancellationToken)
    {
        string url = "https://example.com/crawl_all";
        var rs = new ResponseDto();
        var response =  await _externalHttpService.GetAsync<ResponseDto>(url);

        if(response != null)
        {
            rs = response;
        }
        return rs;
    }
}
