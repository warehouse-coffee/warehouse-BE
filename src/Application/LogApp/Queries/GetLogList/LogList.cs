﻿

using warehouse_BE.Application.Common.Models;

namespace warehouse_BE.Application.Logs.Queries.GetLogList;

public class LogList
{
    public List<LogVM> LogVMs { get; set; }
    public int Status { get; set; }
    public Page? Page { get; set; }
    public LogList()
    {
        LogVMs = new List<LogVM>();

    }
}
