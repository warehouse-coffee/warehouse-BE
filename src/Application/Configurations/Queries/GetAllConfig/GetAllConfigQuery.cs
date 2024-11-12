using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Domain.ValueObjects;

namespace warehouse_BE.Application.Configurations.Queries.GetAllConfig;

public class GetAllConfigQuery : IRequest<ConfigVm>
{

}
public class GetAllConfigQueryHandler : IRequestHandler<GetAllConfigQuery, ConfigVm> 
{
    private readonly IApplicationDbContext _context;
    public GetAllConfigQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ConfigVm> Handle(GetAllConfigQuery request,CancellationToken cancellationToken)
    {
        var rs = new ConfigVm();

        var configurations = await _context.Configurations
            .Where(c => c.Key == ConfigurationKeys.AIServiceKey ||
                        c.Key == ConfigurationKeys.EmailServiceKey ||
                        c.Key == ConfigurationKeys.AiDriverServer)
            .ToListAsync(cancellationToken);

        if (configurations != null)
        {
            rs.AIServiceKey = configurations
                .FirstOrDefault(c => c.Key == ConfigurationKeys.AIServiceKey)?.Value ?? string.Empty;

            rs.EmailServiceKey = configurations
                .FirstOrDefault(c => c.Key == ConfigurationKeys.EmailServiceKey)?.Value ?? string.Empty;

            rs.AiDriverServer = configurations
                .FirstOrDefault(c => c.Key == ConfigurationKeys.AiDriverServer)?.Value ?? string.Empty;
        }

        return rs;
    }
}

