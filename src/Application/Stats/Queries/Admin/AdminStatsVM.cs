namespace warehouse_BE.Application.Stats.Queries.Admin;

public class AdminStatsVM
{
    public decimal TotalInventoryValue { get; set; } 
    public int OnlineEmployeeCount { get; set; } 
    public double OrderCompletionRate { get; set; } 
    public required string HighDemandItemName { get; set; } 
    public int HighDemandItemCount { get; set; }
    public Prediction? Prediction { get; set; }
}
