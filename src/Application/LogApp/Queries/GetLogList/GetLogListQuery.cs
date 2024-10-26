using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.Logs.Queries.GetLogList;

public class GetLogListQuery : IRequest<LogList>
{
    public string? Date { get; set; }
    public string? TypeLog { get; set; }
    public int? Hour { get; set; }
}
public class GetLogListQueryHandler : IRequestHandler<GetLogListQuery, LogList> {
    private readonly ILoggerService _loggerService;
    public GetLogListQueryHandler(ILoggerService loggerService)
    {
        this._loggerService = loggerService;
    }
    public async Task<LogList> Handle(GetLogListQuery request,CancellationToken cancellationToken)
    {
        DateTime logDate;
        if (string.IsNullOrEmpty(request.Date) || !DateTime.TryParse(request.Date, out logDate))
        {
            logDate = DateTime.Now; 
        }

        var logs = _loggerService.ReadLogs(
            logDate,
            string.IsNullOrEmpty(request.TypeLog) ? null : request.TypeLog,
            request.Hour 
        );

        var logVMs = logs.Select(logLine => {
            var logParts = logLine.Split(" - ", 3);
            return new LogVM
            {
                Date = logParts[0].Substring(0, 10),
                Hour = logParts[0].Substring(11, 5),
                LogLevel = logParts[1].Trim('[', ']'),
                Message = logParts[2],
                Type = GetLogType(logParts[1])
            };
        }).ToList();

        return await Task.FromResult(new LogList
        {
            LogVMs = logVMs,
             Status = logs.Any() ? 1 : 0
        });
    }
    private string GetLogType(string logLevel)
    {
        // Logic để xác định loại log từ logLevel
        return logLevel switch
        {
            "INFO" => "Information",
            "WARNING" => "Warning",
            "ERROR" => "Error",
            _ => "Unknown" 
        };
    }
}

