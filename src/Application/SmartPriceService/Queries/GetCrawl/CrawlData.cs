using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.SmartPriceService.Queries.GetCrawl;

public class CrawlData
{
    public int Index { get; set; }
    public string? Date { get; set; }
    public long UnixDateMs { get; set; }
    public double Value { get; set; }
}