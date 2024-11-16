using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.SmartPriceService.Queries.GetCoffeePriceGraph;

public class PointInfo
{
    public string? Date { get; set; }
    public double ai_predict { get; set; }
    public double real_price_difference_rate { get; set; }
}
