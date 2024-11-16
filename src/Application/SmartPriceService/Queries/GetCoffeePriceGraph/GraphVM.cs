using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.SmartPriceService.Queries.GetCoffeePriceGraph;

public class GraphVM
{
    public List<PointInfo> pointInfos {  get; set; } = new List<PointInfo>();
}
