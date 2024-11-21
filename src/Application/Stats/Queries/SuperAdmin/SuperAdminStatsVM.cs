using warehouse_BE.Application.Stats.Queries.Admin;

namespace warehouse_BE.Application.Stats.Queries.SuperAdmin;

public class SuperAdminStatsVM
{
    public int  TotalUser { get; set; }
    public int TotalCompany { get; set; }
    public float CPU { get; set; }
    public float RAM { get; set; }
    public Prediction? Prediction { get; set; }
}

