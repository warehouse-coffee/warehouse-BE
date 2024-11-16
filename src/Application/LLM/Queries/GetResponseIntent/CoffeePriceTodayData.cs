using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.LLM.Queries.GetResponseIntent;

public class CoffeePriceTodayData
{
    public int Index { get; set; }
    public string? Date { get; set; }
    public float unix_date_ms { get; set; }
    public float real_price { get; set; }
}
