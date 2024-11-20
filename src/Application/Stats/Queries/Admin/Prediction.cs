using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.Stats.Queries.Admin;

public class Prediction
{
    public double AI_predict { get; set; }
    public int Accuracy { get; set; }
    public DateTime Date { get; set; }
}
