using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.SmartPriceService.Queries.GetCrawl;

public class CrawlResponse
{
    public long UnixTimeNow { get; set; }
    public string? DateNow { get; set; }
    public List<CrawlData>? Data { get; set; }
}
