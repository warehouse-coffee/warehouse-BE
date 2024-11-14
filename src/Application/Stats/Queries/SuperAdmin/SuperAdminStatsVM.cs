using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.Stats.Queries.SuperAdmin;

public class SuperAdminStatsVM
{
    public int  TotalUser { get; set; }
    public int TotalCompany { get; set; }
    public float CPU { get; set; }
    public float RAM { get; set; }
}
