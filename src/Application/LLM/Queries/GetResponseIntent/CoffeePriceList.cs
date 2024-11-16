using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.LLM.Queries.GetResponseIntent;

public class CoffeePriceList
{
    public  required List<CoffeePriceTodayData> Data { get; set; }

}
