using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.SmartPriceService.Queries.GetAllLinks;

public class GetAllLinksQuery : IRequest<object>
{
}
public class GetAllLinksQueryHandler : IRequestHandler<GetAllLinksQuery, object>
{
    private readonly IExternalHttpService _externalHttpService;

    public GetAllLinksQueryHandler(IExternalHttpService externalHttpService)
    {
        _externalHttpService = externalHttpService;
    }

    public async Task<object> Handle(GetAllLinksQuery request, CancellationToken cancellationToken)
    {
        string url = "https://example.com/link";
        return await _externalHttpService.GetAsync<object>(url);
    }
}

