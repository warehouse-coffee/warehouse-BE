using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Common.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace warehouse_BE.Application.Logs.Queries.GetLogList;

public class GetLogListQuery : IRequest<LogList>
{
    public Page? Page { get; set; }
    public DateTime? Date { get; set; }
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
        DateTime? logDate = null;
        if (!string.IsNullOrEmpty(request.TypeLog) || request.Hour != null)
        {
            logDate = DateTime.Now; 
        }
        if (request.Date != null)
        {
            logDate = request.Date;
        }
        var logs = _loggerService.ReadLogs(
            logDate,
            string.IsNullOrEmpty(request.TypeLog) ? null : request.TypeLog,
            request.Hour 
        );

        var logVMs = logs.Select(logLine => {
            var logParts = logLine.Split(" - ", 3);
            var dateTimePart = logParts[0];
            var logLevelPart = dateTimePart.Substring(dateTimePart.IndexOf('[') + 1, 4); 
            return new LogVM
            {
                Date = dateTimePart.Substring(0, 10), 
                Hour = dateTimePart.Substring(11, 5), 
                LogLevel = logLevelPart.Trim('[', ']'), 
                Message = logParts[2], 
                Type = GetLogType(logLevelPart.Trim('[', ']')) 
            };
        }).ToList();




        int totalLogs = logVMs.Count;
        var pagedLogs = logVMs
            .Skip((request.Page?.PageNumber - 1 ?? 0) * request.Page?.Size ?? 1)
            .Take(request.Page?.Size ?? 10)
            .ToList();

        // Trả về LogList đã phân trang
        return await Task.FromResult(new LogList
        {
            LogVMs = pagedLogs,
            Status = logs.Any() ? 1 : 0,
            Page = new Page
            {
                PageNumber = request.Page?.PageNumber ?? 1,
                Size = request.Page?.Size ?? 0,
                TotalElements = pagedLogs.Count,
            }
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

