namespace warehouse_BE.Application.ReportStorage.Queries.GetReportOftheStorage;

public class ReportVM
{
    public ReportVM()
    {
        WarehouseStatistics = new List<WarehousePerformance>();
        ImportStatistics = new List<ImportSummary>();
        TopProducts = new List<ProductPerformance>();
        SlowMovingProducts = new List<ProductPerformance>();
    }
    public required List<WarehousePerformance> WarehouseStatistics { get; set; } 
    public required List<ImportSummary> ImportStatistics { get; set; } 
    public required List<ProductPerformance> TopProducts { get; set; }
    public required List<ProductPerformance> SlowMovingProducts { get; set; }

    public decimal TotalRevenue { get; set; }
    public decimal TotalImportCost { get; set; }
    public int TotalOrders { get; set; } 
}
