using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.LLM.Queries.GetResponseIntent;

public class CoffeePriceTommorow
{
    public int Index { get; set; }
    public double AI_predict { get; set; }
    public string? Date { get; set; }
    public long unix_date_ms { get; set; }
    public string? Message { get; set; }
}
