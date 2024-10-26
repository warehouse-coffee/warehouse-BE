using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.Logs.Queries.GetLogList;

public  class LogVM
{
    public string? Date { get; set; }
    public string? LogLevel { get; set; }
    public string? Message { get; set; }
    public string? Hour { get; set; }
    public string? Type { get; set; }
}
