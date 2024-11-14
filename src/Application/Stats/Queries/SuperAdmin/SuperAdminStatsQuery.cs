using System.Diagnostics;
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.Stats.Queries.SuperAdmin;

public  class SuperAdminStatsQuery : IRequest<SuperAdminStatsVM>
{
}
public class SuperAdminStatsQueryHandler : IRequestHandler<SuperAdminStatsQuery, SuperAdminStatsVM>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly ILoggerService _loggerService;
    public SuperAdminStatsQueryHandler(IApplicationDbContext context, IIdentityService identityService, ILoggerService loggerService)
    {
        _context = context;
        _identityService = identityService;
        _loggerService = loggerService;
    }
    public async Task<SuperAdminStatsVM> Handle(SuperAdminStatsQuery request, CancellationToken cancellationToken)
    {
        var rs = new SuperAdminStatsVM();

        var company = await _context.Companies.Where(o => o.CompanyId != "USERCOMPANY").CountAsync(cancellationToken);
        var user = await _identityService.GetTotalUser();

        float cpu = 0;
        float ram = 0;

        try
        {
#pragma warning disable CA1416 // Validate platform compatibility
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue(); // Initial call returns 0
            await Task.Delay(1000); // Wait for accurate reading
            cpu = (float)Math.Round(cpuCounter.NextValue(), 2); // Format to 00.00
#pragma warning restore CA1416 // Validate platform compatibility
            // Get RAM usage as a percentage
            var process = Process.GetCurrentProcess();
            long usedMemory = process.WorkingSet64;
            float totalMemoryMB = 2 * 1024;
            float usedMemoryMB = process.WorkingSet64 / (1024 * 1024);
            ram = (float)Math.Round((usedMemoryMB / totalMemoryMB) * 100, 2);
        }
        catch (Exception ex)
        {
            _loggerService.LogError("Error at SuperAdmin Stats : ", ex);
        }

        rs.TotalCompany = company;
        rs.TotalUser = user;
        rs.CPU = cpu;
        rs.RAM = ram;
        return rs;
    }
}
